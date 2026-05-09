using Plugin.Maui.Audio;
using System.Diagnostics;

namespace EnglishReadingApp
{
    public partial class MainPage : ContentPage
    {
        private int _currentQuoteIndex = 0;
        private List<(string quote, string author, string tip)> _quotes;
        private Timer? _timer;

        public MainPage()
        {
            InitializeComponent();

            // 初始化励志语录库
            _quotes = new List<(string, string, string)>
        {
            ("知识改变命运，学习成就未来", "—— 名言警句", "🌟 每天学习，不断成长"),
            ("学而不思则罔，思而不学则殆", "—— 孔子", "📖 学习+思考=进步"),
            ("活到老，学到老", "—— 谚语", "💪 终身学习，终身受益"),
            ("宝剑锋从磨砺出，梅花香自苦寒来", "—— 警世贤文", "🌸 坚持就会有收获"),
            ("温故而知新，可以为师矣", "—— 孔子", "🔄 复习也是学习的一部分"),
            ("书山有路勤为径，学海无涯苦作舟", "—— 韩愈", "⛵ 勤奋是成功的阶梯"),
            ("三人行，必有我师焉", "—— 孔子", "👥 向身边的人学习"),
            ("只要功夫深，铁杵磨成针", "—— 谚语", "✍️ 坚持不懈定能成功"),
            ("博观而约取，厚积而薄发", "—— 苏轼", "📚 积累是成功的基础"),
            ("不积跬步，无以至千里", "—— 荀子", "👣 从小事做起，一步一个脚印"),
            ("天才是百分之一的灵感加百分之九十九的汗水", "—— 爱迪生", "💧 努力比天赋更重要"),
            ("读万卷书，行万里路", "—— 刘彝", "🌍 理论与实践相结合")
        };

            // 显示第一条语录
            UpdateQuote(0);

            // 生成指示点
            GenerateDots();

            // 启动定时器，每5秒切换一条语录
            _timer = new Timer(TimerCallback, null, 3000, 3000);
        }

        private void GenerateDots()
        {
            DotContainer.Children.Clear();

            for (int i = 0; i < _quotes.Count; i++)
            {
                var dot = new ContentView();

                if (i == _currentQuoteIndex)
                {
                    // 当前选中 - 大圆点
                    dot.Content = new Label
                    {
                        Text = "●",
                        FontSize = 10,
                        TextColor = Colors.White
                    };
                }
                else
                {
                    // 未选中 - 小圆点
                    dot.Content = new Label
                    {
                        Text = "○",
                        FontSize = 8,
                        TextColor = Colors.White.WithAlpha(0.5f)
                    };
                }

                DotContainer.Children.Add(dot);
            }
        }

        private void TimerCallback(object? state)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _currentQuoteIndex = (_currentQuoteIndex + 1) % _quotes.Count;
                UpdateQuote(_currentQuoteIndex);
                UpdateDots();
            });
        }

        private void UpdateQuote(int index)
        {
            var item = _quotes[index];
            QuoteLabel.Text = item.quote;
            AuthorLabel.Text = item.author;
            DailyTipLabel.Text = item.tip;
        }

        private void UpdateDots()
        {
            for (int i = 0; i < DotContainer.Children.Count && i < _quotes.Count; i++)
            {
                var dot = DotContainer.Children[i] as ContentView;
                if (dot != null)
                {
                    if (i == _currentQuoteIndex)
                    {
                        dot.Content = new Label
                        {
                            Text = "●",
                            FontSize = 10,
                            TextColor = Colors.White
                        };
                    }
                    else
                    {
                        dot.Content = new Label
                        {
                            Text = "○",
                            FontSize = 8,
                            TextColor = Colors.White.WithAlpha(0.5f)
                        };
                    }
                }
            }
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            // 停止定时器
            _timer?.Dispose();

            // 点击动画
            var button = sender as Button;
            await button.ScaleTo(0.95, 100);
            await button.ScaleTo(1, 100);

            // 淡出效果
            await this.FadeTo(0, 250);

            // 跳转到登录页
            await Navigation.PushAsync(new SubjectPage());

            // 移除当前页面
            Navigation.RemovePage(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer?.Dispose();
        }
    }
}