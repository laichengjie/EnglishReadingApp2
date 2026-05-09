namespace EnglishReadingApp;

public partial class SubjectPage : ContentPage
{
    public SubjectPage()
    {
        InitializeComponent();
    }

    private async void OnChineseClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChinesePage());
    }

    private async void OnMathClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MathPage());
    }

    private async void OnEnglishClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EnglishPage());
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("确认退出", "确定要退出吗？", "是", "否");
        if (confirm)
        {
            // 返回到登录页
            //await Navigation.PopToRootAsync();

            // 退出整个应用程序
            Application.Current.Quit();
        }
    }
}