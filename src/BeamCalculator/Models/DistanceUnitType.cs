namespace BeamCalculator.Models;


public enum DistanceUnitType
{
    Meters,
    Centimeters,
    Millimeters,
    Inches
}

public static class DistanceUnitsTypeExtensions
{
    private static readonly double[,] conversionFactors = new double[4, 4]
    {
        {     1,   0.01,   0.001, 0.0254 },
        {   100,      1,     0.1,   2.54 },
        {  1000,     10,       1,   25.4 },
        { 39.37, 0.3937, 0.03937,      1 }
    };

    public static string ToUserFriendlyString(this DistanceUnitType val)
    {
        switch (val)
        {
            case DistanceUnitType.Meters:
                return "m";
            case DistanceUnitType.Centimeters:
                return "cm";
            case DistanceUnitType.Millimeters:
                return "mm";
            case DistanceUnitType.Inches:
                return "in";
        }

        return val.ToString();
    }

    public static double ConvertFrom(this DistanceUnitType to, DistanceUnitType from, double value)
    {
        return ConvertFrom(to, from, value, 1);
    }

    public static double ConvertTo(this DistanceUnitType from, DistanceUnitType to, double value)
    {
        return ConvertTo(from, to, value, 1);
    }

    public static double ConvertFrom(this DistanceUnitType to, DistanceUnitType from, double value, int unitPower)
    {
        return value * Math.Pow(conversionFactors[(int)to, (int)from], unitPower);
    }

    public static double ConvertTo(this DistanceUnitType from, DistanceUnitType to, double value, int unitPower)
    {
        return value * Math.Pow(conversionFactors[(int)to, (int)from], unitPower);
    }
}
