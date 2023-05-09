using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace BeamCalculator.Helpers.Drawing;


public static class DrawingExtensions
{
    public static SKPoint Mult(this SKPoint p1, SKPoint p2)
    {
        return new SKPoint(p1.X * p2.X, p1.Y * p2.Y);
    }

    public static SKPoint InvertY(this SKPoint p)
    {
        return new SKPoint(p.X, -p.Y);
    }

    public static Point ToMauiPoint(this SKPoint p)
    {
        return new Point((double)new decimal(p.X), (double)new decimal(p.Y));
    }

    public static SKPoint ClampToBounds(this SKPoint p, SKRect rect)
    {
        p.X = Math.Clamp(p.X, rect.Left, rect.Right);
        p.Y = Math.Clamp(p.Y, rect.Top, rect.Bottom);
        return p;
    }

    public static void DrawArrow(this SKCanvas canvas, SKPoint start, SKPoint end, float pLength, float pTheta, SKPaint paint, ArrowDirection direction = ArrowDirection.StartToEnd)
    {
        if(direction == ArrowDirection.EndToStart)
        {
            canvas.DrawArrow(end, start, pLength, pTheta, paint);
            return;
        }

        if(direction == ArrowDirection.Both)
        {
            canvas.DrawArrow(end, start, pLength, pTheta, paint);
        }

        var v = end - start;
        var n = new SKPoint(v.X / v.Length, v.Y / v.Length);
        n.X *= v.Length - (pLength * 0.2f);
        n.Y *= v.Length - (pLength * 0.2f);
        var end1 = n + start;

        // draw errow line
        paint.Style = SKPaintStyle.Stroke;
        paint.IsAntialias = false;
        canvas.DrawLine(start, end1, paint);

        pTheta *= 0.5f;
        pTheta *= MathF.PI / 180;
        var t = MathF.Atan2(end.Y - start.Y, end.X - start.X);
        var arrowP1 = new SKPoint(
            end.X - pLength * MathF.Cos(t + pTheta),
            end.Y - pLength * MathF.Sin(t + pTheta));
        var arrowp2 = new SKPoint(
            end.X - pLength * MathF.Cos(t - pTheta),
            end.Y - pLength * MathF.Sin(t - pTheta));

        var path = new SKPath();
        path.MoveTo(end);
        path.LineTo(arrowP1);
        path.LineTo(arrowp2);
        path.LineTo(end);

        paint.Style = SKPaintStyle.Fill;
        paint.IsAntialias = false;
        canvas.DrawPath(path, paint);
    }

    public enum ArrowDirection
    {
        StartToEnd,
        EndToStart,
        Both
    }

    public static List<SKPoint> ToSKPointList(this List<Point> points)
    {
        return points.Select(p => p.ToSKPoint()).ToList();
    }
}
