using BeamCalculator.Helpers.Drawing;
using SkiaSharp;

namespace BeamCalculator.Models.Section;


public class RoundTubingSectionModel : CommonSectionModel
{
    private static readonly Dictionary<string, SectionDimensionData> roundTubingDimensions = new Dictionary<string, SectionDimensionData>()
    {
        ["outer diameter"] = new SectionDimensionData()
        {
            Value = 40,
            Name = "outer diameter",
            ShortName = "D",
        },
        ["inner diameter"] = new SectionDimensionData()
        {
            Value = 10,
            Name = "inner diameter",
            ShortName = "d",
        },
    };

    private double _dimOuterDiameter => _dimesions["outer diameter"].Value;
    private double _dimInnerDiameter => _dimesions["inner diameter"].Value;


    public override SectionTypes Type => SectionTypes.RoundTubing;
    public override DimensionLabel[] DimensionLabels => new DimensionLabel[]
    {
        new DimensionLabel()
        {
            ShortName = "D",
            Name = "outer diameter",
            Orientation = DimensionLabelOrientation.VerticalLeft,
            Level = 0,
            Pos1 = (float)-OuterDiameter / 2,
            Pos2 = (float)OuterDiameter / 2,
        },
        new DimensionLabel()
        {
            ShortName = "d",
            Name = "inner diameter",
            Orientation = DimensionLabelOrientation.HorizontalDown,
            Level = 0,
            Pos1 = (float)-InnerDiameter / 2,
            Pos2 = (float)InnerDiameter / 2,
        },
    };

    public override double Width => _dimOuterDiameter;
    public override double Height => _dimOuterDiameter;
    public double OuterDiameter => _dimOuterDiameter;
    public double InnerDiameter => _dimInnerDiameter;


    public RoundTubingSectionModel() : base(roundTubingDimensions)
    {
        DrawMapper[nameof(ICommonSectionDrawable.DrawSectionFill)] = (section, canvas, options) => DrawSectionFill(canvas, options);
        DrawMapper[nameof(ICommonSectionDrawable.DrawSectionOutline)] = (section, canvas, options) => DrawSectionOutline(canvas, options);

    }


    protected override bool CheckSectionValidity()
    {
        if (!base.CheckSectionValidity())
            return false;

        var err = "";
        if (_dimInnerDiameter >= _dimOuterDiameter)
            err = "d must be less than D";

        ErrorString = err;
        if (err == "")
            return true;

        return false;
    }

    protected override void CalculateResults()
    {
        Results = SectionParamsCalculator.CalculateRoundTubing(_dimOuterDiameter, _dimInnerDiameter);
    }


    void DrawSectionFill(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var fill = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = options.SectionFillColor,
        };
        using var hole = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Clear,
        };

        canvas.DrawCircle(0, 0, (float)OuterDiameter / 2, fill);

        canvas.DrawCircle(0, 0, (float)InnerDiameter / 2, hole);
    }

    void DrawSectionOutline(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var stroke = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = options.SectionOutlineColor,
            StrokeWidth = options.SectionOutlineWidth / canvas.TotalMatrix.ScaleX,
        };

        canvas.DrawCircle(0, 0, (float)OuterDiameter / 2, stroke);

        canvas.DrawCircle(0, 0, (float)InnerDiameter / 2, stroke);
    }
}
