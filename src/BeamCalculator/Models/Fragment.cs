namespace BeamCalculator.Models;


public struct Fragment
{
    private Point[] points;

    public Point P1 { get { return points[0]; } set { points[0] = value; } }
    public Point P2 { get { return points[1]; } set { points[1] = value; } }
    public Point P3 { get { return points[2]; } set { points[2] = value; } }

    public double Width =>
        points.Select(p => p.X).Max() - points.Select(p => p.X).Min();

    public double Height => 
        points.Select(p => p.Y).Max() - points.Select(p => p.Y).Min();

    public double Top => points.Select(p => p.Y).Max();
    public double Bottom => points.Select(p => p.Y).Min();
    public double Right => points.Select(p => p.X).Max();
    public double Left => points.Select(p => p.X).Min();

    public bool IsEmpty
    {
        get
        {
            if (P1.IsEmpty)
                if (P2.IsEmpty)
                    return P3.IsEmpty;

            return false;
        }
    }

    public Point Center => new Point((P1.X + P2.X + P3.X) / 3, (P1.Y + P2.Y + P3.Y) / 3);

    public double Area
    {
        get
        {
            double determ = (P1.X - P3.X) * (P2.Y - P3.Y) - (P2.X - P3.X) * (P1.Y - P3.Y);
            return 0.5 * Math.Abs(determ);
        }
    }

    public double InertMomentY
    {
        get
        {
            double sum = 0;

            sum += Math.Pow(Center.Y - (P1.Y + P2.Y) / 2, 2);
            sum += Math.Pow(Center.Y - (P2.Y + P3.Y) / 2, 2);
            sum += Math.Pow(Center.Y - (P1.Y + P3.Y) / 2, 2);
            return Area*sum / 3;
        }
    }

    public double InertMomentZ
    {
        get
        {
            double sum = 0;

            sum += Math.Pow(Center.X - (P1.X + P2.X) / 2, 2);
            sum += Math.Pow(Center.X - (P2.X + P3.X) / 2, 2);
            sum += Math.Pow(Center.X - (P1.X + P3.X) / 2, 2);
            return Area * sum / 3;
        }
    }

    public double InertMomentYZ
    {
        get
        {
            double sum = 0;

            sum += ((P1.X + P2.X) / 2 - Center.X) * ((P1.Y + P2.Y) / 2 - Center.Y);
            sum += ((P2.X + P3.X) / 2 - Center.X) * ((P2.Y + P3.Y) / 2 - Center.Y);
            sum += ((P1.X + P3.X) / 2 - Center.X) * ((P1.Y + P3.Y) / 2 - Center.Y);
            return Area * sum / 3;
        }
    }


    public Fragment(Point point1, Point point2, Point point3)
    { 
        points = new Point[3];
        points[0] = point1;
        points[1] = point2;
        points[2] = point3;
    }
    public Fragment(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        points = new Point[3];
        points[0] = new Point(x1, y1);
        points[1] = new Point(x2, y2);
        points[2] = new Point(x3, y3);
    }
}
