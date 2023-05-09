using SkiaSharp;

namespace BeamCalculator.Helpers.Drawing;


public struct SectionDrawingOptions : IDrawingOptions
{
	public SKColor SectionFillColor { get; set; }
    public SKColor SectionOutlineColor { get; set; }
    public float SectionOutlineWidth { get; set; }


	public bool DimLabelHasShortName { get; set; }
	public bool DimLabelHasValue { get; set; }
	public float DimLabelTextSize { get; set; }
    public float DimLabelLineWidth { get; set; }
    public float DimLabelPaddingFromSection { get; set; }
	public float PaddingBetweenDimLabels { get; set; }
	public float DimLabelLineLedge { get; set; }
    public float DimLabelTextPadding { get; set; }
    public float DimLabelArrowLength { get; set; }
    public float DimLabelArrowAngle { get; set; }


    public SectionDrawingOptions()
	{
		var settings = MauiProgram.Services.GetRequiredService<UserSettings>();

		SectionFillColor = settings.SectionFillColor;
        SectionOutlineColor = settings.SectionOutlineColor;

        DimLabelHasShortName = true;
        DimLabelHasValue = false;

        // platform unique parameters
        if (DeviceInfo.Platform == DevicePlatform.Android)
		{
            SectionOutlineWidth = 2f;
			DimLabelTextSize = 26;
			DimLabelLineWidth = 1f;
            DimLabelPaddingFromSection = 28f;
            PaddingBetweenDimLabels = 42f;
            DimLabelLineLedge = 8f;
            DimLabelTextPadding = 3f;
            DimLabelArrowLength = 18f;
            DimLabelArrowAngle = 25;
        }
		else if(DeviceInfo.Platform == DevicePlatform.WinUI)
		{
            SectionOutlineWidth = 2f;
            DimLabelTextSize = 20;
            DimLabelLineWidth = 1f;
            DimLabelPaddingFromSection = 18f;
            PaddingBetweenDimLabels = 32f;
            DimLabelLineLedge = 5f;
            DimLabelTextPadding = 4.5f;
            DimLabelArrowLength = 18f;
            DimLabelArrowAngle = 23;
        }
	}
}
