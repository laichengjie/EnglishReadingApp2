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

## 产品概述

为古诗词拼音标注增加多音字读音修正能力。通过一张可维护的映射表，覆盖拼音库在特定语境下的误读（如「望庐山瀑布」的「瀑」被转为 `bào`，正确应为 `pù`）。

## 核心功能

- **多音字映射表**：支持词组级与单字级两级覆盖，可随诗作扩充持续维护
- **词组优先匹配**：先按词组匹配（如「瀑布」「一行」），未命中的字再查单字表；避免误伤同类字（「门泊」读 bó 不影响「湖泊」读 pō）
- **全链路生效**：诗名、作者、朝代、诗文正文的拼音共用同一转换入口，映射表接入一次即全局生效
- **自动转声调符号**：映射表只需写数字声调（`pu4`），由既有逻辑自动渲染为 `pù`

## 边界说明

- 只修正读音，不改动现有 UI 排版、字号与配色
- 不涉及朗读顺序、录音评分、译文切换、上下首导航等既有逻辑
- 映射表内容以实际转换结果为依据，不预先臆断填充


## 技术栈

- 框架：.NET MAUI 9（C# + XAML），目标框架含 `net9.0-windows10.0.19041.0`
- 拼音转换：已引用的 `CingZeoi.ChinesePinyinConverter` 1.0.0，无需新增 NuGet 包
- 离线验证：临时 .NET 控制台项目（置于 `%TEMP%`），通过 `<Compile Include>` 直接编译 `PoetryItem.cs`——该文件不依赖 MAUI，此方式已验证可行

## 实现方案

### 总体策略

在拼音生成的**数字声调阶段**插入一层覆盖：`ConvertToPinyin` 输出音节序列后，先用多音字映射表修正，再交给既有的 `ConvertNumberToToneMark` 转声调符号。所有拼音渲染路径（诗名/作者/朝代/正文）共用 `GetPinyinWithToneSymbol` 这一入口，因此只需接入一次即全局生效。

### 关键技术决策

**1. 介入时机选在数字声调层**

`ConvertToPinyin` 返回 `pu4` 这类带数字声调的序列，`ConvertNumberToToneMark` 负责转符号（含 a/o/e/i/u/ü 的标调优先级规则）。映射表写数字声调即可完整复用这套规则，无需重复实现标调逻辑，也避免 `v`/`ü` 处理不一致。

**2. 词组优先 + 单字兜底**

单字全局覆盖会误伤：「泊」在「门泊」读 bó 而「湖泊」读 pō；「行」在「一行」读 háng 而「行人」读 xíng。故采用两级匹配：先做最长词组匹配（命中则整段替换并跳过该区间），剩余位置再查单字表。

**3. 新建独立类而非内联字典**

映射表会随诗作扩充持续增长。独立为 `PolyphonicPinyin` 静态类，与 `PoetryItem` 的数据职责分离，便于后续单独维护或改为配置驱动。

**4. 先验证后填表**

先跑一次不启用映射的基线输出，用真实数据定位误读字，再填表，避免凭印象加错音。已知必须修正的是「瀑」（bào→pù）；「行」「泊」「绿」及「少/更/得/曲/觉/落/尽/思/看/中/当」等需以验证结果为准，不臆断。

### 性能

转换仅在切换诗作时执行，单首最多数十次；词组匹配为 O(n × L)（n 为汉字数，L 为最长词组长度，通常 2-3），开销可忽略。映射表用 `static readonly` 字典，无重复构建成本。

## 实现注意事项

- **防御性处理**：`Apply` 必须校验音节数与汉字数，不等时只覆盖安全区间、越界直接跳过，避免索引异常导致整个转换降级返回原中文。
- **最长词组匹配**：从每个位置由长到短尝试匹配，命中后该区间不再参与单字兜底。
- **验证项目需同步**：`PoetryItem.cs` 被临时控制台项目直接引用，新建 `PolyphonicPinyin.cs` 后必须一并加入 `<Compile Include>`，否则验证时编译失败。
- **保留既有降级保护**：`pinyin == chinese` 时置空拼音行的逻辑保持不变，不因新增覆盖而破坏。
- **不改动 `BuildPinyinRow`** 的字号（正文 30 / 拼音 14）、颜色（`#2C3E50`）与宽度策略。
- 临时验证项目放 `%TEMP%`，验证完成后清理。

## 架构设计

改动集中在数据层，展示层无感知：

```mermaid
flowchart LR
    A[汉字串] --> B[ConvertToPinyin 数字声调序列]
    B --> C[PolyphonicPinyin.Apply 词组优先+单字兜底]
    C --> D[ConvertNumberToToneMark 声调符号]
    D --> E[拼音串 供渲染使用]
```

- 数据层：新增 `PolyphonicPinyin`，改造 `GetPinyinWithToneSymbol`
- 展示层：`ChinesePoetryPage` 不改动，经同一入口自动受益

## 目录结构

```
EnglishReadingApp/
├── Entity/
│   ├── PolyphonicPinyin.cs         # [NEW] 多音字覆盖表与匹配逻辑
│   │     - PhraseOverrides：词组级覆盖（优先匹配），键为词组，值为等长数字声调序列
│   │     - CharOverrides：单字级覆盖（词组未命中时兜底）
│   │     - Apply(chinese, syllables)：按词组优先、单字兜底修正音节序列
│   │     - 职责：承载读音修正规则，与 PoetryItem 的数据职责分离
│   │
│   └── PoetryItem.cs               # [MODIFY] 接入覆盖
│         - GetPinyinWithToneSymbol：在 ConvertToPinyin 之后、ConvertNumberToToneMark 之前调用 Apply
│         - 职责：负责把汉字串转为带声调符号的拼音串
│
└── Chinese/
    └── ChinesePoetryPage.xaml.cs   # 无需改动（诗名/作者/朝代/正文拼音均经同一入口，自动受益）
```

## 关键代码结构

多音字覆盖表（新增，接口级定义）：

```csharp
public static class PolyphonicPinyin
{
    // 词组级覆盖（优先匹配）。键为词组，值为与词组等长的数字声调序列，空格分隔
    public static readonly IReadOnlyDictionary<string, string> PhraseOverrides;

    // 单字级覆盖（词组未命中时兜底）。键为单个汉字，值为数字声调拼音
    public static readonly IReadOnlyDictionary<string, string> CharOverrides;

    /// 按"词组优先、单字兜底"修正音节序列。
    /// chinese 与 syllables 长度不等时只处理安全区间，越界跳过。
    public static void Apply(string chinese, IList<string> syllables);
}
```

`GetPinyinWithToneSymbol` 接入示意：

```csharp
var syllables = ChinesePinyinConverter.PinyinConverter
    .ConvertToPinyin(chinese, withTone: true)?.ToList();

if (syllables == null || syllables.Count == 0) return chinese;

PolyphonicPinyin.Apply(chinese, syllables);   // 新增：数字声调层的多音字覆盖

return ConvertNumberToToneMark(string.Join(" ", syllables));
```


## Agent Extensions

### MCP

- **codebase-memory-mcp**
  - 用途：用 `search_code` 确认 `GetPinyinWithToneSymbol` 的全部调用点，验证诗名、作者、朝代、诗文正文四条拼音路径是否均经由该入口
  - 预期结果：得到完整调用清单，证明映射表接入一次即全局生效，不存在绕过该入口的拼音渲染路径

### SubAgent

- **codebase-memory-scout**
  - 用途：复核 `Chinese/` 目录下其他页面（ChinesePage、ChineseLearningPage、ChinesePinyinAlphabetPage）是否存在不经过 `PoetryItem` 的独立汉字转拼音逻辑
  - 预期结果：确认是否还有其他拼音渲染入口需要同步接入映射表，避免改完仍有页面显示错误读音
