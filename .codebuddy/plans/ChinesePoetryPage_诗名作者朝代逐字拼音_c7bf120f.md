---
name: ChinesePoetryPage 诗名作者朝代逐字拼音
overview: 在古诗词学习页为诗名、作者、朝代添加与诗词正文风格一致的逐字拼音标注（每个汉字正上方标注带声调拼音），并支持点击朗读对应文字。
todos:
  - id: poetry-item-pinyin
    content: 用 [mcp:codebase-memory-mcp] 确认引用点后，改造 PoetryItem 暴露拼音转换能力与三个拼音属性
    status: completed
  - id: xaml-containers
    content: 改造 ChinesePoetryPage.xaml，将标题/作者/朝代静态 Label 替换为拼音容器并保留分隔符居中
    status: completed
    dependencies:
      - poetry-item-pinyin
  - id: render-pinyin
    content: 实现 BuildPinyinRow 辅助方法，填充诗名/作者/朝代容器并复用至诗词正文
    status: completed
    dependencies:
      - xaml-containers
  - id: verify-build
    content: 编译项目并逐首切换诗作，验证拼音对齐、降级空拼音与点击朗读
    status: completed
    dependencies:
      - render-pinyin
---

## 问题概述

Android Release 打包失败，报错：

```
IL Trimmer has encountered an unexpected error. Please report the issue at https://aka.ms/report-illink
优化程序集大小失败。
```

即 ILLink（IL 裁剪器）在 Release 阶段内部崩溃，导致 APK 无法产出。

## 核心目标

1. **拿到可用的 Release APK**（首要，恢复出包能力）
2. **查清崩溃根因**，判断是缓存损坏、workload 版本问题，还是某个程序集不兼容裁剪
3. 在根因解决后，尽量恢复完整裁剪（`TrimMode=full`）以保住包体积

## 关键前提（用户已确认）

- **此前成功过，最近才失败** → 属回归问题，优先排查增量因素（近期代码改动、构建缓存损坏），而非"配置从一开始就不对"
- **两条腿走** → 先把裁剪降级拿到 APK，同时抓完整日志查根因，成功后可再恢复完整裁剪

## 边界约束

- 不改动任何业务代码（诗词/拼音/朗读逻辑）
- 不修改签名配置（keystore 路径与密码）
- 临时日志写入 `%TEMP%`，不污染工作区
- 最终若需保留降级配置，必须在 csproj 中留下注释说明原因，便于日后恢复


## 技术栈与环境（已实测）

| 项目 | 值 |
|---|---|
| .NET SDK | 9.0.312 |
| android workload | 35.0.78/9.0.100（**有可用更新**） |
| maui-windows workload | 9.0.111/9.0.100（**有可用更新**） |
| workload 来源 | VS 17.14.37027.9 |
| 项目 | `EnglishReadingApp/EnglishReadingApp.csproj` |

## 成因分析

ILLink 的 "unexpected error" 是**无编号的内部异常**，区别于 IL1012 这类有编号的裁剪告警。结合"此前成功、最近才失败"这一关键线索，按嫌疑度排序：

**1. obj 增量缓存损坏（最高嫌疑）**

我在排查过程中执行过 `dotnet build ... -t:Compile`，该命令跳过 XamlG（XAML 预编译），曾导致：

```
CSC : error CS5001: 程序不包含适合于入口点的静态 "Main" 方法
App.xaml.cs(19,18): error CS1061: "App"未包含"InitializeComponent"的定义
```

这类**半途中断的构建会让 `obj/` 处于不一致状态**（残留不完整/损坏的中间程序集）。ILLink 处理这些损坏程序集时极易内部崩溃。这是"此前成功、最近才失败"最吻合的解释。

现场证据：`obj\Release\net9.0-android\` 下已有 `android-arm64\aot\arm64-v8a\`、`android-x64\aot\x86_64\` 的 AOT 中间产物，`bin\Release\net9.0-android\publish\` 下也有 `com.companyname.englishreadingapp...` 文件 —— 说明发布流程跑到了很深阶段才崩，不是早期配置错误，符合"处理到某个损坏程序集时崩溃"的特征。

**2. workload / SDK 版本 bug**

`dotnet workload list` 明确提示 android/ios/maccatalyst/maui-windows 均有可用更新。已知某些 workload 版本存在 ILLink 崩溃缺陷。

**3. 第三方程序集不兼容裁剪**

项目中两个小众包未标记 trimming-safe：

```84:89:EnglishReadingApp/EnglishReadingApp.csproj
		<PackageReference Include="CingZeoi.ChinesePinyinConverter" Version="1.0.0" />
		<PackageReference Include="Microsoft.International.Converters.PinYinConverter" Version="1.0.0" />
		<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
		<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="9.0.8" />
		<PackageReference Include="PinYin" Version="1.0.1" />
		<PackageReference Include="Plugin.Maui.Audio" Version="4.0.0" />
```

但这两个包是项目早期就引入的，与"最近才失败"不吻合，嫌疑低于前两项。

**4. 近期代码改动（需排除）**

新增 `Entity/PolyphonicPinyin.cs`（值元组数组 + LINQ + Dictionary）、修改 `Entity/PoetryItem.cs` 与诗词页面。纯托管代码理论上不会触发 ILLink 崩溃，但需用 `detect_changes` 确认影响范围后排除。

## 实现方案

采用**先最小代价修复、再逐级降级**的递进策略，而非直接降级：

```
清理缓存 → full 模式发布（抓日志）→ 成功? ── 是 ──> 问题解决，体积不变
                                        │
                                        否
                                        ↓
                              切 partial 出包（拿 APK）
                                        ↓
                              分析日志定位根因 → 决定终态
```

**为什么先清理缓存再试 full**：最高嫌疑就是缓存损坏，清理后 full 模式成功概率高。若成功，则无需降级，APK 体积保持最优。这一步代价仅一次构建时间，值得先试。

**裁剪档位权衡**：

| 档位 | 说明 | 体积 | 崩溃风险 |
|---|---|---|---|
| `TrimMode=full` | 当前默认值，裁剪最彻底 | 最小 | 高 |
| `TrimMode=partial` | 仅裁剪 BCL/SDK 已标记程序集，不裁剪应用代码与未标记包 | 适度增大 | 低 |
| `PublishTrimmed=false` | 完全不裁剪 | 最大 | 无（仅作最后兜底） |

优先用 `partial` 而非 `PublishTrimmed=false`：`partial` 仍能裁掉大量 BCL 代码，且不会因禁用裁剪触发 `RunAOTCompilation` 相关的连带问题（已知 .NET 7 起 `PublishTrimmed=false` 与 AOT 组合会报 MSBuild 错误）。

## 关键操作

**目标配置位置**（当前无任何裁剪设置，使用 MAUI Android Release 默认）：

```45:53:EnglishReadingApp/EnglishReadingApp.csproj
	<PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net9.0-android|AnyCPU'">
	  <AndroidPackageFormat>apk</AndroidPackageFormat>
	  <AndroidKeyStore>True</AndroidKeyStore>
	  <AndroidSigningStorePass>hengkangit</AndroidSigningStorePass>
	  <AndroidSigningKeyAlias>hengkangit</AndroidSigningKeyAlias>
	  <AndroidSigningKeyPass>hengkangit</AndroidSigningKeyPass>
	  <AndroidUseAapt2>True</AndroidUseAapt2>
	  <AndroidCreatePackagePerAbi>False</AndroidCreatePackagePerAbi>
	</PropertyGroup>
```

若需降级，在该组追加（**带注释**）：

```xml
<!-- 裁剪降级为 partial：full 模式触发 ILLink 内部异常（unexpected error）。
     根因定位并修复后可改回 full 以减小体积。 -->
<TrimMode>partial</TrimMode>
```

**清理缓存**（仅清 Android Release 相关目录，避免误伤其他 TFM）：

```powershell
Remove-Item -Recurse -Force `
  EnglishReadingApp\bin\Release\net9.0-android, `
  EnglishReadingApp\obj\Release\net9.0-android -ErrorAction SilentlyContinue
```

**发布并抓日志**（日志写 `%TEMP%`，不污染工作区）：

```powershell
dotnet publish EnglishReadingApp\EnglishReadingApp.csproj `
  -f net9.0-android -c Release -v detailed 2>&1 | `
  Out-File "$env:TEMP\publish-full.log" -Encoding utf8
```

日志分析重点：`ILLink`、`unexpected error`、`Fatal error in IL Linker`、异常堆栈中出现的**程序集名称**（这是定位根因的关键线索）。

## 执行注意事项

- **时间预算**：Android Release 完整发布含 AOT 编译，单次需数分钟，需预留；可能要跑 2 次（full 失败后再 partial）
- **清理范围**：只删 `net9.0-android` 的 Release 目录，保留其他 TFM 与 Debug 产物，减少重建成本
- **恢复能力**：NuGet 包已在全局缓存，清理后 restore 可离线完成
- **workload 更新是备选**：`dotnet workload update` 需联网且下载量大（数 GB），仅在日志明确指向 workload 缺陷时才执行
- **不改签名**：keystore 与四组密码一律不动
- **产物验证**：成功后确认 `bin\Release\net9.0-android\publish\` 下生成已签名 `.apk`，并记录体积用于对比

## 目录结构

```
EnglishReadingApp/
└── EnglishReadingApp.csproj    # [MODIFY] 仅在需要降级时，于 Release|net9.0-android
                                #          配置组追加 <TrimMode>partial</TrimMode> 及原因注释
                                #          若清理缓存后 full 模式成功，则此文件不改动

%TEMP%\publish-full.log         # [NEW][临时] 完整发布日志，用于定位 ILLink 崩溃根因，用后清理
```

（业务代码 `Entity/`、`Chinese/` 等一律不动）


## Agent Extensions

### MCP

- **codebase-memory-mcp**
  - 用途：用 `detect_changes` 映射近期改动（新增 `PolyphonicPinyin.cs`、修改 `PoetryItem.cs` 与诗词页面）的传递影响范围，确认这些改动是否触及任何与裁剪/AOT 相关的代码路径
  - 预期结果：得出明确结论——近期改动是否可作为 ILLink 崩溃的诱因，从而予以排除或列为嫌疑

### SubAgent

- **codebase-memory-scout**
  - 用途：核查项目中是否存在 `[Preserve]`、`TrimmerRootDescriptor`、`IsTrimmable` 等既有裁剪相关配置或自定义 linker 配置文件，避免重复引入或相互冲突
  - 预期结果：确认项目当前裁剪配置的完整现状，为最终固化方案提供依据
