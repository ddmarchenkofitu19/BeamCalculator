using SkiaSharp;

namespace BeamCalculator.Helpers.Drawing;


public interface ICommonSectionDrawable
{
    void DrawSectionFill(SKCanvas canvas, SectionDrawingOptions options);

    void DrawSectionOutline(SKCanvas canvas, SectionDrawingOptions options);

    void DrawDimensionLabels(SKCanvas canvas, SectionDrawingOptions options);

    void DrawInertiaAxes(SKCanvas canvas, SectionDrawingOptions options);
}
