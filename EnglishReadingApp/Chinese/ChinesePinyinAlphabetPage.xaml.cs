using Plugin.Maui.Audio;

namespace EnglishReadingApp;

public partial class ChinesePinyinAlphabetPage : ContentPage
{
    private List<string> _initials = new()
    {
        "b", "p", "m", "f", "d", "t", "n", "l", "g", "k", "h", "j",
        "q", "x", "zh", "ch", "sh", "r", "z", "c", "s", "y", "w"
    };

    private List<string> _singleVowels = new()
    {
        "a", "o", "e", "i", "u", "ü"
    };

    private List<string> _compoundVowels = new()
    {
        "ai", "ei", "ui", "ao", "ou", "iu", "ie", "üe"
    };

    private List<string> _frontNasal = new()
    {
        "an", "en", "in", "un", "ün"
    };

    private List<string> _backNasal = new()
    {
        "ang", "eng", "ing", "ong"
    };

    private List<string> _wholeSyllables = new()
    {
        "zhi", "chi", "shi", "ri", "zi", "ci", "si", "yi",
        "wu", "yu", "ye", "yue", "yuan", "yin", "yun", "ying"
    };

    public ChinesePinyinAlphabetPage()
    {
        InitializeComponent();
        CreatePinyinButtons();
    }

    private void CreatePinyinButtons()
    {
        // 声母
        foreach (var pinyin in _initials)
        {
            InitialsFlex.Children.Add(CreatePinyinButton(pinyin));
        }

        // 单韵母
        foreach (var pinyin in _singleVowels)
        {
            SingleVowelsFlex.Children.Add(CreatePinyinButton(pinyin));
        }

        // 复韵母
        foreach (var pinyin in _compoundVowels)
        {
            CompoundVowelsFlex.Children.Add(CreatePinyinButton(pinyin));
        }

        // 前鼻韵母
        foreach (var pinyin in _frontNasal)
        {
            FrontNasalFlex.Children.Add(CreatePinyinButton(pinyin));
        }

        // 后鼻韵母
        foreach (var pinyin in _backNasal)
        {
            BackNasalFlex.Children.Add(CreatePinyinButton(pinyin));
        }

        // 整体认读音节
        foreach (var pinyin in _wholeSyllables)
        {
            WholeSyllablesFlex.Children.Add(CreatePinyinButton(pinyin));
        }
    }

    private Button CreatePinyinButton(string pinyin)
    {
        var button = new Button
        {
            Text = pinyin,
            BackgroundColor = Colors.White,  // 改为白色背景
            TextColor = Colors.Black,         // 改为黑色文字
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 20,
            WidthRequest = 65,
            HeightRequest = 40,
            Margin = new Thickness(4),
            BorderWidth = 1,                  // 添加边框
            BorderColor = Color.FromArgb("#FF6B6B")  // 边框颜色保持原来的红色调
        };

        button.Clicked += async (s, e) => await SpeakPinyin(pinyin);
        return button;
    }

    private async Task SpeakPinyin(string pinyin)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            await TextToSpeech.Default.SpeakAsync(pinyin, new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("提示", $"无法发音: {ex.Message}", "确定");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async Task SpeakMultiplePinyin(List<string> pinyinList)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            foreach (var pinyin in pinyinList)
            {
                await TextToSpeech.Default.SpeakAsync(pinyin);
                await Task.Delay(300);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("提示", $"发音失败: {ex.Message}", "确定");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnReadAllClicked(object sender, EventArgs e)
    {
        var allPinyin = new List<string>();
        allPinyin.AddRange(_initials);
        allPinyin.AddRange(_singleVowels);
        allPinyin.AddRange(_compoundVowels);
        allPinyin.AddRange(_frontNasal);
        allPinyin.AddRange(_backNasal);
        allPinyin.AddRange(_wholeSyllables);

        await SpeakMultiplePinyin(allPinyin);
    }

    private async void OnReadInitialsClicked(object sender, EventArgs e)
    {
        await SpeakMultiplePinyin(_initials);
    }

    private async void OnReadFinalsClicked(object sender, EventArgs e)
    {
        var finals = new List<string>();
        finals.AddRange(_singleVowels);
        finals.AddRange(_compoundVowels);
        finals.AddRange(_frontNasal);
        finals.AddRange(_backNasal);

        await SpeakMultiplePinyin(finals);
    }
}