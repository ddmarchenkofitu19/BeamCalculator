using BeamCalculator.Helpers;
using BeamCalculator.Models;
using BeamCalculator.Models.Section;
using BeamCalculator.ViewModels;
using BeamCalculator.Views;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace BeamCalculator;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }


    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// add services for constructor injection
		ConfigureViews(builder);
		ConfigureViewModels(builder);
		ConfigureModels(builder);
		ConfigureServices(builder);

		var app = builder.Build();
		Services = app.Services;

		return app;
    }


	public static void ConfigureViews(MauiAppBuilder builder)
	{
        builder.Services.AddSingleton<ToolsListPage>();
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<SectionDesignerPage>();

        if(DeviceInfo.Platform == DevicePlatform.WinUI)
            builder.Services.AddTransient<SectionToolPage>();
        else if(DeviceInfo.Platform == DevicePlatform.Android)
            builder.Services.AddSingleton<SectionToolPage>();
    }

    public static void ConfigureViewModels(MauiAppBuilder builder)
    {
        builder.Services.AddTransient<CommonSectionViewModel>();
        builder.Services.AddTransient<CustomSectionViewModel>();
        builder.Services.AddTransient<SectionDesignerViewModel>();
    }

    public static void ConfigureModels(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<CustomSectionModel>();

        builder.Services.AddSingleton<RectangleSectionModel>();
        builder.Services.AddSingleton<TProfileSymmetricSectionModel>();
        builder.Services.AddSingleton<TProfileSectionModel>();
        builder.Services.AddSingleton<IProfileSectionModel>();
        builder.Services.AddSingleton<RectangleTubingSectionModel>();
        builder.Services.AddSingleton<UProfileSectionModel>();
        builder.Services.AddSingleton<RoundTubingSectionModel>();
        builder.Services.AddSingleton<XProfileSectionModel>();
        builder.Services.AddSingleton<LProfileSectionModel>();
    }

    public static void ConfigureServices(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton(e => Preferences.Default);
        builder.Services.AddSingleton<UserSettings>();
        builder.Services.AddSingleton<SectionModelProvider>();
    }
}
