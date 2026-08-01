namespace EnglishReadingApp;

public partial class ChineseLearningPage : ContentPage
{
    public ChineseLearningPage()
    {
        InitializeComponent();
    }  

    // 拼音学习
    private async void OnPinyinFrameTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChinesePinyinAlphabetPage());
    }

    // 词语学习
    private async void OnWordFrameTapped(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new ChineseWordPage());
    }

    // 古诗词
    private async void OnPoetryFrameTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChinesePoetryPage());
    }

    // 成语学习
    private async void OnIdiomFrameTapped(object sender, EventArgs e)
    {
       // await Navigation.PushAsync(new ChineseIdiomPage());
    }

    // 短文阅读
    private async void OnArticleFrameTapped(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new ChineseArticlePage());
    }
}