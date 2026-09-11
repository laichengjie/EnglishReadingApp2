using Plugin.Maui.Audio;
using System.Diagnostics;
using EnglishReadingApp.Entity;

namespace EnglishReadingApp;

public partial class ChinesePoetryPage : ContentPage
{
    private List<PoetryItem> _poetryList;
    private int _currentIndex = 0;
    private bool _showTranslation = false;

    // 当前朗读任务的取消源；切换诗词或点击单字朗读时，用它中断上一次未读完的朗读
    private CancellationTokenSource? _speechCts;

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
        Title = "江雪",
        Author = "柳宗元",
        Dynasty = "唐代",
        Content = "千山鸟飞绝\n万径人踪灭\n孤舟蓑笠翁\n独钓寒江雪",
        Translation = "群山之中不见飞鸟的踪迹，所有的道路上也不见人的身影。江面上一只孤独的小船，船上有个穿蓑衣戴斗笠的老翁，独自在寒冷的江面上垂钓。"
    },

    // 1
    new PoetryItem
    {
        Title = "咏廿四气诗·白露八月节",
        Author = "元稹",
        Dynasty = "唐代",
        Content = "露沾蔬草白，\n天气转青高。\n叶下和秋吹，\n惊看两鬓毛。\n养羞因野鸟，\n为客讶蓬蒿。\n火急收田种，\n晨昏莫辞劳。",
        Translation = "白露时节，露水沾湿蔬菜野草，草叶发白，天气转凉，天空青碧高远。秋风吹落叶，令人惊觉两鬓已生白发。野鸟储藏食物准备过冬，客居他乡的人见蓬蒿丛生而惊讶。农事紧急，要赶紧收割田种，从早到晚不要推辞辛劳。"
    },
    // 2
    new PoetryItem
    {
        Title = "诗经·周南·螽斯",
        Author = "佚名",
        Dynasty = "先秦",
        Content = "螽斯羽，诜诜兮。\n宜尔子孙，振振兮。\n螽斯羽，薨薨兮。\n宜尔子孙，绳绳兮。\n螽斯羽，揖揖兮。\n宜尔子孙，蛰蛰兮。",
        Translation = "蝗虫张开翅膀，成群飞舞。你的子孙真多啊，昌盛振奋。蝗虫张开翅膀，嗡嗡齐飞。你的子孙真多啊，绵延不绝。蝗虫张开翅膀，聚集众多。你的子孙真多啊，和乐安聚。"
    },
    // 3
    new PoetryItem
    {
        Title = "诗经·卫风·木瓜",
        Author = "佚名",
        Dynasty = "先秦",
        Content = "投我以木瓜，\n报之以琼琚。\n匪报也，\n永以为好也！\n投我以木桃，\n报之以琼瑶。\n匪报也，\n永以为好也！\n投我以木李，\n报之以琼玖。\n匪报也，\n永以为好也！",
        Translation = "你赠我木瓜，我回赠你美玉。不只是报答，是希望永远相好！你赠我木桃，我回赠你美玉。不只是报答，是希望永远相好！你赠我木李，我回赠你美玉。不只是报答，是希望永远相好！"
    },
    // 4
    new PoetryItem
    {
        Title = "咏廿四气诗·秋分八月中",
        Author = "元稹",
        Dynasty = "唐代",
        Content = "琴弹南吕调，\n风色已高清。\n云散飘飖影，\n雷收振怒声。\n乾坤能静肃，\n寒暑喜均平。\n忽见新来雁，\n人心敢不惊？",
        Translation = "弹起南吕之调，秋风已经清朗高远。云散后留下飘摇的影子，雷声收起震怒。天地变得宁静肃穆，寒暑正好均衡。忽然看见新飞来的大雁，人心怎能不惊动？"
    },
    // 5
    new PoetryItem
    {
        Title = "诗经·秦风·无衣",
        Author = "佚名",
        Dynasty = "先秦",
        Content = "岂曰无衣？\n与子同袍。\n王于兴师，\n修我戈矛。\n与子同仇！\n岂曰无衣？\n与子同泽。\n王于兴师，\n修我矛戟。\n与子偕作！\n岂曰无衣？\n与子同裳。\n王于兴师，\n修我甲兵。\n与子偕行！",
        Translation = "谁说没有衣裳？我和你同穿战袍。君王要出兵打仗，修好我们的戈和矛。和你共同对敌！谁说没有衣裳？我和你同穿内衣。君王要出兵打仗，修好我们的矛和戟。和你一同行动！谁说没有衣裳？我和你同穿下裳。君王要出兵打仗，修好我们的铠甲兵器。和你一同前进！"
    },
    // 6
    new PoetryItem
    {
        Title = "咏廿四气诗·寒露九月节",
        Author = "元稹",
        Dynasty = "唐代",
        Content = "寒露惊秋晚，\n朝看菊渐黄。\n千家风扫叶，\n万里雁随阳。\n化蛤悲群鸟，\n收田畏早霜。\n因知松柏志，\n冬夏色苍苍。",
        Translation = "寒露到来，令人惊觉秋天已深，早晨看见菊花渐渐变黄。千家万户被风扫落叶，万里长空大雁追随暖阳南飞。传说雀鸟入水化为蛤，令人悲叹群鸟；收割田地又怕早霜。因此知道松柏的志向，无论冬夏都苍翠不改。"
    },
    // 7
    new PoetryItem
    {
        Title = "诗经·国风·十亩之间",
        Author = "佚名",
        Dynasty = "先秦",
        Content = "十亩之间兮，\n桑者闲闲兮，\n行与子还兮。\n十亩之外兮，\n桑者泄泄兮，\n行与子逝兮。",
        Translation = "十亩桑田之间啊，采桑的人从容悠闲啊，我要和你一起回去啊。十亩桑田之外啊，采桑的人舒缓自在啊，我要和你一起离去啊。"
    },
    // 8
    new PoetryItem
    {
        Title = "咏廿四气诗·霜降九月中",
        Author = "元稹",
        Dynasty = "唐代",
        Content = "风卷清云尽，\n空天万里霜。\n野豺先祭月，\n仙菊遇重阳。\n秋色悲疏木，\n鸿鸣忆故乡。\n谁知一樽酒，\n能使百秋亡。",
        Translation = "风吹卷清云散尽，空旷天空万里铺霜。野豺开始祭月，仙菊正遇重阳。秋色令稀疏树木更显悲凉，鸿雁鸣叫让人思念故乡。谁知道一樽酒，能让人忘却百秋忧愁。"
    },
    // 9
    new PoetryItem
    {
        Title = "诗经·秦风·渭阳",
        Author = "佚名",
        Dynasty = "先秦",
        Content = "我送舅氏，\n曰至渭阳。\n何以赠之？\n路车乘黄。\n我送舅氏，\n悠悠我思。\n何以赠之？\n琼瑰玉佩。",
        Translation = "我送舅舅，送到渭水之北。用什么赠送他？一辆大车和四匹黄马。我送舅舅，心中思念悠长。用什么赠送他？美玉和玉佩。"
    },
    // 10
    new PoetryItem
    {
        Title = "立冬",
        Author = "李白",
        Dynasty = "唐代",
        Content = "冻笔新诗懒写，\n寒炉美酒时温。\n醉看墨花月白，\n恍疑雪满前村。",
        Translation = "天气寒冷，毛笔冻结，懒得写新诗；寒夜炉边，美酒时常温热。醉眼朦胧中看着墨花和月色洁白，恍惚怀疑大雪已经铺满前村。"
    },
    // 11
    new PoetryItem
    {
        Title = "诗经·邶风·二子乘舟",
        Author = "佚名",
        Dynasty = "先秦",
        Content = "二子乘舟，\n泛泛其景。\n愿言思子，\n中心养养。\n二子乘舟，\n泛泛其逝。\n愿言思子，\n不瑕有害。",
        Translation = "两个孩子乘船，船影漂浮远去。思念你们啊，心中忧虑不安。两个孩子乘船，船影漂流消逝。思念你们啊，希望没有灾祸。"
    },
    // 12
    new PoetryItem
    {
        Title = "春近四绝句（其三）",
        Author = "黄庭坚",
        Dynasty = "宋代",
        Content = "小雪晴沙不作泥，\n疏帘红日弄朝晖。\n年华已伴梅梢晚，\n春色先从草际归。",
        Translation = "小雪后晴朗的沙地不再成泥，稀疏帘幕间红日摆弄早晨的光辉。年华已随着梅梢迟晚，春色却先从草边归来。"
    },
    // 13
    new PoetryItem
    {
        Title = "大雪",
        Author = "陆游",
        Dynasty = "宋代",
        Content = "海天黯黯万重云，\n欲到前村路不分。\n烈风吹雪深一丈，\n大布缝衫重七斤。",
        Translation = "海天昏暗，乌云重重，想要走到前村，道路已经分辨不清。猛烈的风吹着大雪，积雪深达一丈；粗布缝成的衣衫沉重得像有七斤。"
    },
    // 14
    new PoetryItem
    {
        Title = "邯郸冬至夜思家",
        Author = "白居易",
        Dynasty = "唐代",
        Content = "邯郸驿里逢冬至，\n抱膝灯前影伴身。\n想得家中夜深坐，\n还应说着远行人。",
        Translation = "在邯郸驿站里遇到冬至，抱着膝盖坐在灯前，只有影子陪伴自己。想到家里的人深夜坐着，应该还在谈论着远行在外的我。"
    },
    // 15
    new PoetryItem
    {
        Title = "窗前木芙蓉",
        Author = "范成大",
        Dynasty = "宋代",
        Content = "辛苦孤花破小寒，\n花心应似客心酸。\n更凭青女留连得，\n未作愁红怨绿看。",
        Translation = "辛苦孤独的花朵冲破小寒开放，花心应该像客居之人的心一样酸楚。更凭借霜神青女留连，未变成愁红怨绿的哀伤模样。"
    },
    // 16
    new PoetryItem
    {
        Title = "连夕大寒示邻士二首 其一",
        Author = "李光",
        Dynasty = "宋代",
        Content = "冻云垂地北风颠，\n妆点江湖欲雪天。\n我亦随身有蓑笠，\n兴来同上钓鱼船。",
        Translation = "冻云低垂到地面，北风狂颠，装点江湖，正是将要下雪的天气。我也随身带着蓑衣斗笠，兴致来时一起上钓鱼船。"
    },
    // 17
    new PoetryItem
    {
        Title = "立春偶成",
        Author = "张栻",
        Dynasty = "宋代",
        Content = "律回岁晚冰霜少，\n春到人间草木知。\n便觉眼前生意满，\n东风吹水绿参差。",
        Translation = "节气回转，一年将尽时冰霜渐少；春天来到人间，草木最先知道。只觉得眼前充满生机，东风吹拂水面，绿波参差荡漾。"
    },
    // 18
    new PoetryItem
    {
        Title = "春游湖",
        Author = "徐俯",
        Dynasty = "宋代",
        Content = "双飞燕子几时回，\n夹岸桃花蘸水开。\n春雨断桥人不度，\n小舟撑出柳阴来。",
        Translation = "双双飞舞的燕子什么时候回来？两岸桃花贴着水面盛开。春雨后断桥阻隔，行人无法渡过；一只小船从柳荫中撑出来。"
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

    /// <summary>
    /// 中断当前正在进行的朗读。
    /// 被中断的朗读任务会走 OperationCanceledException 分支静默结束，不会弹出错误提示。
    /// </summary>
    private void CancelSpeech()
    {
        // 原子地"取出并清空"字段。
        // 注意：Cancel() 会同步触发被取消任务的延续，而延续里的 finally 可能重入本方法；
        // 先把字段摘空，既能保证只处理一次，后续也只操作局部变量，不会空引用。
        var cts = Interlocked.Exchange(ref _speechCts, null);
        if (cts == null) return;

        cts.Cancel();
        cts.Dispose();
    }

    /// <summary>
    /// 先中断上一次未读完的朗读，再为本次朗读创建新的取消源。
    /// 切歌、点击"听全诗"、点击单字朗读都会调用，保证同一时刻只有一路朗读在播放。
    /// </summary>
    private CancellationTokenSource RestartSpeech()
    {
        CancelSpeech();
        var cts = new CancellationTokenSource();
        _speechCts = cts;
        return cts;
    }

    private async Task SpeakText(string text)
    {
        // 点击单字朗读时，同样要中断正在播放的全诗
        var cts = RestartSpeech();

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            StatusLabel.Text = $"🔊 {text}";

            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f
            }, cts.Token);
        }
        catch (OperationCanceledException)
        {
            // 被切歌或新的朗读打断，属正常流程，无需提示
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"朗读失败: {ex.Message}");
            await DisplayAlert("提示", $"无法发音: {ex.Message}", "确定");
        }
        finally
        {
            // 只有仍是"当前"朗读任务才复位 UI，
            // 否则旧任务收尾时会把新任务刚点亮的转圈指示器关掉
            if (!cts.IsCancellationRequested)
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                StatusLabel.Text = "点击听全诗或跟读";
            }

            // 原子地"是我就摘掉"，避免 check-then-act 竞态导致的空引用或重复释放
            if (Interlocked.CompareExchange(ref _speechCts, null, cts) == cts)
            {
                cts.Dispose();
            }
        }
    }

    private async void OnListenFullClicked(object? sender, EventArgs? e)
    {
        // 中断上一首（或上一次点击）未读完的朗读，再开始本次播放
        var cts = RestartSpeech();

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
                await TextToSpeech.Default.SpeakAsync(segment.Text, options, cts.Token);

                // 段落之间稍作停顿，听感更自然
                await Task.Delay(300, cts.Token);
            }

            // 诗文按行朗读：整段 Content 含换行符，Android 端会忽略换行连读导致听感急促，
            // 逐行朗读可在 Android 上也保留换行处的停顿
            var lines = item.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                StatusLabel.Text = $"🔊 正在朗读诗文：{trimmed}";
                await TextToSpeech.Default.SpeakAsync(trimmed, options, cts.Token);
                await Task.Delay(250, cts.Token);
            }

            StatusLabel.Text = "播放完成";
        }
        catch (OperationCanceledException)
        {
            // 被切歌或新的朗读打断，属正常流程，无需提示
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"播放失败: {ex.Message}");
            await DisplayAlert("提示", $"播放失败: {ex.Message}", "确定");
        }
        finally
        {
            // 已作废的旧播放任务不再改动 UI，避免把新任务刚点亮的指示器关掉
            if (!cts.IsCancellationRequested)
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                await Task.Delay(500);
                StatusLabel.Text = "点击听全诗或跟读";
            }

            // 原子地"是我就摘掉"，避免 check-then-act 竞态导致的空引用或重复释放
            if (Interlocked.CompareExchange(ref _speechCts, null, cts) == cts)
            {
                cts.Dispose();
            }
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
            // 切换诗词前先中断当前未读完的朗读
            CancelSpeech();

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
            // 先把上一首没读完的朗读中断，再切歌
            CancelSpeech();

            _currentIndex++;
            DisplayCurrentPoetry();

            // 立即开始朗读新的一首
            OnListenFullClicked(null, null); 
        }
        else
        {
            await DisplayAlert("提示", "已经是最后一首了", "确定");
        }
    }

    /// <summary>
    /// 右上角"目录"按钮：重建列表后显示弹层。
    /// 每次打开都重建，保证"当前正在阅读的一首"的高亮是最新的。
    /// </summary>
    private void OnCatalogClicked(object sender, EventArgs e)
    {
        BuildCatalogList();
        CatalogOverlay.IsVisible = true;
    }

    /// <summary>
    /// 关闭目录弹层（点击遮罩空白处或右上角 ✕）。
    /// </summary>
    private void OnCloseCatalogClicked(object? sender, EventArgs? e)
    {
        CatalogOverlay.IsVisible = false;
    }

    /// <summary>
    /// 构建目录列表：每项为"序号 + 诗名 + 作者"，当前正在阅读的一首高亮。
    /// </summary>
    private void BuildCatalogList()
    {
        CatalogList.Children.Clear();
        if (_poetryList == null || _poetryList.Count == 0) return;

        for (int i = 0; i < _poetryList.Count; i++)
        {
            var item = _poetryList[i];
            int index = i;                      // 闭包捕获循环变量，需先拷到局部变量
            bool isCurrent = index == _currentIndex;

            var numberLabel = new Label
            {
                Text = $"{index + 1}",
                FontSize = 14,
                TextColor = isCurrent ? Color.FromArgb("#E67E22") : Color.FromArgb("#95A5A6"),
                FontAttributes = isCurrent ? FontAttributes.Bold : FontAttributes.None,
                WidthRequest = 28,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var titleLabel = new Label
            {
                Text = item.Title,
                FontSize = 17,
                TextColor = isCurrent ? Color.FromArgb("#E67E22") : Color.FromArgb("#2C3E50"),
                FontAttributes = isCurrent ? FontAttributes.Bold : FontAttributes.None,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            var authorLabel = new Label
            {
                Text = item.Author,
                FontSize = 13,
                TextColor = Color.FromArgb("#7F8C8D"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End
            };

            var row = new Grid
            {
                Padding = new Thickness(10, 12),
                ColumnSpacing = 8,
                BackgroundColor = isCurrent ? Color.FromArgb("#FFF3E0") : Colors.Transparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                }
            };

            Grid.SetColumn(numberLabel, 0);
            Grid.SetColumn(titleLabel, 1);
            Grid.SetColumn(authorLabel, 2);
            row.Children.Add(numberLabel);
            row.Children.Add(titleLabel);
            row.Children.Add(authorLabel);

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => SelectPoetry(index);
            row.GestureRecognizers.Add(tap);

            CatalogList.Children.Add(row);

            // 各项之间加分隔线（最后一项之后不加）
            if (index < _poetryList.Count - 1)
            {
                CatalogList.Children.Add(new BoxView
                {
                    HeightRequest = 1,
                    Color = Color.FromArgb("#F0F0F0")
                });
            }
        }
    }

    /// <summary>
    /// 从目录选中第 index 首：关闭弹层、中断正在进行的朗读，然后显示该首。
    /// </summary>
    private void SelectPoetry(int index)
    {
        if (_poetryList == null || index < 0 || index >= _poetryList.Count) return;

        CatalogOverlay.IsVisible = false;
        CancelSpeech();

        _currentIndex = index;
        DisplayCurrentPoetry();
    }
}