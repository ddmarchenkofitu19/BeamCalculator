using BeamCalculator.Helpers.Geometry;

namespace BeamCalculator.Models;


public static class SectionParamsCalculator
{
    public static CalculationResults CalculateRoundTubing(double outerRadius, double innerRadius)
    {
        var results = new CalculationResults();

        results.Area = Math.PI * (outerRadius * outerRadius - innerRadius * innerRadius) / 4;
        var m = Math.PI * (Math.Pow(outerRadius, 4) - Math.Pow(innerRadius, 4)) / 64;

        results.InertiaMomentY = m;
        results.InertiaMomentZ = m;
        results.InertiaAxesAngle = 0;
        results.MainInertiaMomentY = m;
        results.MainInertiaMomentZ = m;

        results.InertiaRadiusY = Math.Sqrt(m / results.Area);
        results.InertiaRadiusZ = results.InertiaRadiusY;

        var n = -(m * m / outerRadius);
        results.SectionCore.Add(new Point(n, n));

        return results;
    }

    public static CalculationResults CalculateFragmentedSection(List<Fragment> fragments, List<List<Point>> contours)
    {
        var results = new CalculationResults();

        double accZ = 0, accY = 0;
        foreach (var frag in fragments)
        {
            results.Area += frag.Area;
            accY += frag.Area * frag.Center.X;
            accZ += frag.Area * frag.Center.Y;
        }

        // calculate cross section gravity center
        Point massCenter = new Point(accY / results.Area, accZ / results.Area);
        results.MassCenter = massCenter;

        // Calculate innertial moment by Z and Y axes and centrifugal innertial moment
        foreach (var frag in fragments)
        {
            // offsets from section gravity center
            var x0 = frag.Center.X - massCenter.X;
            var y0 = frag.Center.Y - massCenter.Y;
            // innertial moments
            results.InertiaMomentY += frag.Area * y0 * y0 + frag.InertMomentY;
            results.InertiaMomentZ += frag.Area * x0 * x0 + frag.InertMomentZ;
            results.InertiaMomentYZ += frag.Area * x0 * y0 + frag.InertMomentYZ;
        }

        var ev = results.InertiaMomentY + results.InertiaMomentZ; // innertial moment sum
        var ew = (results.InertiaMomentY - results.InertiaMomentZ) / 2; // innertial moment halfsub
        
        results.MainInertiaMomentZ = ev * 0.5f - Math.Sqrt(ew * ew + results.InertiaMomentYZ * results.InertiaMomentYZ);
        results.MainInertiaMomentY = ev - results.MainInertiaMomentZ;

        // calculate angle between base axes and inertial axes 
        ew = results.MainInertiaMomentZ - results.InertiaMomentY; // innertial moment sub
        if (Math.Abs(ew / ev) < 1e-7 || Math.Abs(results.InertiaMomentYZ / ev) < 1e-7)
        {
            if (results.InertiaMomentY >= results.InertiaMomentZ)
            {
                results.InertiaAxesAngle = 0;
            }
            else
            {
                results.InertiaAxesAngle = MathF.PI / 2;
            }
        }
        else
            results.InertiaAxesAngle = Math.Atan(results.InertiaMomentYZ / ew);

        // inertia radius
        results.InertiaRadiusY = Math.Sqrt(results.MainInertiaMomentY / results.Area);
        results.InertiaRadiusZ = Math.Sqrt(results.MainInertiaMomentZ / results.Area);

        // section core
        var convex = GeometryUtils.PolygonConvexHull(contours[0].ToArray());

        foreach (var p in convex) 
        {
            var x = -(results.InertiaRadiusZ * results.InertiaRadiusZ / (p.X - massCenter.X));
            var y = -(results.InertiaRadiusY * results.InertiaRadiusY / (p.Y - massCenter.Y));

            results.SectionCore.Add(new Point(x, y));
        }

        return results;
    }
}
