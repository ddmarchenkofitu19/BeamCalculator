using BeamCalculator.Models;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using MauiPoint = Microsoft.Maui.Graphics.Point;

namespace BeamCalculator.Helpers;


public static class GeometryExtensions
{
    public static List<Vertex> ToVertexList(List<MauiPoint> points)
     => points.Select(p => p.ToVertex()).ToList();
    
    public static List<Fragment> ToFragmentList(this ICollection<Triangle> triangles)
        => triangles.Select(t => t.ToFragment()).ToList();
    
    public static Fragment ToFragment(this Triangle tr)
        => new Fragment(tr.GetVertex(0).ToMauiPoint(), tr.GetVertex(1).ToMauiPoint(), tr.GetVertex(2).ToMauiPoint());
    
    public static MauiPoint ToMauiPoint(this Vertex v)
        => new MauiPoint(v.X, v.Y);
    
    public static Vertex ToVertex(this MauiPoint p)
        => new Vertex(p.X, p.Y);

    public static bool CompareWithTolerance(this MauiPoint p1, MauiPoint p2, double tolerance)
    {
        return p1.X - tolerance <= p2.X && p1.X + tolerance >= p2.X &&
               p1.Y - tolerance <= p2.Y && p1.Y + tolerance >= p2.Y;
    }
}
