using BeamCalculator.Helpers;
using BeamCalculator.Models.Section;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace BeamCalculator.Components;


public partial class CrossSectionVisualizer : ContentView, ISectionVisualizer
{
    #region BindableProperty

    public static readonly BindableProperty SectionProperty =
       BindableProperty.Create(
           propertyName: nameof(Section),
           returnType: typeof(SectionModel),
           declaringType: typeof(CrossSectionVisualizer),
           defaultBindingMode: BindingMode.OneWay,
           propertyChanged: SectionPropertyChanged);

    public static readonly BindableProperty ErrorProperty =
       BindableProperty.Create(
           propertyName: nameof(Error),
           returnType: typeof(bool),
           declaringType: typeof(CrossSectionVisualizer),
           defaultValue: false,
           defaultBindingMode: BindingMode.OneWay,
           propertyChanged: ErrorPropertyChanged);

    public SectionModel Section
    {
        get { return (SectionModel)GetValue(SectionProperty); }
        set { SetValue(SectionProperty, value); }
    }

    public bool Error
    {
        get { return (bool)GetValue(ErrorProperty); }
        set { SetValue(ErrorProperty, value); }
    }

    private static void SectionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var component = (CrossSectionVisualizer)bindable;
    
        component.InvalidateResetScale();
    }

    private static void ErrorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var component = (CrossSectionVisualizer)bindable;
    }

    #endregion


    private readonly UserSettings settings;

    private bool firstDraw = true;
    private SKMatrix touchTransforms;
    private Dictionary<long, SKPoint> touches = new Dictionary<long, SKPoint>();


    public CrossSectionVisualizer()
	{
		InitializeComponent();
        container.BindingContext = this;

        DelayedRedraw();

        settings = MauiProgram.Services.GetService<UserSettings>();
        settings.PropertyChanged += Settings_PropertyChanged;
    }

    public void Invalidate()
        => canvasView.InvalidateSurface();

    public void InvalidateResetScale()
    {
        firstDraw = true;
        canvasView.InvalidateSurface();
    }


    private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(settings.SectionOutlineColor):
            case nameof(settings.SectionFillColor):
            case nameof(settings.SectionFillOpacity):
                canvasView.InvalidateSurface();
                break;
        }
    }

    private void DelayedRedraw()
    {
        // fixing error with empty canvas on windows when page opened
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            this.Dispatcher.DispatchDelayed(
                new TimeSpan(0, 0, 0, 0, milliseconds: 300),
                () => this.canvasView.InvalidateSurface());
        }
    }

    private void AdjustSection_Tapped(object sender, TappedEventArgs e)
        => this.InvalidateResetScale();

    private void OnTouch(object sender, SKTouchEventArgs e)
    {
        //System.Diagnostics.Debug.WriteLine(
        //    $"id: {e.Id} {e.ActionType} loc: {e.Location} cont: {e.InContact}");

        SKMatrix scaleMatr;
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                touches.TryAdd(e.Id, e.Location);
                e.Handled = true;
                break;

            case SKTouchAction.Cancelled:
            case SKTouchAction.Released:
                if (touches.ContainsKey(e.Id))
                {
                    touches.Remove(e.Id);
                    e.Handled = true;
                }
                break;

            case SKTouchAction.Moved:
                if (touches.ContainsKey(e.Id))
                {
                    if (touches.Count == 1)
                    {
                        var curP = e.Location;
                        var prevP = touches[e.Id];

                        touchTransforms.TransX += curP.X - prevP.X;
                        touchTransforms.TransY += curP.Y - prevP.Y;

                        canvasView.InvalidateSurface();
                    }
                    else if (touches.Count == 2)
                    {
                        long[] keys = new long[touches.Count];
                        touches.Keys.CopyTo(keys, 0);

                        int pivotInd = (keys[0] == e.Id) ? 1 : 0;

                        var pivotP = touches[keys[pivotInd]];
                        var prevP = touches[e.Id];
                        var newP = e.Location;

                        var oldVect = prevP - pivotP;
                        var newVect = newP - pivotP;

                        var scale = newVect.Length / oldVect.Length;

                        if (!float.IsNaN(scale) && !float.IsInfinity(scale))
                        {
                            scaleMatr = SKMatrix.CreateScale(scale, scale, pivotP.X, pivotP.Y);

                            touchTransforms = touchTransforms.PostConcat(scaleMatr);
                            canvasView.InvalidateSurface();
                        }
                    }

                    touches[e.Id] = e.Location;
                    e.Handled = true;
                }
                break;

            case SKTouchAction.WheelChanged:
                var dt = 1 + e.WheelDelta * 0.001f;
                scaleMatr = SKMatrix.CreateScale(dt, dt, e.Location.X, e.Location.Y);

                touchTransforms = touchTransforms.PostConcat(scaleMatr);
                canvasView.InvalidateSurface();
                break;
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;
        var bounds = canvas.LocalClipBounds;

        if (bounds.Width == 0 || bounds.Height == 0 || Section == null)
            return;

        canvas.Clear(SKColors.Transparent);

        // calculate starting matr with translation to center and scale depending on section size
        if (firstDraw)
        {
            var widthRatio = (bounds.Width * 0.45f) / Section.Width;
            var heightRatio = (bounds.Height * 0.45f) / Section.Height;
            var scale = (float)Math.Min(widthRatio, heightRatio);
            scale = Double.IsInfinity(scale) ? 1 : scale;

            touchTransforms = SKMatrix.CreateScaleTranslation(scale, scale, bounds.MidX, bounds.MidY);
            firstDraw = false;
        }

        var savedMatr = canvas.TotalMatrix;
        canvas.SetMatrix(touchTransforms);

        Section.Draw(canvas, new SKRect(-bounds.Width / 2, -bounds.Height / 2, bounds.Width / 2, bounds.Height / 2));

        DrawBackground(canvas);

        canvas.SetMatrix(savedMatr);
    }

    private void DrawBackground(SKCanvas canvas)
    {
        var bounds = canvas.LocalClipBounds;

        using var line = new SKPaint
        {
            Color = SKColors.LightGray,
            StrokeWidth = 2f / canvas.TotalMatrix.ScaleX,
            BlendMode = SKBlendMode.DstOver,
        };

        // Draw X and Y axes lines
        line.Color = new SKColor(135, 135, 135);
        canvas.DrawLine(
            new SKPoint(0, bounds.Top),
            new SKPoint(0, bounds.Bottom),
            line);
        canvas.DrawLine(
            new SKPoint(bounds.Left, 0),
            new SKPoint(bounds.Right, 0),
            line);


        // Draw adaptive graph paper pattern
        float smCellSize = 1; // base cell size
        var maxSmallCellsInBoundsCount = 30;
        var smallCellsInBigCount = 10;

        // calc adaptive cell size
        var minDim = MathF.Min(bounds.Width, bounds.Height);
        var exp = MathF.Log(minDim / maxSmallCellsInBoundsCount, smallCellsInBigCount);
        var n = MathF.Ceiling(exp);
        var sizeFactor = MathF.Pow(smallCellsInBigCount, n);
        smCellSize *= sizeFactor != 0 ? sizeFactor : 1;

        int c;
        float sp;
        // Draw big cells lines
        var xlCellSize = smCellSize * smallCellsInBigCount; // calc big cell size
        line.Color = new SKColor(185, 185, 185);
        // vertival
        c = (int)MathF.Floor(bounds.Left / xlCellSize);
        sp = c * xlCellSize;
        for (var x = sp; x <= bounds.Right; x += xlCellSize)
        {
            canvas.DrawLine(
                new SKPoint(x, bounds.Top),
                new SKPoint(x, bounds.Bottom),
                line);
        }
        // horizontal
        c = (int)MathF.Floor(bounds.Top / xlCellSize);
        sp = c * xlCellSize;
        for (var y = sp; y <= bounds.Bottom; y += xlCellSize)
        {
            canvas.DrawLine(
                new SKPoint(bounds.Left, y),
                new SKPoint(bounds.Right, y),
                line);
        }


        // Draw small cells lines
        // vertical
        line.Color = new SKColor(230, 230, 230);
        c = (int)MathF.Floor(bounds.Left / smCellSize);
        sp = c * smCellSize;
        for (var x = sp; x <= bounds.Right; x += smCellSize)
        {
            canvas.DrawLine(
                new SKPoint(x, bounds.Top),
                new SKPoint(x, bounds.Bottom),
                line);
        }
        // horizontal
        c = (int)MathF.Floor(bounds.Top / smCellSize);
        sp = c * smCellSize;
        for (var y = sp; y <= bounds.Bottom; y += smCellSize)
        {
            canvas.DrawLine(
                new SKPoint(bounds.Left, y),
                new SKPoint(bounds.Right, y),
                line);
        }
        

        ////Draw X and Y axes
        //// x axis
        //using var axesStroke = new SKPaint
        //{
        //    Color = SKColors.Red,
        //    StrokeWidth = 2,
        //    IsAntialias = true,
        //};
        //canvas.DrawArrow(
        //    new SKPoint(bounds.Left, bounds.MidY),
        //    new SKPoint(bounds.Right, bounds.MidY),
        //    25,
        //    27,
        //    axesStroke);

        //// y axis
        //axesStroke.Color = SKColors.Green;
        //canvas.DrawArrow(
        //    new SKPoint(bounds.MidX, bounds.Bottom),
        //    new SKPoint(bounds.MidX, bounds.Top),
        //    25,
        //    27,
        //    axesStroke);
    }

}

public interface ISectionVisualizer
{
    public void Invalidate();

    public void InvalidateResetScale();
}