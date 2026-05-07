namespace EnglishReadingApp;

public partial class App : Application
{
    public App(IServiceProvider services)
    {
        InitializeComponent();

        // 全局异常处理
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"崩溃异常: {ex?.Message}");
            System.Diagnostics.Debug.WriteLine($"堆栈: {ex?.StackTrace}");
        };

        // 通过依赖注入获取 MainPage
        MainPage = new NavigationPage(services.GetRequiredService<MainPage>());
    }
}