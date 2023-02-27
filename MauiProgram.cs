using Plugin.Maui.Audio;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace ParkingSystem;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("Aclonica.ttf", "Aclonica");
			});
		builder.Services.AddSingleton(AudioManager.Current);
		builder.Services.AddTransient<Pages.HomePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<Pages.RegisterPage>();
        return builder.Build();
	}
}
