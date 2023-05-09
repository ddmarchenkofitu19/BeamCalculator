using BeamCalculator.Views;
using BeamCalculator.Views.Flyout;

namespace BeamCalculator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        if(DeviceInfo.Platform == DevicePlatform.Android)
        {
            // Pregenerating SectionToolPage for better performance
            var p = MauiProgram.Services.GetRequiredService<SectionToolPage>();
        }

        MainPage = new FlyoutRoot();
    }
}
