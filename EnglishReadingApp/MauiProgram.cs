using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace EnglishReadingApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .AddAudio();  // 添加音频支持

        // 注册服务
        builder.Services.AddSingleton(AudioManager.Current);  // 注册音频管理器
        builder.Services.AddTransient<MainPage>();            // 注册 MainPage
        builder.Services.AddTransient<SubjectPage>();   
        builder.Services.AddTransient<ChinesePage>();
        builder.Services.AddTransient<ChineseLearningPage>();
        builder.Services.AddTransient<ChinesePinyinAlphabetPage>();
        builder.Services.AddTransient<EnglishPage>();            
        builder.Services.AddTransient<MathPage>();            

        // 可选：注册其他服务
        builder.Services.AddSingleton<IAudioManager>(AudioManager.Current);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}