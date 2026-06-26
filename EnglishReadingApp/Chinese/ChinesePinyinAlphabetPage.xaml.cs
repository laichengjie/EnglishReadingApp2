
using Plugin.Maui.Audio;
using System.Diagnostics;

namespace EnglishReadingApp;

public partial class ChinesePinyinAlphabetPage : ContentPage
{
    private readonly QwenTTSService _qwenTTS;

    // 一、声母表（23个）
    private List<PinyinItem> _initials = new()
    {
        new PinyinItem { Pinyin = "b", Chinese = "玻", Example = "玻璃" },
        new PinyinItem { Pinyin = "p", Chinese = "坡", Example = "山坡" },
        new PinyinItem { Pinyin = "m", Chinese = "摸", Example = "抚摸" },
        new PinyinItem { Pinyin = "f", Chinese = "佛", Example = "佛像" },
        new PinyinItem { Pinyin = "d", Chinese = "得", Example = "得到" },
        new PinyinItem { Pinyin = "t", Chinese = "特", Example = "特别" },
        new PinyinItem { Pinyin = "n", Chinese = "讷", Example = "讷讷" },
        new PinyinItem { Pinyin = "l", Chinese = "勒", Example = "勒紧" },
        new PinyinItem { Pinyin = "g", Chinese = "哥", Example = "哥哥" },
        new PinyinItem { Pinyin = "k", Chinese = "科", Example = "科学" },
        new PinyinItem { Pinyin = "h", Chinese = "喝", Example = "喝水" },
        new PinyinItem { Pinyin = "j", Chinese = "鸡", Example = "小鸡" },
        new PinyinItem { Pinyin = "q", Chinese = "欺", Example = "欺负" },
        new PinyinItem { Pinyin = "x", Chinese = "西", Example = "西瓜" },
        new PinyinItem { Pinyin = "zh", Chinese = "知", Example = "知道" },
        new PinyinItem { Pinyin = "ch", Chinese = "吃", Example = "吃饭" },
        new PinyinItem { Pinyin = "sh", Chinese = "诗", Example = "诗歌" },
        new PinyinItem { Pinyin = "r", Chinese = "日", Example = "日子" },
        new PinyinItem { Pinyin = "z", Chinese = "资", Example = "资本" },
        new PinyinItem { Pinyin = "c", Chinese = "刺", Example = "刺猬" },
        new PinyinItem { Pinyin = "s", Chinese = "思", Example = "思考" },
        new PinyinItem { Pinyin = "y", Chinese = "衣", Example = "衣服" },
        new PinyinItem { Pinyin = "w", Chinese = "乌", Example = "乌鸦" }
    };

    // 二、单韵母（6个）
    private List<PinyinItem> _singleVowels = new()
    {
        new PinyinItem { Pinyin = "a", Chinese = "啊", Example = "啊呀" },
        new PinyinItem { Pinyin = "o", Chinese = "喔", Example = "喔喔" },
        new PinyinItem { Pinyin = "e", Chinese = "鹅", Example = "天鹅" },
        new PinyinItem { Pinyin = "i", Chinese = "衣", Example = "衣服" },
        new PinyinItem { Pinyin = "u", Chinese = "乌", Example = "乌云" },
        new PinyinItem { Pinyin = "ü", Chinese = "鱼", Example = "小鱼" }
    };

    // 三、复韵母（18个）
    private List<PinyinItem> _compoundVowels = new()
    {
        new PinyinItem { Pinyin = "ai", Chinese = "哀", Example = "悲哀" },
        new PinyinItem { Pinyin = "ei", Chinese = "诶", Example = "诶呀" },
        new PinyinItem { Pinyin = "ui", Chinese = "威", Example = "威风" },
        new PinyinItem { Pinyin = "ao", Chinese = "熬", Example = "熬夜" },
        new PinyinItem { Pinyin = "ou", Chinese = "欧", Example = "欧洲" },
        new PinyinItem { Pinyin = "iu", Chinese = "优", Example = "优秀" },
        new PinyinItem { Pinyin = "ie", Chinese = "耶", Example = "耶耶" },
        new PinyinItem { Pinyin = "üe", Chinese = "约", Example = "约定" },
        new PinyinItem { Pinyin = "er", Chinese = "耳", Example = "耳朵" },
        new PinyinItem { Pinyin = "an", Chinese = "安", Example = "安全" },
        new PinyinItem { Pinyin = "en", Chinese = "恩", Example = "恩情" },
        new PinyinItem { Pinyin = "in", Chinese = "因", Example = "因为" },
        new PinyinItem { Pinyin = "un", Chinese = "温", Example = "温暖" },
        new PinyinItem { Pinyin = "ün", Chinese = "晕", Example = "晕倒" },
        new PinyinItem { Pinyin = "ang", Chinese = "昂", Example = "昂扬" },
        new PinyinItem { Pinyin = "eng", Chinese = "亨", Example = "亨通" },
        new PinyinItem { Pinyin = "ing", Chinese = "英", Example = "英雄" },
        new PinyinItem { Pinyin = "ong", Chinese = "翁", Example = "老翁" }
    };

    // 四、整体认读音节（16个）
    private List<PinyinItem> _wholeSyllables = new()
    {
        new PinyinItem { Pinyin = "zhi", Chinese = "织", Example = "织布" },
        new PinyinItem { Pinyin = "chi", Chinese = "吃", Example = "吃饭" },
        new PinyinItem { Pinyin = "shi", Chinese = "狮", Example = "狮子" },
        new PinyinItem { Pinyin = "ri", Chinese = "日", Example = "日子" },
        new PinyinItem { Pinyin = "zi", Chinese = "资", Example = "资本" },
        new PinyinItem { Pinyin = "ci", Chinese = "次", Example = "次数" },
        new PinyinItem { Pinyin = "si", Chinese = "丝", Example = "丝绸" },
        new PinyinItem { Pinyin = "yi", Chinese = "衣", Example = "衣服" },
        new PinyinItem { Pinyin = "wu", Chinese = "屋", Example = "房屋" },
        new PinyinItem { Pinyin = "yu", Chinese = "鱼", Example = "小鱼" },
        new PinyinItem { Pinyin = "ye", Chinese = "椰", Example = "椰子" },
        new PinyinItem { Pinyin = "yue", Chinese = "月", Example = "月亮" },
        new PinyinItem { Pinyin = "yuan", Chinese = "元", Example = "一元" },
        new PinyinItem { Pinyin = "yin", Chinese = "音", Example = "音乐" },
        new PinyinItem { Pinyin = "yun", Chinese = "云", Example = "云彩" },
        new PinyinItem { Pinyin = "ying", Chinese = "鹰", Example = "老鹰" }
    };

    public ChinesePinyinAlphabetPage()
    {
        InitializeComponent();

        // 初始化 QwenTTSService
        _qwenTTS = new QwenTTSService();

        // 创建按钮
        CreatePinyinButtons();
    }

    private void CreatePinyinButtons()
    {
        // 声母
        foreach (var item in _initials)
        {
            InitialsFlex.Children.Add(CreatePinyinButton(item));
        }

        // 单韵母
        foreach (var item in _singleVowels)
        {
            SingleVowelsFlex.Children.Add(CreatePinyinButton(item));
        }

        // 复韵母
        foreach (var item in _compoundVowels)
        {
            CompoundVowelsFlex.Children.Add(CreatePinyinButton(item));
        }

        // 整体认读音节
        foreach (var item in _wholeSyllables)
        {
            WholeSyllablesFlex.Children.Add(CreatePinyinButton(item));
        }
    }

    private View CreatePinyinButton(PinyinItem item)
    {
        // 按钮内容布局
        var stackLayout = new VerticalStackLayout
        {
            Spacing = 2,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        stackLayout.Children.Add(new Label
        {
            Text = item.Pinyin,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#FF6B6B"),
            HorizontalOptions = LayoutOptions.Center
        });

        stackLayout.Children.Add(new Label
        {
            Text = item.Chinese,
            FontSize = 14,
            TextColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center
        });

        // 使用 Frame 包裹内容并添加点击事件
        var frame = new Frame
        {
            Content = stackLayout,
            BackgroundColor = Colors.White,
            BorderColor = Color.FromArgb("#FF6B6B"),
            CornerRadius = 15,
            Padding = new Thickness(10, 8),
            Margin = new Thickness(4),
            WidthRequest = 75,
            HeightRequest = 65,
            HasShadow = false
        };

        // 添加点击手势
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) => await SpeakPinyin(item);
        frame.GestureRecognizers.Add(tapGesture);

        return frame;
    }

    private async Task SpeakPinyin(PinyinItem item)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            // 优先朗读汉字（发音更准确）
            string speakText = item.Chinese;
            System.Diagnostics.Debug.WriteLine($"朗读: {item.Pinyin} -> {speakText}");

            // 使用阿里云 TTS 朗读汉字
            var audioStream = await _qwenTTS.SpeakAsync(speakText, "CHERRY", "Chinese");

            // 播放音频
            var audioManager = AudioManager.Current;
            var player = audioManager.CreatePlayer(audioStream);
            player.Play();

            // 等待播放完成
            while (player.CurrentPosition < player.Duration)
            {
                await Task.Delay(50);
            }

            player.Dispose();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"阿里 TTS 失败: {ex.Message}");

            // 降级方案：使用系统 TTS
            try
            {
                await TextToSpeech.Default.SpeakAsync(item.Chinese, new SpeechOptions
                {
                    Volume = 1.0f,
                    Pitch = 1.0f
                });
            }
            catch (Exception ttsEx)
            {
                await DisplayAlert("提示", $"无法发音: {ttsEx.Message}", "确定");
            }
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async Task SpeakMultiplePinyin(List<PinyinItem> itemList)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            foreach (var item in itemList)
            {
                var audioStream = await _qwenTTS.SpeakAsync(item.Chinese, "CHERRY", "Chinese");
                var audioManager = AudioManager.Current;
                var player = audioManager.CreatePlayer(audioStream);
                player.Play();

                while (player.CurrentPosition < player.Duration)
                {
                    await Task.Delay(50);
                }
                player.Dispose();

                await Task.Delay(150);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"批量发音失败: {ex.Message}");

            // 降级方案
            try
            {
                foreach (var item in itemList)
                {
                    await TextToSpeech.Default.SpeakAsync(item.Chinese);
                    await Task.Delay(200);
                }
            }
            catch (Exception innerEx)
            {
                await DisplayAlert("提示", $"发音失败: {innerEx.Message}", "确定");
            }
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnReadAllClicked(object sender, EventArgs e)
    {
        var allItems = new List<PinyinItem>();
        allItems.AddRange(_initials);
        allItems.AddRange(_singleVowels);
        allItems.AddRange(_compoundVowels);
        allItems.AddRange(_wholeSyllables);

        await SpeakMultiplePinyin(allItems);
    }

    private async void OnReadInitialsClicked(object sender, EventArgs e)
    {
        await SpeakMultiplePinyin(_initials);
    }

    private async void OnReadFinalsClicked(object sender, EventArgs e)
    {
        var finals = new List<PinyinItem>();
        finals.AddRange(_singleVowels);
        finals.AddRange(_compoundVowels);
        finals.AddRange(_wholeSyllables);

        await SpeakMultiplePinyin(finals);
    }

    // 批量下载所有拼音读音
    private async Task BatchDownloadAllPinyinAsync()
    {
        var allItems = new List<PinyinItem>();
        allItems.AddRange(_initials);
        allItems.AddRange(_singleVowels);
        allItems.AddRange(_compoundVowels);
        allItems.AddRange(_wholeSyllables);

        // 创建下载文件夹
        string downloadFolder = Path.Combine(FileSystem.AppDataDirectory, "PinyinAudios");
        if (!Directory.Exists(downloadFolder))
        {
            Directory.CreateDirectory(downloadFolder);
        }

        int successCount = 0;
        int failCount = 0;

        foreach (var item in allItems)
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                //StatusLabel.Text = $"正在合成: {item.Pinyin} ({item.Chinese})";

                var audioStream = await _qwenTTS.SpeakAsync(item.Chinese, "CHERRY", "Chinese");

                // 生成安全文件名
                string safeFileName = $"{item.Pinyin}_{item.Chinese}.wav";
                string filePath = Path.Combine(downloadFolder, safeFileName);

                using (var fileStream = File.Create(filePath))
                {
                    await audioStream.CopyToAsync(fileStream);
                }

                successCount++;
                System.Diagnostics.Debug.WriteLine($"下载成功: {filePath}");
            }
            catch (Exception ex)
            {
                failCount++;
                System.Diagnostics.Debug.WriteLine($"下载失败 {item.Pinyin}: {ex.Message}");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }

            await Task.Delay(200);
        }

        await DisplayAlert("下载完成",
            $"✅ 成功: {successCount} 个\n❌ 失败: {failCount} 个\n📁 保存在: {downloadFolder}",
            "确定");
    }

    private async void OnBatchDownloadClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("确认下载",
            "将下载所有拼音的读音音频，\n共 " + (_initials.Count + _singleVowels.Count + _compoundVowels.Count + _wholeSyllables.Count) + " 个文件，\n确定要继续吗？",
            "确定", "取消");

        if (confirm)
        {
            await BatchDownloadAllPinyinAsync();
        }
    }
}

// 拼音数据模型
public class PinyinItem
{
    public string Pinyin { get; set; } = "";
    public string Chinese { get; set; } = "";
    public string Example { get; set; } = "";
}