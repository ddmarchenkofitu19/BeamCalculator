using BeamCalculator.Components;
using BeamCalculator.Helpers;
using BeamCalculator.Helpers.Drawing;
using BeamCalculator.Models;
using BeamCalculator.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace BeamCalculator.Views;


public partial class SectionDesignerPage : ContentPage
{
    private readonly UserSettings _settings;
    private readonly SectionDesignerViewModel _vm;
    private readonly float _pointRadius = DeviceInfo.Platform == DevicePlatform.WinUI ? 8 : 15;
    private readonly float _pointTouchRadius = DeviceInfo.Platform == DevicePlatform.WinUI ? 10 : 30;
    private float _gridCellSize;
    private bool _firstDraw = true;
    private SKMatrix _touchTransforms;
    private Dictionary<long, SectionDesignerTouchInteraction> _touches = new Dictionary<long, SectionDesignerTouchInteraction>();
    private SectionDesignerState _state = SectionDesignerState.Normal;


    #region BindableProperties

    public static readonly BindableProperty PointsProperty = BindableProperty.Create(
           propertyName: nameof(Points),
           returnType: typeof(List<SKPoint>),
           declaringType: typeof(SectionDesignerPage),
           defaultBindingMode: BindingMode.OneWay,
           propertyChanged: (b, o, n) => (b as SectionDesignerPage).Invalidate());

    public static readonly BindableProperty ContoursIndicesProperty = BindableProperty.Create(
           propertyName: nameof(ContoursIndices),
           returnType: typeof(List<List<int>>),
           declaringType: typeof(SectionDesignerPage),
           defaultBindingMode: BindingMode.OneWay,
           propertyChanged: (b, o, n) => 
           {
               var page = b as SectionDesignerPage;
               page.UpdateContoursNames();
               page.Invalidate();

               if(((List<List<int>>)n).Count == 1)
               {
                   page.deleteHoleButton.IsEnabled = false;
               }
               else
               {
                   page.deleteHoleButton.IsEnabled = true;
               }
           });

    public static readonly BindableProperty SectionSizeProperty = BindableProperty.Create(
           propertyName: nameof(SectionSize),
           returnType: typeof(SKSize),
           declaringType: typeof(SectionDesignerPage),
           defaultBindingMode: BindingMode.OneWay);

    public static readonly BindableProperty SelectedPointIndexProperty = BindableProperty.Create(
            propertyName: nameof(SelectedPointIndex),
            returnType: typeof(int),
            declaringType: typeof(SectionDesignerPage),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0,
            propertyChanged: (b, o, n) => (b as SectionDesignerPage).Invalidate());

    public static readonly BindableProperty SelectedContourIndexProperty = BindableProperty.Create(
            propertyName: nameof(SelectedContourIndex),
            returnType: typeof(int),
            declaringType: typeof(SectionDesignerPage),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0,
            propertyChanged: (b, o, n) =>
            {
                if ((int)n < 0) return;

                var page = b as SectionDesignerPage;

                if (page.selectedContourPicker.SelectedIndex != (int)n)
                    page.selectedContourPicker.SelectedIndex = (int)n;

                page.Invalidate();
            });

    public List<SKPoint> Points
    {
        get => (List<SKPoint>)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    public List<List<int>> ContoursIndices
    {
        get => (List<List<int>>)GetValue(ContoursIndicesProperty);
        set => SetValue(ContoursIndicesProperty, value);
    }

    public SKSize SectionSize
    {
        get => (SKSize)GetValue(SectionSizeProperty);
        set => SetValue(SectionSizeProperty, value);
    }

    public int SelectedPointIndex
    {
        get => (int)GetValue(SelectedPointIndexProperty);
        set => SetValue(SelectedPointIndexProperty, value);
    }

    public int SelectedContourIndex
    {
        get => (int)GetValue(SelectedContourIndexProperty);
        set => SetValue(SelectedContourIndexProperty, value);
    }

    #endregion


    public SectionDesignerPage()
	{
		InitializeComponent();

        _settings = MauiProgram.Services.GetRequiredService<UserSettings>();
        _settings.PropertyChanged += Settings_PropertyChanged;

        // get VM
        _vm = MauiProgram.Services.GetRequiredService<SectionDesignerViewModel>();
        this.BindingContext = _vm;
        this.SetBinding(PointsProperty, new Binding(nameof(SectionDesignerViewModel.Points)));
        this.SetBinding(ContoursIndicesProperty, new Binding(nameof(SectionDesignerViewModel.Contours)));
        this.SetBinding(SectionSizeProperty, new Binding(nameof(SectionDesignerViewModel.SectionSize)));
        this.SetBinding(SelectedPointIndexProperty, new Binding(nameof(SectionDesignerViewModel.SelectedPointIndex)));
        this.SetBinding(SelectedContourIndexProperty, new Binding(nameof(SectionDesignerViewModel.SelectedContourIndex)));

        // set item source for grid unit selector
        gridUnitPicker.ItemsSource = Enum.GetValues<DistanceUnitType>().Select(u => u.ToUserFriendlyString()).ToArray();
        gridUnitPicker.SelectedIndex = (int)DistanceUnitType.Centimeters;

        // set title view 
        if (DeviceInfo.Platform == DevicePlatform.Android)
            NavigationPage.SetTitleView(this, new ButtonsTitleView()
            {
                Title = "Section Designer",
                Buttons = new TitleViewButton[]
                {
                    new()
                    {
                        ImageSource = "done_black_24dp.png",
                        Command = new Command(this.Done_Tapped)
                    }
                }
            });
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            NavigationPage.SetTitleView(this, new DefaultTitleView() { Title = "Section Designer" });
    }


    public void Invalidate()
    => canvasView.InvalidateSurface();

    public void InvalidateResetMatrix()
    {
        _firstDraw = true;
        canvasView.InvalidateSurface();
    }

    public void Done()
    {
        var page = MauiProgram.Services.GetRequiredService<SectionToolPage>();
        page.ToolType = SectionTypes.Custom;

        Navigation.PushAsync(page);
    }

    private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UserSettings.SectionOutlineColor) ||
            e.PropertyName == nameof(UserSettings.SectionFillColor) ||
            e.PropertyName == nameof(UserSettings.SectionFillOpacity))
        {
            Invalidate();
        }
    }

    private void Done_Tapped()
        => Done();

    private void DoneButton_Clicked(object sender, EventArgs e)
        => Done();

    private void GridUnit_Changed(object sender, EventArgs e)
    {
        var unit = (DistanceUnitType)gridUnitPicker.SelectedIndex;

        if (Enum.IsDefined(unit))
        {
            _gridCellSize = (float)unit.ConvertTo(CalculationResults.ResultValuesUnit, 1);
            InvalidateResetMatrix();
        }
    }

    private void SelectedContour_Changed(object sender, EventArgs e)
    {
        if (selectedContourPicker.SelectedIndex < 0) return;

        SelectedContourIndex = selectedContourPicker.SelectedIndex;
    }

    private void AddPointButton_Clicked(object sender, EventArgs e)
    {
        if(_state != SectionDesignerState.AddPoint)
            ChangeDesignerState(SectionDesignerState.AddPoint);
        else
            ChangeDesignerState(SectionDesignerState.Normal);
    }

    private void DeletePointButton_Clicked(object sender, EventArgs e)
    {
        _vm.DeletePoint();
    }

    private void AddHoleButton_Clicked(object sender, EventArgs e)
    {
        if (_state != SectionDesignerState.AddHole)
            ChangeDesignerState(SectionDesignerState.AddHole);
        else
            ChangeDesignerState(SectionDesignerState.Normal);
    }

    private void DeleteHoleButton_Clicked(object sender, EventArgs e)
    {
        _vm.DeleteHole();
        
    }

    private void UpdateContoursNames()
    {
        var list = new List<string>();
        list.Add("outer contour");

        for(var i = 1; i < ContoursIndices.Count; i++)
            list.Add($"hole {i}");

        selectedContourPicker.ItemsSource = list;
        selectedContourPicker.SelectedIndex = 0;
    }

    private void ChangeDesignerState(SectionDesignerState state)
    {
        _state = state;

        switch (state)
        {
            case SectionDesignerState.Normal:
                addPointButton.Background = Color.FromArgb("#a2d5d5d5"); // unselected
                addHoleButton.Background = Color.FromArgb("#a2d5d5d5");  // unselected
                break;

            case SectionDesignerState.AddPoint:
                addPointButton.Background = Color.FromArgb("#a280deea"); // selected
                addHoleButton.Background = Color.FromArgb("#a2d5d5d5");  // unselected
                break;

            case SectionDesignerState.AddHole:
                addPointButton.Background = Color.FromArgb("#a2d5d5d5"); // unselected
                addHoleButton.Background = Color.FromArgb("#a280deea");  // selected
                break;
        }
    }

    private void OnTouch(object sender, SKTouchEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine(
            $"id: {e.Id} {e.ActionType} loc: {e.Location} cont: {e.InContact}");
        SKMatrix scaleMatr;

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                // find closest among touched poits
                int nearestPointInd = -1;
                float minDistanceFromTouch = float.MaxValue;
                var mappedLoc = _touchTransforms.Invert().MapPoint(e.Location);

                for (var i = 0; i < Points.Count; i++)
                {
                    var distance = (mappedLoc - Points[i]).Length;

                    if (distance < _pointTouchRadius * 2 / _touchTransforms.ScaleX && distance < minDistanceFromTouch)
                    {
                        minDistanceFromTouch = distance;
                        nearestPointInd = i;
                    }
                }

                _touches.TryAdd(e.Id, new SectionDesignerTouchInteraction(e.Location, nearestPointInd));
                e.Handled = true;
                break;

            case SKTouchAction.Cancelled:
            case SKTouchAction.Released:
                if (_touches.ContainsKey(e.Id))
                {
                    // if touch moved less then point diameter
                    if ((_touches[e.Id].StartPos - e.Location).Length < _pointTouchRadius * 2 / _touchTransforms.ScaleX)
                    {
                        if(_state == SectionDesignerState.Normal)
                        {
                            // if there is no point touched or currently selected point touched
                            if (_touches[e.Id].TouchedPointIndex < 0 ||
                                SelectedPointIndex >= 0 && _touches[e.Id].TouchedPointIndex == SelectedPointIndex)
                            {
                                // deselect selected point
                                SelectedPointIndex = -1;

                                // redraw
                                canvasView.InvalidateSurface();
                            }
                            // if some point touched and it is not same point as selected point
                            else if (_touches[e.Id].TouchedPointIndex >= 0 && _touches[e.Id].TouchedPointIndex != SelectedPointIndex)
                            {
                                // select touched point 
                                SelectedPointIndex = _touches[e.Id].TouchedPointIndex;

                                // redraw
                                canvasView.InvalidateSurface();
                            }
                        }
                        else if(_state == SectionDesignerState.AddPoint)
                        {
                            mappedLoc = _touchTransforms.Invert().MapPoint(e.Location);
                            _vm.AddPoint(AdjustPointToGrid(mappedLoc));
                            ChangeDesignerState(SectionDesignerState.Normal);
                        }
                        else if (_state == SectionDesignerState.AddHole)
                        {
                            mappedLoc = _touchTransforms.Invert().MapPoint(e.Location);
                            _vm.AddHole(AdjustPointToGrid(mappedLoc), _gridCellSize);
                            ChangeDesignerState(SectionDesignerState.Normal);
                        }
                    }

                    // remove from touch collection
                    _touches.Remove(e.Id);
                    e.Handled = true;
                }
                break;

            case SKTouchAction.Moved:
                if (_touches.ContainsKey(e.Id))
                {
                    // ignore if position not changed
                    if (_touches[e.Id].CurrentPos == e.Location) 
                        return;

                    // if only one touch point => move gesture or move point
                    if (_touches.Count == 1)
                    {
                        // if selected poind touched => invoke VM MovePoint
                        if (SelectedPointIndex >= 0 && _touches[e.Id].TouchedPointIndex == SelectedPointIndex)
                        {
                            var curMapped = _touchTransforms.Invert().MapPoint(e.Location);
                            var adjusted = AdjustPointToGrid(curMapped);
                            _vm.MovePoint(adjusted);
                        }
                        else
                        {
                            var curP = e.Location;
                            var prevP = _touches[e.Id].CurrentPos;

                            _touchTransforms.TransX += curP.X - prevP.X;
                            _touchTransforms.TransY += curP.Y - prevP.Y;
                        }

                        canvasView.InvalidateSurface();
                    }
                    else if (_touches.Count == 2) // if 2 touch points => pitch(zoom on android) gesture 
                    {
                        long[] keys = new long[_touches.Count];
                        _touches.Keys.CopyTo(keys, 0);

                        int pivotInd = (keys[0] == e.Id) ? 1 : 0;

                        var pivotP = _touches[keys[pivotInd]].CurrentPos;
                        var prevP = _touches[e.Id].CurrentPos;
                        var newP = e.Location;

                        var oldVect = prevP - pivotP;
                        var newVect = newP - pivotP;

                        var scale = newVect.Length / oldVect.Length;

                        if (!float.IsNaN(scale) && !float.IsInfinity(scale))
                        {
                            scaleMatr = SKMatrix.CreateScale(scale, scale, pivotP.X, pivotP.Y);

                            _touchTransforms = _touchTransforms.PostConcat(scaleMatr);
                            canvasView.InvalidateSurface();
                        }
                    }

                    // update current touch position
                    _touches[e.Id].CurrentPos = e.Location;
                    e.Handled = true;
                }
                break;

            case SKTouchAction.WheelChanged:
                // zoom on windows
                var dt = 1 + e.WheelDelta * 0.001f;
                scaleMatr = SKMatrix.CreateScale(dt, dt, e.Location.X, e.Location.Y);

                _touchTransforms = _touchTransforms.PostConcat(scaleMatr);
                canvasView.InvalidateSurface();
                break;
        }
    }

    private SKPoint AdjustPointToGrid(SKPoint point)
    {
        var halfcellsize = _gridCellSize / 2;

        var xoffset = point.X % _gridCellSize;
        var yoffset = point.Y % _gridCellSize;

        if (MathF.Abs(xoffset) < halfcellsize)
            point.X -= xoffset;
        else
            point.X += xoffset > 0 ? _gridCellSize - xoffset : -(_gridCellSize + xoffset);

        if (MathF.Abs(yoffset) < halfcellsize)
            point.Y -= yoffset;
        else
            point.Y += yoffset > 0 ? _gridCellSize - yoffset : -(_gridCellSize + yoffset);

        return point;
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;
        var bounds = canvas.LocalClipBounds;

        canvas.Clear(SKColors.Transparent);

        // calculate starting matrix with translation to center and scale depending on section size
        if (_firstDraw)
        {
            //_touchTransforms = SKMatrix.CreateTranslation(bounds.MidX, bounds.MidY);
            var widthRatio = (bounds.Width * 0.4f) / SectionSize.Width;
            var heightRatio = (bounds.Height * 0.4f) / SectionSize.Height;
            var scale = (float)Math.Min(widthRatio, heightRatio);
            scale = Double.IsInfinity(scale) ? 1 : scale;

            _touchTransforms = SKMatrix.CreateScaleTranslation(scale, scale, bounds.MidX, bounds.MidY);
            _firstDraw = false;
        }

        // save matrix and apply touch matrix
        var savedMatr = canvas.TotalMatrix;
        canvas.SetMatrix(_touchTransforms);


        // Draw things
        DrawSection(canvas);
        DrawBackground(canvas);


        // restore matrix
        canvas.SetMatrix(savedMatr);
    }

    private void DrawSection(SKCanvas canvas)
    {
        var o = new SectionDrawingOptions();
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = o.SectionFillColor,
        };
        using var hole = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Clear,
        };
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = o.SectionOutlineColor,
            StrokeWidth = o.SectionOutlineWidth / canvas.TotalMatrix.ScaleX,
        };
        using var pointStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.YellowGreen,
            StrokeWidth = _pointRadius / 3.5f / canvas.TotalMatrix.ScaleX,
        };
        using var pointFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.YellowGreen,
        };

        if (Points == null)
            return;

        for(var i = 0; i < ContoursIndices.Count; i++)
        {
            var path = new SKPath();
            for (var j = 0; j < ContoursIndices[i].Count; j++)
            {
                if (j == 0)
                    path.MoveTo(Points[ContoursIndices[i][j]]);
                else
                    path.LineTo(Points[ContoursIndices[i][j]]);
            }
            path.Close();

            canvas.DrawPath(path, i == 0 ? fill : hole);

            if (i == SelectedContourIndex)
                stroke.Color = SKColors.Coral;
            else
                stroke.Color = o.SectionOutlineColor;
            canvas.DrawPath(path, stroke);
        }

        for (var i = 0; i < Points.Count; i++)
        {
            if (i == SelectedPointIndex)
            {
                pointStroke.Color = SKColors.Orange;
                pointFill.Color = SKColors.Orange;
            }
            else
            {
                pointStroke.Color = SKColors.YellowGreen;
                pointFill.Color = SKColors.YellowGreen;
            }

            canvas.DrawCircle(Points[i], _pointRadius / canvas.TotalMatrix.ScaleX, pointStroke);
            canvas.DrawCircle(Points[i], _pointRadius / 1.9f / canvas.TotalMatrix.ScaleX, pointFill);
        }
    }

    private void DrawBackground(SKCanvas canvas)
    {
        if(_gridCellSize == 0) return;

        var bounds = canvas.LocalClipBounds;
        using var line = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(230, 230, 230),
            StrokeWidth = 2f / canvas.TotalMatrix.ScaleX,
            BlendMode = SKBlendMode.DstOver,
        };


        var c = (int)MathF.Floor(bounds.Left / _gridCellSize);
        var sp = c * _gridCellSize;
        for (var i = sp; i < bounds.Right; i += _gridCellSize)
        {
            canvas.DrawLine(new SKPoint(i, bounds.Top), new SKPoint(i, bounds.Bottom), line);
        }
        c = (int)MathF.Floor(bounds.Top / _gridCellSize);
        sp = c * _gridCellSize;
        for (var i = sp; i < bounds.Bottom; i += _gridCellSize)
        {
            canvas.DrawLine(new SKPoint(bounds.Left, i), new SKPoint(bounds.Right, i), line);
        }
        
    }

    private enum SectionDesignerState
    {
        Normal,
        AddPoint,
        AddHole,
    }
}