using BeamCalculator.Helpers.Drawing;
using BeamCalculator.Models.Section;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;

namespace BeamCalculator.ViewModels;


[ObservableObject]
public partial class SectionDesignerViewModel
{
    private readonly CustomSectionModel _model;

    [ObservableProperty]
    List<SKPoint> points;
    [ObservableProperty]
    List<List<int>> contours;
    [ObservableProperty]
    int selectedPointIndex = 0;
    [ObservableProperty]
    int selectedContourIndex = 0;
    [ObservableProperty]
    SKSize sectionSize;


    public SectionDesignerViewModel()
    {
        _model = MauiProgram.Services.GetRequiredService<CustomSectionModel>();
        _model.PropertyChanged += Model_PropertyChanged;
        Points = _model.DrawingPoints;
        Contours = _model.ContoursIndices;
        SectionSize = new SKSize((float)_model.Width, (float)_model.Height);
    }


    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CustomSectionModel.DrawingPoints))
        {
            Points = _model.DrawingPoints;
            SectionSize = new SKSize((float)_model.Width, (float)_model.Height);
        }
        else if (e.PropertyName == nameof(CustomSectionModel.ContoursIndices))
        {
            Contours = new List<List<int>>(_model.ContoursIndices);
            SectionSize = new SKSize((float)_model.Width, (float)_model.Height);
        }
    }

    public void MovePoint(SKPoint moveTo)
    {
        if (SelectedPointIndex < 0) return;

        var inverted = moveTo.InvertY();
        var p = inverted.ToMauiPoint();
        _model.MovePoint(SelectedPointIndex, moveTo.InvertY().ToMauiPoint());
    }

    public void AddPoint(SKPoint position)
    {
        if(selectedContourIndex < 0) return;

        var ind = _model.AddPoint(position.InvertY().ToMauiPoint(), selectedContourIndex);

        System.Diagnostics.Debug.WriteLine($"added point index: {ind}");
        if (ind >= 0)
            SelectedPointIndex = ind;
    }

    public void DeletePoint()
    {
        if(SelectedPointIndex < 0) return;

        if (_model.DeletePoint(SelectedPointIndex))
            SelectedPointIndex = -1;
    }

    public void AddHole(SKPoint position, float size)
    {
        var ind = _model.AddHole(position.InvertY().ToMauiPoint(), (double)new Decimal(size));

        if (ind > 0)
            SelectedContourIndex = ind;
    }

    public void DeleteHole()
    {
        if (SelectedContourIndex < 1) return;

        var cInd = SelectedContourIndex;

        _model.DeleteHole(SelectedContourIndex);

        SelectedContourIndex = cInd - 1;
    }
}
