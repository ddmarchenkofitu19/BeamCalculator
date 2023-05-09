using SkiaSharp;

namespace BeamCalculator.Helpers.Drawing;


public interface ICustomSectionDrawable
{
    void DrawSectionFill(SKCanvas canvas, SectionDrawingOptions options);

    void DrawSectionOutline(SKCanvas canvas, SectionDrawingOptions options);

    void DrawPointsLabels(SKCanvas canvas, SectionDrawingOptions options);

    void DrawInertiaAxes(SKCanvas canvas, SectionDrawingOptions options);

    void DrawInertiaEllipse(SKCanvas canvas, SectionDrawingOptions options);

    void DrawSectionCore(SKCanvas canvas, SectionDrawingOptions options);
}
