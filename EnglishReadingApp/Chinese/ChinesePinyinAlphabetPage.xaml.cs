using Plugin.Maui.Audio;
using System.Diagnostics;

namespace EnglishReadingApp;

public partial class ChinesePinyinAlphabetPage : ContentPage
{
    private readonly QwenTTSService _qwenTTS;
    private Dictionary<PinyinItem, View> _buttonMap = new();
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isReading = false;

    // 一、声母表（23个）
    private List<PinyinItem> _initials = new()
    {
        new PinyinItem { Pinyin = "b", Chinese = "玻", Example = "玻璃" },
        new PinyinItem { Pinyin = "p", Chinese = "坡", Example = "山坡" },
        new PinyinItem { Pinyin = "m", Chinese = "摸", Example = "抚摸" },
        new PinyinItem { Pinyin = "f", Chinese = "佛", Example = "佛像" },  // 多音 fó/fú：无单音同音字可替，保持原字
        // 呼读音 dé。"德"为单音字恒读 dé；原"得"有 dé/de/děi 三读，易被 TTS 读错
        new PinyinItem { Pinyin = "d", Chinese = "德", Example = "得到" },
        new PinyinItem { Pinyin = "t", Chinese = "特", Example = "特别" },
        new PinyinItem { Pinyin = "n", Chinese = "讷", Example = "讷讷" },
        new PinyinItem { Pinyin = "l", Chinese = "勒", Example = "勒紧" },  // 多音 lè/lēi：无单音同音字可替，保持原字
        new PinyinItem { Pinyin = "g", Chinese = "哥", Example = "哥哥" },
        new PinyinItem { Pinyin = "k", Chinese = "科", Example = "科学" },
        new PinyinItem { Pinyin = "h", Chinese = "喝", Example = "喝水" },  // 多音 hē/hè：无单音同音字可替，保持原字
        new PinyinItem { Pinyin = "j", Chinese = "鸡", Example = "小鸡" },
        new PinyinItem { Pinyin = "q", Chinese = "欺", Example = "欺负" },
        new PinyinItem { Pinyin = "x", Chinese = "西", Example = "西瓜" },
        new PinyinItem { Pinyin = "zh", Chinese = "知", Example = "知道" },
        new PinyinItem { Pinyin = "ch", Chinese = "吃", Example = "吃饭" },
        new PinyinItem { Pinyin = "sh", Chinese = "诗", Example = "诗歌" },
        new PinyinItem { Pinyin = "r", Chinese = "日", Example = "日子" },
        new PinyinItem { Pinyin = "z", Chinese = "资", Example = "资本" },
        // 标准呼读音为 cī（一声），非 cì。"雌"与 z(资 zī)、s(思 sī) 一致
        new PinyinItem { Pinyin = "c", Chinese = "雌", Example = "刺猬" },
        new PinyinItem { Pinyin = "s", Chinese = "思", Example = "思考" },
        new PinyinItem { Pinyin = "y", Chinese = "衣", Example = "衣服" },
        new PinyinItem { Pinyin = "w", Chinese = "乌", Example = "乌鸦" }
    };

    // 二、单韵母（6个）
    private List<PinyinItem> _singleVowels = new()
    {
        new PinyinItem { Pinyin = "a", Chinese = "啊", Example = "啊呀" },
        // "噢"为单音字恒读 ō；原"喔"有 ō/wō 两读，易被 TTS 误读为 wō
        new PinyinItem { Pinyin = "o", Chinese = "噢", Example = "喔喔" },
        new PinyinItem { Pinyin = "e", Chinese = "鹅", Example = "天鹅" },
        new PinyinItem { Pinyin = "i", Chinese = "衣", Example = "衣服" },
        new PinyinItem { Pinyin = "u", Chinese = "乌", Example = "乌云" },
        new PinyinItem { Pinyin = "ü", Chinese = "鱼", Example = "小鱼" }
    };

    // 三、复韵母（18个）
    private List<PinyinItem> _compoundVowels = new()
    {
        new PinyinItem { Pinyin = "ai", Chinese = "哀", Example = "悲哀" },
        new PinyinItem { Pinyin = "ei", Chinese = "诶", Example = "诶呀" },  // 多音 ēi/éi/ěi/èi：无单音同音字可替，保持原字
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
        // 整体认读 ci 读 cī（一声），非 cì。用"疵"与声母 c 的"雌"区分
        new PinyinItem { Pinyin = "ci", Chinese = "疵", Example = "次数" },
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
        _qwenTTS = new QwenTTSService();
        CreatePinyinButtons();

        // 页面消失时取消朗读
        this.Disappearing += (s, e) => StopReading();
    }

    private void CreatePinyinButtons()
    {
        foreach (var item in _initials)
        {
            var button = CreatePinyinButton(item);
            InitialsFlex.Children.Add(button);
            _buttonMap[item] = button;
        }

        foreach (var item in _singleVowels)
        {
            var button = CreatePinyinButton(item);
            SingleVowelsFlex.Children.Add(button);
            _buttonMap[item] = button;
        }

        foreach (var item in _compoundVowels)
        {
            var button = CreatePinyinButton(item);
            CompoundVowelsFlex.Children.Add(button);
            _buttonMap[item] = button;
        }

        foreach (var item in _wholeSyllables)
        {
            var button = CreatePinyinButton(item);
            WholeSyllablesFlex.Children.Add(button);
            _buttonMap[item] = button;
        }
    }

    private View CreatePinyinButton(PinyinItem item)
    {
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

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) => await SpeakPinyin(item);
        frame.GestureRecognizers.Add(tapGesture);

        return frame;
    }

    private void HighlightButton(PinyinItem item)
    {
        if (_buttonMap.TryGetValue(item, out var view) && view is Frame frame)
        {
            frame.BackgroundColor = Color.FromArgb("#FFD93D");
        }
    }

    private void UnhighlightAllButtons()
    {
        foreach (var kvp in _buttonMap)
        {
            if (kvp.Value is Frame frame)
            {
                frame.BackgroundColor = Colors.White;
            }
        }
    }

    private void StopReading()
    {
        _isReading = false;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        UnhighlightAllButtons();
        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;
    }

    private async Task SpeakPinyin(PinyinItem item)
    {
        try
        {
            StopReading();
            await Task.Delay(100);

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            UnhighlightAllButtons();
            HighlightButton(item);

            await TextToSpeech.Default.SpeakAsync(item.Chinese, new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"朗读失败: {ex.Message}");
        }
        finally
        {
            await Task.Delay(300);
            UnhighlightAllButtons();
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void StartReading(List<PinyinItem> itemList)
    {
        if (_isReading)
        {
            StopReading();
            // 等待取消完成
            Task.Delay(100).Wait();
        }

        _isReading = true;
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        Task.Run(async () =>
        {
            try
            {
                foreach (var item in itemList)
                {
                    if (token.IsCancellationRequested || !_isReading)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            UnhighlightAllButtons();
                            LoadingIndicator.IsVisible = false;
                            LoadingIndicator.IsRunning = false;
                        });
                        break;
                    }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UnhighlightAllButtons();
                        HighlightButton(item);
                        LoadingIndicator.IsVisible = true;
                        LoadingIndicator.IsRunning = true;
                        StatusLabel.Text = $"🔊 {item.Pinyin} ({item.Chinese})";
                        StatusLabel.IsVisible = true;
                    });

                    // 检查取消
                    if (token.IsCancellationRequested) break;

                    await TextToSpeech.Default.SpeakAsync(item.Chinese, new SpeechOptions
                    {
                        Volume = 1.0f,
                        Pitch = 1.0f
                    });

                    await Task.Delay(150, token);
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_isReading)
                    {
                        UnhighlightAllButtons();
                        StatusLabel.Text = "✅ 朗读完成";
                        LoadingIndicator.IsVisible = false;
                        LoadingIndicator.IsRunning = false;
                        _isReading = false;
                    }
                });

                await Task.Delay(1000);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusLabel.IsVisible = false;
                });
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("朗读已取消");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UnhighlightAllButtons();
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    _isReading = false;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"朗读错误: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    _isReading = false;
                });
            }
        }, token);
    }

    private void OnReadAllClicked(object sender, EventArgs e)
    {
        var allItems = new List<PinyinItem>();
        allItems.AddRange(_initials);
        allItems.AddRange(_singleVowels);
        allItems.AddRange(_compoundVowels);
        allItems.AddRange(_wholeSyllables);
        StartReading(allItems);
    }

    private void OnReadInitialsClicked(object sender, EventArgs e)
    {
        StartReading(_initials);
    }

    private void OnReadFinalsClicked(object sender, EventArgs e)
    {
        var finals = new List<PinyinItem>();
        finals.AddRange(_singleVowels);
        finals.AddRange(_compoundVowels);
        finals.AddRange(_wholeSyllables);
        StartReading(finals);
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        StopReading();
        StatusLabel.Text = "⏹ 已停止";
        StatusLabel.IsVisible = true;
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StatusLabel.IsVisible = false;
            });
        });
    }
}

public class PinyinItem
{
    public string Pinyin { get; set; } = "";
    public string Chinese { get; set; } = "";
    public string Example { get; set; } = "";
}