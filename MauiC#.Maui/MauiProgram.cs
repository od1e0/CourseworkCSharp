using Microsoft.Extensions.Logging;
using Maui.GoogleMaps.Hosting;
using MauiC_.Maui.Services;
using MauiC_.Maui.ViewModels;


namespace MauiC_.Maui
{
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
                    fonts.AddFont("fontello.ttf", "Icons");
                    fonts.AddFont("Rubik-Light.ttf", "RubikLight");
                    fonts.AddFont("Rubik-Regular.ttf", "RubikRegular");
                });

            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<ImageService>();
            builder.Services.AddSingleton<ProfileViewModel>();

            builder.Services.AddTransient<SettingsViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
#if ANDROID
        builder.UseGoogleMaps();
#elif IOS
        builder.UseGoogleMaps("AIzaSyDYqPTx-Uu6QTcfedkvxx1owf6uI8g8JgY");
#endif

            return builder.Build();
        }
    }
}
