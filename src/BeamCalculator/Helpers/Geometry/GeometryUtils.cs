using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Collections.Generic;
using System.Linq;

namespace BeamCalculator.Helpers.Geometry;


public static class GeometryUtils
{
    public static bool IsContourClockwiseOriented(List<Point> contour)
    {
        // wiki:Curve_Orientation#OrientationOfSimplePolygon
        var cInd = FindCornerPoint(contour);

        var a = contour[cInd - 1 >= 0 ? cInd - 1 : contour.Count - 1];
        var b = contour[cInd];
        var c = contour[cInd + 1 < contour.Count ? cInd + 1 : 0];

        var detOrient = (b.X * c.Y + a.X * b.Y + a.Y * c.X) - (a.Y * b.X + b.Y * c.X + a.X * c.Y);
        System.Diagnostics.Debug.WriteLine($"cInd: {cInd}, orient: {detOrient}");
        if (detOrient < 0)
            return true;

        return false;
    }

    private static int FindCornerPoint(List<Point> contour)
    {
        // finding point along one edge of bounding box,
        // in this case among edges with smallest Y, choose one with smallest X
        int cornerInd = -1;
        double minY = Double.MaxValue;
        double minXAtMinY = Double.MaxValue;

        for(var i = 0; i < contour.Count; i++)
        {
            var p = contour[i];

            if (p.Y > minY) 
                continue;
            if (p.Y == minY)
                if (p.X >= minXAtMinY)
                    continue;

            cornerInd = i;
            minY = p.Y;
            minXAtMinY = p.X;
        }

        return cornerInd;
    }

    public static bool LineRectContainsPoint(Point lineP1, Point lineP2, Point point)
        => LineRectContainsPoint(lineP1, lineP2, point, 0);

    public static bool LineRectContainsPoint(Point lineP1, Point lineP2, Point point, double tolerance)
    {
        return point.X <= Math.Max(lineP1.X, lineP2.X) && point.X >= Math.Min(lineP1.X, lineP2.X) &&
            point.Y <= Math.Max(lineP1.Y, lineP2.Y) && point.Y >= Math.Min(lineP1.Y, lineP2.Y);
    }

    public static List<double> DistancesToContourEdges(List<Point> contour, Point point)
    {
        List<double> dist = new List<double>();
        
        for (var i = 0; i < contour.Count; i++)
        {
            var j = (i + 1) % contour.Count;

            dist.Add(contour[i].Distance(point) + contour[j].Distance(point));
            //System.Diagnostics.Debug.WriteLine($"edge {i}: {contour[i].Distance(point) + contour[j].Distance(point)}");
        }

        return dist;
    }

    public static IEnumerable<int> EdgesIndicesOrderedByDistanceToPoint(List<Point> contour, Point point)
    {
        var distances = DistancesToContourEdges(contour, point);

        return Enumerable.Range(0, contour.Count).OrderBy(i => distances[i]);
    }


    public static bool PointIsInsideContour(List<Point> contour, Point point)
    {
        // "Inclusion of a Point in a Polygon" by Den Sunday
        int wn = 0;
          
        int j;
        for(var i = 0; i < contour.Count; i++)
        {
            j = (i + 1) % contour.Count;

            if (contour[i].Y < point.Y)
            {
                if (contour[j].Y > point.Y)
                    if (PointIsLeftOfLine(contour[i], contour[j], point) == PointRelativeToLine.IsRight)
                        ++wn;
            }
            else 
            {
                if (contour[j].Y < point.Y)
                    if (PointIsLeftOfLine(contour[i], contour[j], point) >= PointRelativeToLine.OnLine)
                        --wn;
            }
        }

        return wn != 0;
    }

    public static PointRelativeToLine PointIsLeftOfLine(Point lineP1, Point lineP2, Point point)
    {
        // "Area of Triangles and Polygons" by Den Sunday
        var n = (lineP2.X - lineP1.X) * (point.Y - lineP1.Y) - (point.X - lineP1.X) * (lineP2.Y - lineP1.Y);

        if (n > 0)
            return PointRelativeToLine.IsLeft;
        else if (n == 0)
            return PointRelativeToLine.OnLine;
        else
            return PointRelativeToLine.IsRight;
    }

    public static Point[] PolygonConvexHull(Point[] points)
    {
        // A.M. Andrew Monotone Chain algorithm implementation from Dan Sanday "The Convex hull of  Planar Point Set" aka A10: hull-1

        var convex = new Stack<Point>();
        int bot = 0;
        int i;

        // presorting points array by increasing X and then increasing Y
        var p = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();

        // get the indices of points with min x-coord and min|max y-coords
        int minmin = 0, minmax;
        double xmin = p[minmin].X;
        for (i = 0; i < p.Length; i++)
            if (p[i].X != xmin)
                break;
        minmax = i - 1;

        // degenerate case: all x-coords == xmin
        if(minmax == p.Length-1)
        {
            convex.Push(p[minmin]);
            if (p[minmax].Y != p[minmin].Y)
                convex.Push(p[minmax]);

            return convex.ToArray();
        }

        // get the indices of points with max x-coord and min|max y-coords
        int maxmin, maxmax = p.Length - 1;
        double xmax = p[maxmax].X;
        for (i = maxmax - 1; i >= 0; i--)
            if (p[i].X != xmax)
                break;
        maxmin = i + 1;

        // compute the Lower Hull using the stack
        convex.Push(p[minmin]); // push first point onto stack
        i = minmax;
        while (++i <= maxmin)
        {
            // the lower line joins p[minmin] with p[maxmin]
            if (PointIsLeftOfLine(p[minmin], p[maxmin], p[i]) >= 0 && i < maxmin)
                continue; // ignore p[i] above or on the lower line

            while(convex.Count > 1) // there are at least 2 points on the stack
            {
                // test if [i] is left of the line at the stack top
                if (PointIsLeftOfLine(convex.ElementAt(^2), convex.ElementAt(^1), p[i]) > 0)
                    break; // [i] ia a new hull vertex
                else
                    convex.Pop(); // pop top point 
            }

            convex.Push(p[i]); // push p[i] onto stack
        }

        // compute the Upper Hull on the stack above the bottom hull
        if (maxmax != maxmin)
            convex.Push(p[maxmax]);
        bot = convex.Count - 1;
        i = maxmin;
        while(--i >= minmax)
        {
            // the upper line joins p[maxmax] with p[minmax]
            if (PointIsLeftOfLine(p[maxmax], p[minmax], p[i]) > 0 && i > minmax)
                continue; // ignore p[i] below or on the upper line

            while (convex.Count - 1 > bot) // at least two points on the upper stack
            {
                // test if p[i] is lefy of the line at the stack top
                if (PointIsLeftOfLine(convex.ElementAt(^2), convex.ElementAt(^1), p[i]) > 0)
                    break; // p[i] ia a new hull vertex
                else
                    convex.Pop(); // pop top point 
            }

            convex.Push(p[i]); // push p[i] onto stack
        }

        return convex.ToArray();
    }
}

public enum PointRelativeToLine
{
    IsRight = -1,
    OnLine = 0,
    IsLeft = 1,
}