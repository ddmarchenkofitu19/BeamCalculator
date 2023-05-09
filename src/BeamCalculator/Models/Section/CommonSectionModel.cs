using BeamCalculator.Helpers.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;

namespace BeamCalculator.Models.Section;


public partial class CommonSectionModel : SectionModel, ICommonSectionDrawable
{
    private static DrawMapper<SectionDrawingOptions> GetDefaultDrawMapper()
        => new DrawMapper<SectionDrawingOptions>()
        {
            [nameof(ICommonSectionDrawable.DrawSectionFill)] = MapDrawSectionFill,
            [nameof(ICommonSectionDrawable.DrawSectionOutline)] = MapDrawSectionOutline,
            [nameof(ICommonSectionDrawable.DrawDimensionLabels)] = MapDrawDimensionLabels,
            [nameof(ICommonSectionDrawable.DrawInertiaAxes)] = MapDrawInertiaAxes,
        };
    private static List<string> GetDefaultLayerDrawingOrder()
        => new List<string>()
        {
            nameof(ICommonSectionDrawable.DrawSectionFill),
            nameof(ICommonSectionDrawable.DrawSectionOutline),
            nameof(ICommonSectionDrawable.DrawDimensionLabels),
            nameof(ICommonSectionDrawable.DrawInertiaAxes),
        };

    private static void MapDrawSectionFill(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
    => (section as ICommonSectionDrawable).DrawSectionFill(canvas, options);
    private static void MapDrawSectionOutline(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICommonSectionDrawable).DrawSectionOutline(canvas, options);
    private static void MapDrawDimensionLabels(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICommonSectionDrawable).DrawDimensionLabels(canvas, options);
    private static void MapDrawInertiaAxes(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICommonSectionDrawable).DrawInertiaAxes(canvas, options);


    [ObservableProperty]
    private string _errorString;

    protected readonly Dictionary<string, SectionDimensionData> _dimesions = new Dictionary<string, SectionDimensionData>();

    public override Point Center => new Point(0, 0);

    public virtual DimensionLabel[] DimensionLabels => throw new NotImplementedException();


    protected CommonSectionModel(Dictionary<string, SectionDimensionData> dimensions)
        : base(GetDefaultDrawMapper(), GetDefaultLayerDrawingOrder())
    {
        _dimesions = dimensions;
        _settings.PropertyChanged += Settings_PropertyChanged;
    }


    public ReadOnlyDictionary<string, SectionDimensionData> GetDimensions()
        => new ReadOnlyDictionary<string, SectionDimensionData>(_dimesions);

    public void SetDimensionValue(string name, double value)
    {
        if (_dimesions.ContainsKey(name))
        {
            var convValue = _settings.InputDistanceUnitType.ConvertTo(CalculationResults.ResultValuesUnit, value);
            if (_dimesions[name].Value != convValue)
            {
                _dimesions[name].Value = convValue;
                Recalculate();
            }
        }
    }

    protected override bool CheckSectionValidity()
    {
        foreach (var dim in _dimesions.Values)
        {
            if (dim.Value <= 0)
            {
                ErrorString = String.Format("{0} must be greater then 0!", dim.Value);
                return false;
            }
        }

        ErrorString = "";
        return true;
    }

    private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_settings.InputDistanceUnitType))
        {
            Recalculate();
        }
    }



    void ICommonSectionDrawable.DrawSectionFill(SKCanvas canvas, SectionDrawingOptions options)
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
        var points = DrawingPoints;

        for (var i = 0; i < ContoursIndices.Count; i++)
        {
            var path = new SKPath();
            for (var j = 0; j < ContoursIndices[i].Count; j++)
            {
                if (j == 0)
                    path.MoveTo(points[ContoursIndices[i][j]]);
                else
                    path.LineTo(points[ContoursIndices[i][j]]);
            }
            path.Close();

            canvas.DrawPath(path, i == 0 ? fill : hole);
        }
    }

    void ICommonSectionDrawable.DrawSectionOutline(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var stroke = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = options.SectionOutlineColor,
            StrokeWidth = options.SectionOutlineWidth / canvas.TotalMatrix.ScaleX,
        };
        var points = DrawingPoints;

        for (var i = 0; i < ContoursIndices.Count; i++)
        {
            var path = new SKPath();
            for (var j = 0; j < ContoursIndices[i].Count; j++)
            {
                if (j == 0)
                    path.MoveTo(points[ContoursIndices[i][j]]);
                else
                    path.LineTo(points[ContoursIndices[i][j]]);
            }
            path.Close();

            canvas.DrawPath(path, stroke);
        }
    }

    void ICommonSectionDrawable.DrawDimensionLabels(SKCanvas canvas, SectionDrawingOptions options)
    {
        var dl = DimensionLabels;
        var paddingFromSection = options.DimLabelPaddingFromSection / canvas.TotalMatrix.ScaleX;
        var paddingBetweenLabels = options.PaddingBetweenDimLabels / canvas.TotalMatrix.ScaleX;
        var lineLedge = options.DimLabelLineLedge / canvas.TotalMatrix.ScaleX;
        var arrowLength = options.DimLabelArrowLength / canvas.TotalMatrix.ScaleX;
        var arrowAngle = options.DimLabelArrowAngle;
        var textPadding = options.DimLabelTextPadding / canvas.TotalMatrix.ScaleX;

        using var linePaint = new SKPaint
        {
            StrokeWidth = options.DimLabelLineWidth / canvas.TotalMatrix.ScaleX,
            Color = SKColors.Blue,
        };
        using var textPaint = new SKPaint
        {
            TextSize = options.DimLabelTextSize / canvas.TotalMatrix.ScaleX,
            Color = SKColors.Black,
            TextAlign = SKTextAlign.Left,
            //IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", new SKFontStyle(SKFontStyleWeight.Normal, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright)),
        };


        for (int i = 0; i < dl.Length; i++)
        {
            string str = options.DimLabelHasShortName ? dl[i].ShortName : "";
            str += options.DimLabelHasShortName && options.DimLabelHasValue ? "=" : "";
            str += options.DimLabelHasValue ? _dimesions[dl[i].Name].Value.ToString("F4", CultureInfo.InvariantCulture).TrimEnd(new char[] { '0', '.' }) : "";
            str += options.DimLabelHasValue ? _settings.InputDistanceUnitType.ToUserFriendlyString() : "";

            switch (dl[i].Orientation)
            {
                case DimensionLabelOrientation.VerticalLeft:
                    // label position
                    var xpos = -(float)Width / 2 - (paddingFromSection + paddingBetweenLabels * (dl[i].Level));
                    var p1 = new SKPoint(xpos, dl[i].Pos1);
                    var p2 = new SKPoint(xpos, dl[i].Pos2);

                    // draw main dimension line with arrows
                    canvas.DrawArrow(p1, p2, arrowLength, arrowAngle, linePaint, DrawingExtensions.ArrowDirection.Both);

                    // draw line tips
                    var tipX1 = xpos - lineLedge;
                    var tipX2 = -(float)Width / 2 - paddingFromSection * 0.2f;
                    canvas.DrawLine(new SKPoint(tipX1, dl[i].Pos1), new SKPoint(tipX2, dl[i].Pos1), linePaint);
                    canvas.DrawLine(new SKPoint(tipX1, dl[i].Pos2), new SKPoint(tipX2, dl[i].Pos2), linePaint);

                    // Maesure string
                    var textWidth = textPaint.MeasureText(str);

                    var midLine = p1.Y + MathF.Abs(p1.Y - p2.Y) / 2;
                    var strPos = new SKPoint(-midLine - textWidth / 2, xpos - textPadding);

                    // Save matrix and rotate canvas
                    var saved = canvas.TotalMatrix;
                    canvas.RotateDegrees(-90);

                    // Draw string
                    canvas.DrawText(str, strPos, textPaint);

                    // Restore matrix
                    canvas.SetMatrix(saved);
                    break;


                case DimensionLabelOrientation.VerticalRight:
                    // label position
                    xpos = (float)Width / 2 + (paddingFromSection + paddingBetweenLabels * (dl[i].Level));
                    p1 = new SKPoint(xpos, dl[i].Pos1);
                    p2 = new SKPoint(xpos, dl[i].Pos2);

                    // draw main dimension line with arrows
                    canvas.DrawArrow(p1, p2, arrowLength, arrowAngle, linePaint, DrawingExtensions.ArrowDirection.Both);

                    // draw line tips
                    tipX1 = xpos - paddingFromSection * 0.8f;
                    tipX2 = xpos + lineLedge;
                    canvas.DrawLine(new SKPoint(tipX1, dl[i].Pos1), new SKPoint(tipX2, dl[i].Pos1), linePaint);
                    canvas.DrawLine(new SKPoint(tipX1, dl[i].Pos2), new SKPoint(tipX2, dl[i].Pos2), linePaint);

                    // Maesure string
                    textWidth = textPaint.MeasureText(str);
                    var textSize = new SKRect();
                    textPaint.MeasureText(str, ref textSize);

                    midLine = p1.Y + MathF.Abs(p1.Y - p2.Y) / 2;
                    var metrics = textPaint.FontMetrics;
                    strPos = new SKPoint(-midLine - textWidth / 2, xpos + -metrics.Ascent);

                    // Save matrix and rotate canvas
                    saved = canvas.TotalMatrix;
                    canvas.RotateDegrees(-90);

                    // Draw string
                    canvas.DrawText(str, strPos, textPaint);

                    // Restore matrix
                    canvas.SetMatrix(saved);
                    break;


                case DimensionLabelOrientation.HorizontalUp:
                    // label position
                    var ypos = -(float)Height / 2 - (paddingFromSection + paddingBetweenLabels * (dl[i].Level));
                    p1 = new SKPoint(dl[i].Pos1, ypos);
                    p2 = new SKPoint(dl[i].Pos2, ypos);

                    // draw main dimension line with arrows
                    canvas.DrawArrow(p1, p2, arrowLength, arrowAngle, linePaint, DrawingExtensions.ArrowDirection.Both);

                    // draw line tips
                    var tipY1 = ypos - lineLedge;
                    var tipY2 = -(float)Height / 2 - paddingFromSection * 0.2f;
                    canvas.DrawLine(new SKPoint(dl[i].Pos1, tipY1), new SKPoint(dl[i].Pos1, tipY2), linePaint);
                    canvas.DrawLine(new SKPoint(dl[i].Pos2, tipY1), new SKPoint(dl[i].Pos2, tipY2), linePaint);

                    // Maesure string
                    textWidth = textPaint.MeasureText(str);

                    midLine = p1.X + MathF.Abs(p1.X - p2.X) / 2;
                    strPos = new SKPoint(midLine - textWidth / 2, ypos - textPadding);

                    // Draw string
                    canvas.DrawText(str, strPos, textPaint);
                    break;


                case DimensionLabelOrientation.HorizontalDown:
                    // label position
                    ypos = (float)Height / 2 + (paddingFromSection + paddingBetweenLabels * (dl[i].Level));
                    p1 = new SKPoint(dl[i].Pos1, ypos);
                    p2 = new SKPoint(dl[i].Pos2, ypos);

                    // draw main dimension line with arrows
                    canvas.DrawArrow(p1, p2, arrowLength, arrowAngle, linePaint, DrawingExtensions.ArrowDirection.Both);

                    // draw line tips
                    tipY1 = (float)Height / 2 + paddingFromSection * 0.2f;
                    tipY2 = ypos + lineLedge;
                    canvas.DrawLine(new SKPoint(dl[i].Pos1, tipY1), new SKPoint(dl[i].Pos1, tipY2), linePaint);
                    canvas.DrawLine(new SKPoint(dl[i].Pos2, tipY1), new SKPoint(dl[i].Pos2, tipY2), linePaint);

                    // Maesure string
                    textWidth = textPaint.MeasureText(str);
                    textSize = new SKRect();
                    textPaint.MeasureText(str, ref textSize);

                    metrics = textPaint.FontMetrics;

                    midLine = p1.X + MathF.Abs(p1.X - p2.X) / 2;
                    strPos = new SKPoint(midLine - textWidth / 2, ypos + -metrics.Ascent); // textSize.Height

                    // Draw string
                    canvas.DrawText(str, strPos, textPaint);
                    break;
            }
        }
    }

    void ICommonSectionDrawable.DrawInertiaAxes(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var axisStroke = new SKPaint
        {
            Color = SKColors.Blue,
            StrokeWidth = options.DimLabelLineWidth * 2 / canvas.TotalMatrix.ScaleX,
            IsAntialias = true,
        };
        using var textPaint = new SKPaint
        {
            TextSize = options.DimLabelTextSize / canvas.TotalMatrix.ScaleX,
            Color = SKColors.Black,
            TextAlign = SKTextAlign.Left,
            Typeface = SKTypeface.FromFamilyName("Arial", new SKFontStyle(SKFontStyleWeight.Normal, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright)),
        };

        var b = canvas.LocalClipBounds;

        // calc axes arrows size and target points
        var axisSize = MathF.Min(b.Width, b.Height) / 10f;
        var pZ = new SKPoint(0, 0 - axisSize);
        var pY = new SKPoint(0 + axisSize, 0);

        // save matrix and rotate canvas
        var saved = canvas.TotalMatrix;
        canvas.RotateRadians((float)-Results.InertiaAxesAngle, 0, 0);

        // Z axis
        canvas.DrawArrow(
            new SKPoint(0, 0),
            pZ,
            13 / saved.ScaleX,
            27,
            axisStroke);

        // add padding between arrow and label
        pZ.Y -= options.DimLabelTextPadding / saved.ScaleX;
        
        // rotate canvas back around label drawing point
        canvas.Save();
        canvas.RotateRadians((float)Results.InertiaAxesAngle, pZ.X, pZ.Y);

        // draw label verticaly
        canvas.DrawText(
            "Z1",
            pZ,
            textPaint);

        // restore canvas state
        canvas.Restore();


        // Y axis
        axisStroke.Color = SKColors.Green;
        canvas.DrawArrow(
            new SKPoint(0, 0),
            pY,
            13 / saved.ScaleX,
            27,
            axisStroke);

        // add padding between arrow and label
        pY.Y -= options.DimLabelTextPadding / saved.ScaleX;

        // rotate canvas back around label drawing point
        canvas.Save();
        canvas.RotateRadians((float)Results.InertiaAxesAngle, pY.X, pY.Y);

        // draw label verticaly
        canvas.DrawText(
            "Y1",
            pY,
            textPaint);

        // restore matrix
        canvas.SetMatrix(saved);
    }
}
