using BeamCalculator.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace BeamCalculator.Models;


[INotifyPropertyChanged]
public partial class CalculationResults
{
    public static DistanceUnitType ResultValuesUnit => DistanceUnitType.Millimeters;


    private UserSettings _settings;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AreaString))]
    private double area = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InertiaMomentYString))]
    private double inertiaMomentY = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InertiaMomentZString))]
    private double inertiaMomentZ = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InertiaMomentYZString))]
    private double inertiaMomentYZ = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InertiaAxesAngleString))]
    private double inertiaAxesAngle = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MainInertiaMomentYString))]
    private double mainInertiaMomentY = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MainInertiaMomentZString))]
    private double mainInertiaMomentZ = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(inertiaRadiusYString))]
    private double inertiaRadiusY = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(inertiaRadiusZString))]
    private double inertiaRadiusZ = 0;

    [ObservableProperty]
    private Point massCenter = new Point(0, 0);

    [ObservableProperty]
    private List<Point> sectionCore = new List<Point>();


    public string AreaString 
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, area, 2), 3);
    public string InertiaMomentYString 
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, inertiaMomentY, 4), 3);
    public string InertiaMomentZString 
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, inertiaMomentY, 4), 3);
    public string InertiaMomentYZString 
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, inertiaMomentY, 4), 3);
    public string InertiaAxesAngleString 
        => ConvertDoubleToOutputString(inertiaAxesAngle * 180 / Math.PI, 3);
    public string MainInertiaMomentYString 
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, mainInertiaMomentY, 4), 3);
    public string MainInertiaMomentZString 
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, mainInertiaMomentZ, 4), 3);
    public string inertiaRadiusYString
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, inertiaRadiusY), 3);
    public string inertiaRadiusZString
        => ConvertDoubleToOutputString(ResultValuesUnit.ConvertTo(_settings.OutputDistanceUnitType, inertiaRadiusZ), 3);


    public CalculationResults()
    {
        _settings = MauiProgram.Services.GetRequiredService<UserSettings>();
    }


    public string ConvertDoubleToOutputString(double value, int decimalPlaces)
    {
        return String.Format(CultureInfo.InvariantCulture, $"{{0:F{decimalPlaces}}}", value);
    }

    public string ConvertIntToOutputString(int value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}
