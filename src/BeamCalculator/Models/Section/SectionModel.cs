using BeamCalculator.Helpers;
using BeamCalculator.Helpers.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace BeamCalculator.Models.Section;


[ObservableObject]
public partial class SectionModel
{
    protected readonly UserSettings _settings;

    [ObservableProperty]
    private CalculationResults _results;
    [ObservableProperty]
    private bool _needRedraw;
    protected DrawMapper<SectionDrawingOptions> DrawMapper;
    protected List<string> LayerDrawingOrder;


    public virtual SectionTypes Type => SectionTypes.None;

    public virtual List<Point> Points => throw new NotImplementedException();

    public virtual List<List<int>> ContoursIndices => throw new NotImplementedException();

    public virtual List<Fragment> Fragments => throw new NotImplementedException();

    public virtual double Width => Points.Max(p => p.X) - Points.Min(p => p.X);

    public virtual double Height => Points.Max(p => p.Y) - Points.Min(p => p.Y);
    
    public virtual Point Center => new Point(Points.Min(p => p.X) + Width / 2, Points.Min(p => p.Y) + Height / 2);

    public List<SKPoint> DrawingPoints => Points.Select(p => p.ToSKPoint().InvertY()).ToList();


    protected SectionModel(DrawMapper<SectionDrawingOptions> drawMapper, List<string> layerDrawingOrder)
    {
        DrawMapper = drawMapper;
        LayerDrawingOrder = layerDrawingOrder;
        _settings = MauiProgram.Services.GetRequiredService<UserSettings>();
    }

    public void Recalculate()
    {
        if (CheckSectionValidity())
        {
            CalculateResults();
            Redraw();
        }
    }

    protected virtual bool CheckSectionValidity()
        => true;

    protected virtual void Redraw()
        => NeedRedraw = true;

    protected virtual void CalculateResults()
        => Results = SectionParamsCalculator.CalculateFragmentedSection(Fragments, GetAllContours());


    protected List<Point> GetContour(int contourIndex)
    {
        return ContoursIndices[contourIndex].Select(i => Points[i]).ToList();
    }

    protected List<List<Point>> GetAllContours()
    {
        var c = new List<List<Point>>();

        for (var i = 0; i < ContoursIndices.Count; i++)
            c.Add(GetContour(i));

        return c;
    }

    protected void AppendToDrawing(string previousKey, string addKey, Action<SKCanvas, SectionDrawingOptions> action)
    {
        DrawMapper[addKey] = (section, canvas, options) => action?.Invoke(canvas, options);
        LayerDrawingOrder.InsertAfter(addKey, previousKey);
    }
    protected void PrependToDrawing(string nextKey, string addKey, Action<SKCanvas, SectionDrawingOptions> action)
    {
        DrawMapper[addKey] = (section, canvas, options) => action?.Invoke(canvas, options);
        LayerDrawingOrder.InsertBefore(addKey, nextKey);
    }

    public virtual void Draw(SKCanvas canvas, SKRect bounds)
        => this.Draw(canvas, bounds, new SectionDrawingOptions());

    public virtual void Draw(SKCanvas canvas, SKRect bounds, SectionDrawingOptions options)
    {
        var saved = canvas.TotalMatrix;
        var layers = LayerDrawingOrder;

        // move bounds center to (0,0) point
        var matr = SKMatrix.CreateTranslation(bounds.MidX - (float)Center.X, bounds.MidY + (float)Center.Y);

        canvas.SetMatrix(canvas.TotalMatrix.PreConcat(matr));

        foreach (var layer in layers)
        {
            DrawMapper.DrawLayer(layer, this, canvas, options);
        }
        
        canvas.SetMatrix(saved);
    }
}
