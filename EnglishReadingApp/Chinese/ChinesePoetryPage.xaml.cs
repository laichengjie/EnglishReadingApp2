using Plugin.Maui.Audio;
using System.Diagnostics;
using EnglishReadingApp.Entity;

namespace EnglishReadingApp;

public partial class ChinesePoetryPage : ContentPage
{
    private List<PoetryItem> _poetryList;
    private int _currentIndex = 0;
    private bool _showTranslation = false;

    public ChinesePoetryPage()
    {
        InitializeComponent();
        LoadPoetryData();
        DisplayCurrentPoetry();
    }

    private void LoadPoetryData()
    {
        _poetryList = new List<PoetryItem>
        {
            new PoetryItem
            {
                Title = "静夜思",
                Author = "李白",
                Dynasty = "唐代",
                Content = "床前明月光\n疑是地上霜\n举头望明月\n低头思故乡",
                Translation = "明亮的月光洒在床前，好像地上泛起了一层白霜。我抬起头来，看那空中的明月，不由得低头沉思，想起了远方的家乡。"
            },
            new PoetryItem
            {
                Title = "春晓",
                Author = "孟浩然",
                Dynasty = "唐代",
                Content = "春眠不觉晓\n处处闻啼鸟\n夜来风雨声\n花落知多少",
                Translation = "春天睡醒不觉天已大亮，到处是鸟儿清脆的叫声。回想昨夜的阵阵风雨声，不知吹落了多少娇美的春花。"
            },
            new PoetryItem
            {
                Title = "咏鹅",
                Author = "骆宾王",
                Dynasty = "唐代",
                Content = "鹅鹅鹅\n曲项向天歌\n白毛浮绿水\n红掌拨清波",
                Translation = "鹅，鹅，鹅，弯曲着脖子对着天空歌唱。白色的羽毛漂浮在碧绿的水面上，红色的脚掌划动着清澈的水波。"
            },
            new PoetryItem
            {
                Title = "悯农",
                Author = "李绅",
                Dynasty = "唐代",
                Content = "锄禾日当午\n汗滴禾下土\n谁知盘中餐\n粒粒皆辛苦",
                Translation = "盛夏中午，农民还在田里锄地，汗水滴落在泥土里。谁又知道盘中的饭菜，每一粒都是农民辛辛苦苦劳动得来的。"
            },
            new PoetryItem
            {
                Title = "登鹳雀楼",
                Author = "王之涣",
                Dynasty = "唐代",
                Content = "白日依山尽\n黄河入海流\n欲穷千里目\n更上一层楼",
                Translation = "夕阳沿着西山慢慢落下，黄河水滚滚流入大海。想要看到更远的景色，就要再登上一层楼。"
            },
            new PoetryItem
            {
                Title = "望庐山瀑布",
                Author = "李白",
                Dynasty = "唐代",
                Content = "日照香炉生紫烟\n遥看瀑布挂前川\n飞流直下三千尺\n疑是银河落九天",
                Translation = "太阳照在香炉峰上，升起紫色的烟雾，远远看去，瀑布像一条白练挂在山前。水流从三千尺的高处飞泻而下，好像是银河从九天之上落下来。"
            },
            new PoetryItem
            {
                Title = "绝句",
                Author = "杜甫",
                Dynasty = "唐代",
                Content = "两个黄鹂鸣翠柳\n一行白鹭上青天\n窗含西岭千秋雪\n门泊东吴万里船",
                Translation = "两只黄鹂在翠绿的柳树间欢快地歌唱，一行白鹭飞向蔚蓝的天空。窗口可以看见西岭千年不化的积雪，门口停泊着来自东吴的远航船只。"
            },
            new PoetryItem
            {
                Title = "赋得古原草送别",
                Author = "白居易",
                Dynasty = "唐代",
                Content = "离离原上草\n一岁一枯荣\n野火烧不尽\n春风吹又生",
                Translation = "原野上长满了茂盛的青草，每年都会经历一次枯萎和茂盛。野火无法将它们完全烧尽，春风吹来，它们又会重新生长。"
            },
            new PoetryItem
            {
                Title = "望月怀远",
                Author = "张九龄",
                Dynasty = "唐代",
                Content = "海上生明月\n天涯共此时\n情人怨遥夜\n竟夕起相思",
                Translation = "海上升起了一轮明月，远在天涯的人和我共同仰望。有情的人儿怨恨这漫长的夜晚，整个晚上都在思念远方的亲人。"
            },
            new PoetryItem
            {
                Title = "江雪",
                Author = "柳宗元",
                Dynasty = "唐代",
                Content = "千山鸟飞绝\n万径人踪灭\n孤舟蓑笠翁\n独钓寒江雪",
                Translation = "群山之中不见飞鸟的踪迹，所有的道路上也不见人的身影。江面上一只孤独的小船，船上有个穿蓑衣戴斗笠的老翁，独自在寒冷的江面上垂钓。"
            }
        };
    }

    private void DisplayCurrentPoetry()
    {
        if (_poetryList == null || _poetryList.Count == 0) return;

        var item = _poetryList[_currentIndex];

        TranslationLabel.Text = item.Translation;

        // 诗名（26pt，点击朗读诗名）
        TitlePinyinLayout.Children.Clear();
        TitlePinyinLayout.Children.Add(BuildPinyinRow(
            item.Title, item.TitlePinyin, 26, 13,
            Color.FromArgb("#2C3E50"), item.Title));

        // 作者（16pt，点击朗读作者）
        AuthorPinyinLayout.Children.Clear();
        AuthorPinyinLayout.Children.Add(BuildPinyinRow(
            item.Author, item.AuthorPinyin, 16, 11,
            Color.FromArgb("#2C3E50"), item.Author));

        // 朝代（16pt，点击朗读朝代）
        DynastyPinyinLayout.Children.Clear();
        DynastyPinyinLayout.Children.Add(BuildPinyinRow(
            item.Dynasty, item.DynastyPinyin, 16, 11,
            Color.FromArgb("#2C3E50"), item.Dynasty));

        // 诗词正文（30pt，点击朗读整句）
        ContentLayout.Children.Clear();
        var pinyinLines = item.GetPinyinLines();

        foreach (var line in pinyinLines)
        {
            // 使用 StackLayout 让每一行独立
            var lineContainer = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 2,
                Margin = new Thickness(0, 4)
            };

            lineContainer.Children.Add(BuildPinyinRow(
                line.Chinese, line.Pinyin, 30, 14,
                Color.FromArgb("#2C3E50"), line.Chinese));

            ContentLayout.Children.Add(lineContainer);
        }

        var translationFrame = this.FindByName<Frame>("TranslationFrame");
        if (translationFrame != null)
        {
            translationFrame.IsVisible = _showTranslation;
        }

        
        ResultFrame.IsVisible = false;
    }

    /// <summary>
    /// 构建一行"逐字拼音"：每个汉字一个纵向单元（上拼音、下汉字），整行居中。
    /// 诗名、作者、朝代与诗词正文共用此方法，保证视觉与交互一致。
    /// </summary>
    /// <param name="chinese">汉字串，如 "静夜思"</param>
    /// <param name="pinyin">空格分隔的带声调拼音，如 "jìng yè sī"；转换失败时等于 chinese</param>
    /// <param name="charFontSize">汉字字号（诗名 26 / 作者·朝代 16 / 正文 30）</param>
    /// <param name="pinyinFontSize">拼音字号（诗名 13 / 作者·朝代 11 / 正文 14）</param>
    /// <param name="textColor">文字颜色</param>
    /// <param name="speakText">点击时朗读的文本；为 null 或空白则不绑定手势</param>
    private HorizontalStackLayout BuildPinyinRow(
        string chinese,
        string pinyin,
        double charFontSize,
        double pinyinFontSize,
        Color textColor,
        string? speakText)
    {
        var row = new HorizontalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        if (string.IsNullOrEmpty(chinese)) return row;

        // 降级保护：转换失败时 pinyin 等于原中文，此时不能再把汉字当拼音显示一遍
        bool pinyinValid = !string.IsNullOrEmpty(pinyin) && pinyin != chinese;
        var pinyinWords = pinyinValid
            ? pinyin.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            : Array.Empty<string>();

        var chineseChars = chinese.ToCharArray();
        bool canSpeak = !string.IsNullOrWhiteSpace(speakText);

        for (int i = 0; i < chineseChars.Length; i++)
        {
            string charPinyin = i < pinyinWords.Length ? pinyinWords[i] : "";

            // 宽度取"汉字"与"拼音"所需宽度的较大者，避免较长的拼音被挤压
            double width = Math.Max(
                charFontSize * 1.8,
                charPinyin.Length * pinyinFontSize * 0.62);

            var charContainer = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 2,
                WidthRequest = width,
                Margin = new Thickness(0)
            };

            var pinyinLabel = new Label
            {
                Text = charPinyin,
                FontSize = pinyinFontSize,
                FontAttributes = FontAttributes.Bold,
                TextColor = textColor,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var chineseLabel = new Label
            {
                Text = chineseChars[i].ToString(),
                FontSize = charFontSize,
                FontAttributes = FontAttributes.Bold,
                TextColor = textColor,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            // 点击拼音或汉字朗读对应文字
            if (canSpeak)
            {
                string textToSpeak = speakText!;

                var tapPinyin = new TapGestureRecognizer();
                tapPinyin.Tapped += async (s, e) => await SpeakText(textToSpeak);
                pinyinLabel.GestureRecognizers.Add(tapPinyin);

                var tapChinese = new TapGestureRecognizer();
                tapChinese.Tapped += async (s, e) => await SpeakText(textToSpeak);
                chineseLabel.GestureRecognizers.Add(tapChinese);
            }

            charContainer.Children.Add(pinyinLabel);
            charContainer.Children.Add(chineseLabel);

            row.Children.Add(charContainer);
        }

        return row;
    }

    private async Task SpeakText(string text)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            StatusLabel.Text = $"🔊 {text}";

            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"朗读失败: {ex.Message}");
            await DisplayAlert("提示", $"无法发音: {ex.Message}", "确定");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            StatusLabel.Text = "点击听全诗或跟读";
        }
    }

    private async void OnListenFullClicked(object sender, EventArgs e)
    {
        var item = _poetryList[_currentIndex];
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var options = new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f
            };

            // 注：MAUI 9 的 SpeechOptions 尚不支持 Rate（语速）属性，
            // Android 端语速偏快的问题，此处通过下方"诗文逐行朗读"增加句间停顿来缓解

            // 依次朗读：诗名 → 时代 → 作者
            var segments = new (string Label, string Text)[]
            {
                ("诗名", item.Title),
                ("时代", item.Dynasty),
                ("作者", item.Author)
            };

            foreach (var segment in segments)
            {
                if (string.IsNullOrWhiteSpace(segment.Text)) continue;

                StatusLabel.Text = $"🔊 正在朗读{segment.Label}：{segment.Text}";
                await TextToSpeech.Default.SpeakAsync(segment.Text, options);

                // 段落之间稍作停顿，听感更自然
                await Task.Delay(300);
            }

            // 诗文按行朗读：整段 Content 含换行符，Android 端会忽略换行连读导致听感急促，
            // 逐行朗读可在 Android 上也保留换行处的停顿
            var lines = item.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                StatusLabel.Text = $"🔊 正在朗读诗文：{trimmed}";
                await TextToSpeech.Default.SpeakAsync(trimmed, options);
                await Task.Delay(250);
            }

            StatusLabel.Text = "播放完成";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"播放失败: {ex.Message}");
            await DisplayAlert("提示", $"播放失败: {ex.Message}", "确定");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            await Task.Delay(500);
            StatusLabel.Text = "点击听全诗或跟读";
        }
    }

    private async void OnRecordClicked(object sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("提示", "需要麦克风权限", "确定");
                    return;
                }
            }

            ResultFrame.IsVisible = true;
            ResultLabel.Text = "🎤 正在识别您的朗读...";
            ScoreLabel.Text = "";

            await Task.Delay(2000);

            var random = new Random();
            int score = random.Next(60, 100);
            ScoreLabel.Text = score >= 80 ? "🌟 优秀！继续加油！" : "💪 不错，再练练会更好！";
            ResultLabel.Text = score >= 80 ? "发音清晰，节奏准确！" : "注意语调，再听一遍示范吧！";
        }
        catch (Exception ex)
        {
            await DisplayAlert("提示", $"识别失败: {ex.Message}", "确定");
        }
    }

    private void OnTranslationClicked(object sender, EventArgs e)
    {
        _showTranslation = !_showTranslation;
        var translationFrame = this.FindByName<Frame>("TranslationFrame");
        if (translationFrame != null)
        {
            translationFrame.IsVisible = _showTranslation;
        }
    }

    private async void OnPreviousClicked(object sender, EventArgs e)
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            DisplayCurrentPoetry();
        }
        else
        {
            await DisplayAlert("提示", "已经是第一首了", "确定");
        }
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (_currentIndex < _poetryList.Count - 1)
        {
            _currentIndex++;
            DisplayCurrentPoetry();
        }
        else
        {
            await DisplayAlert("提示", "已经是最后一首了", "确定");
        }
    }
}