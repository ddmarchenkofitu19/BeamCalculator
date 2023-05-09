namespace BeamCalculator.Helpers.Geometry;


public class LinesSegmentsIntersection
{
    private Point p11;
    private Point p12;
    private Point p21;
    private Point p22;
    private double tolerance;


    public IntersectionType Intersection { get; private set; }
    
    public bool HasIntersection { get; private set; }


	public LinesSegmentsIntersection(Point line1P1, Point line1P2, Point line2P1, Point line2P2) 
        : this(line1P1, line1P2, line2P1, line2P2, 0)
	{ }

    public LinesSegmentsIntersection(Point line1P1, Point line1P2, Point line2P1, Point line2P2, double tolerance)
    {
        p11 = line1P1;
        p12 = line1P2;
        p21 = line2P1;
        p22 = line2P2;
        this.tolerance = tolerance;

        CheckSegmentsIntersection();
    }


    private void CheckSegmentsIntersection()
    {
        HasIntersection = false;

        // positions of line1 points relative to line2
        var o11 = GeometryUtils.PointIsLeftOfLine(p21, p22, p11);
        var o12 = GeometryUtils.PointIsLeftOfLine(p21, p22, p12);

        // positions of line2 points relative to line1
        var o21 = GeometryUtils.PointIsLeftOfLine(p11, p12, p21);
        var o22 = GeometryUtils.PointIsLeftOfLine(p11, p12, p22);

        // two collinear lines
        if (o21 == PointRelativeToLine.OnLine && o22 == PointRelativeToLine.OnLine)
        {
            var maxL1 = Math.Max(p11.X, p12.X);
            var minL1 = Math.Min(p11.X, p12.X);
            var maxL2 = Math.Max(p21.X, p22.X);
            var minL2 = Math.Min(p21.X, p22.X);
            // two collinear lines with overlapping
            if (maxL1 - minL2 > 0 && maxL2 - minL1 > 0)
            {
                Intersection = IntersectionType.CollinearWithOverlapping;
                return;
            }

            // two touching collinear lines
            if (maxL1 - minL2 == 0 || maxL2 - minL1 == 0)
            {
                Intersection = IntersectionType.CollinearWithTouching;
                return;
            }
        
            Intersection = IntersectionType.Collinear;
            return;
        }
        
        // points intersects inside segment bounds
        if((int)o11 * (int)o12 < 0 && (int)o21 * (int)o22 < 0)
        {
            Intersection = IntersectionType.IntersectInsideSegmentsBounds;
            return;
        }

        if((o11 == PointRelativeToLine.OnLine && GeometryUtils.LineRectContainsPoint(p21, p22, p11)) ||
           (o12 == PointRelativeToLine.OnLine && GeometryUtils.LineRectContainsPoint(p21, p22, p12)) ||
           (o21 == PointRelativeToLine.OnLine && GeometryUtils.LineRectContainsPoint(p11, p12, p21)) ||
           (o22 == PointRelativeToLine.OnLine && GeometryUtils.LineRectContainsPoint(p11, p12, p22)))
        {
            Intersection = IntersectionType.IntersectOnSegmentBoundary;
            return;
        }

        var dx1 = p11.X - p12.X;
        var dy1 = p11.Y - p12.Y;
        var dx2 = p21.X - p22.X;
        var dy2 = p21.Y - p22.Y;
        var cosAngle = Math.Abs((dx1 * dx2 + dy1 * dy2) / Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2)));
        // parallel lines
        if (cosAngle == 1)
        {
            Intersection = IntersectionType.Parallel;
            return;
        }

        Intersection = IntersectionType.IntersectOutsideSegmentsBounds;
    }

    private Point GetIntersectionPoint()
    {
        double x, y;

        // first line is vertical, p11x == p12x
        // slope will be infinity
        if ((tolerance == 0 && Math.Abs(p11.X - p12.X) == 0) || (tolerance > 0 && Math.Abs(p11.X - p12.X) < tolerance))
        {
            // calculate slope for second line
            double m2 = (p22.Y - p21.Y) / (p22.X - p21.X);
            double c2 = -m2 * p21.X + p21.Y;

            x = p11.X;
            y = c2 + m2 * p11.X;
        }
        // second line is vertical, p21x == p22x
        // slope will be infinity
        else if ((tolerance == 0 && Math.Abs(p21.X - p22.X) == 0) || (tolerance > 0 && Math.Abs(p21.X - p22.X) < tolerance))
        {
            // calculate slope for first line
            double m1 = (p12.Y - p11.Y) / (p12.X - p11.X);
            double c1 = -m1 * p11.X + p11.Y;

            x = p21.X;
            y = c1 + m1 * p21.X;
        }
        // first & second line are not vertical
        else
        {
            // calculate slope for first line
            double m1 = (p12.Y - p11.Y) / (p12.X - p11.X);
            double c1 = -m1 * p11.X + p11.Y;

            // calculate slope for second line
            double m2 = (p22.Y - p21.Y) / (p22.X - p21.X);
            double c2 = -m2 * p21.X + p21.Y;

            // calculate intersection point
            x = (c1 - c2) / (m2 - m1);
            y = c2 + m2 * x;

            // verify by original line 1 & 2 equations otherwise x and y can be infinity or NaN
            //if(!(Math.Abs(-m1 * x + y - c1) < tolerance &&
            //    Math.Abs(-m2 * x + y - c2) < tolerance))
            //{
            //    return default(Point);
            //}
        }

        return new Point(x, y);
    }
}
public enum IntersectionType
{
    Parallel,
    
    Collinear,
    CollinearWithOverlapping,
    CollinearWithTouching,

    IntersectOutsideSegmentsBounds,
    IntersectInsideSegmentsBounds,
    IntersectOnSegmentBoundary
}