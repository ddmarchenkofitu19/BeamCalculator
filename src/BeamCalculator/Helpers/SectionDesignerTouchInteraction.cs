using SkiaSharp;

namespace BeamCalculator.Helpers;


public class SectionDesignerTouchInteraction
{
    private readonly int _touchedPointIndex;
    private readonly SKPoint _start;
    private SKPoint _current;

    public int TouchedPointIndex { get => _touchedPointIndex; }

    public SKPoint StartPos => _start;

    public SKPoint CurrentPos { get => _current; set => _current = value; }


    public SectionDesignerTouchInteraction(SKPoint startPosition)
    {
        _start = startPosition;
        _current = startPosition;
        _touchedPointIndex = -1;
    }

    public SectionDesignerTouchInteraction(SKPoint startPosition, int touchedPointIndex)
    {
        _start = startPosition;
        _current = startPosition;
        _touchedPointIndex = touchedPointIndex;
    }
}
