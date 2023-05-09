using BeamCalculator.Helpers;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TNETPoint = TriangleNet.Geometry.Point;
using MAUIPoint = Microsoft.Maui.Graphics.Point;

namespace BeamCalculator.Models;


public class Triangulator
{
    public Triangulator()
    {

    }

    public List<Fragment> Triangulate(List<MAUIPoint> points, List<List<int>> contours)
    {
        var p = new Polygon();
        p.Add(new Contour(contours[0].Select(i => new Vertex(points[i].X, points[i].Y)), 1));
        
        for (var i = 1; i < contours.Count; i++)
        {
            var vertices = contours[i].Select(i => new Vertex(points[i].X, points[i].Y));
            p.Add(new Contour(vertices, i+1), GetPointInsideContour(vertices.ToList()));
        }
        
        var options = new ConstraintOptions() { ConformingDelaunay = true };
        var quality = new QualityOptions() { MinimumAngle = 18 };
        
        var mesh = p.Triangulate(options, quality);
        
        return mesh.Triangles.ToFragmentList();
    }

    private TNETPoint GetPointInsideContour(List<Vertex> contour)
    {
        // getting contour bounds
        double xmin = Double.PositiveInfinity, xmax = Double.NegativeInfinity;
        double ymin = Double.PositiveInfinity, ymax = Double.NegativeInfinity;
        for(var i = 0; i < contour.Count; i++)
        {
            xmin = Math.Min(xmin, contour[i].X);
            xmax = Math.Max(xmax, contour[i].X);
            ymin = Math.Min(ymin, contour[i].Y);
            ymax = Math.Max(ymax, contour[i].Y);
        }
        var bounds = new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
    
    
        // find vertical line in bounds and without points located on it 
        double lineX = 0;
        bool finded = false;
        var cx = bounds.Left + bounds.Width / 2;
        //from center to right bound
        for(double x = cx; x < bounds.Right; x += 0.001)
        {
            if(!contour.Exists(p => p.X == x)) 
            {
                lineX = x;
                finded = true;
                break;
            }
        }
        //if not finded => from center to left bound
        if (!finded)
        {
            for (double x = cx; x > bounds.Left; x -= 0.001)
            {
                if (!contour.Exists(p => p.X == x))
                {
                    lineX = x;
                    break;
                }
            }
        }
    
    
        // find intersections of line with edges
        var lineP1 = new TNETPoint(lineX, bounds.Bottom - 10);
        var lineP2 = new TNETPoint(lineX, bounds.Top + 10);
        List<double> intsects = new List<double>();
        for(var i = 0; i < contour.Count; i++)
        {
            var p1 = contour[i];
            var p2 = contour[i + 1 < contour.Count ? i + 1 : 0];
    
            //if edge has intersection
            if((lineX > p1.X && lineX < p2.X) || (lineX < p1.X && lineX > p2.X))
            {
                var intrsectY = p1.Y + ((lineX - p1.X) * (p2.Y - p1.Y)) / (p2.X - p1.X); 
                intsects.Add(intrsectY);
            }
        }
    
    
        // sort intersections list by point.Y in ascending order
        intsects.Order();
    
        // take midpoint beetween first two points
        var mid = new TNETPoint(lineX, (intsects[0] + intsects[1]) / 2);
    
        return mid;
    }
}
