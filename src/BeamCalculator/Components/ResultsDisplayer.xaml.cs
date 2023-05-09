using BeamCalculator.Helpers;
using BeamCalculator.Models;

namespace BeamCalculator.Components;


public partial class ResultsDisplayer : ContentView
{
    private readonly UserSettings _settings;


    private string distanceUnitString;
    public string DistanceUnitString
    {
        get => distanceUnitString;
        set
        {
            OnPropertyChanging(nameof(DistanceUnitString));
            distanceUnitString = value;
            OnPropertyChanged(nameof(DistanceUnitString));
        }
    }


    public static readonly BindableProperty ResultValuesProperty =
          BindableProperty.Create(
              propertyName: nameof(ResultValues),
              returnType: typeof(CalculationResults),
              declaringType: typeof(ResultsDisplayer),
              defaultValue: new CalculationResults(),
              defaultBindingMode: BindingMode.OneWay);
    public CalculationResults ResultValues
    {
        get => (CalculationResults)GetValue(ResultValuesProperty);
        set => SetValue(ResultValuesProperty, value);
    }


    public ResultsDisplayer()
    {
        InitializeComponent();

        _settings = MauiProgram.Services.GetRequiredService<UserSettings>();
        _settings.PropertyChanged += Settings_PropertyChanged;

        DistanceUnitString = _settings.OutputDistanceUnitType.ToUserFriendlyString();
    }




    private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_settings.OutputDistanceUnitType))
        {
            var outUnit = _settings.OutputDistanceUnitType;
            DistanceUnitString = outUnit.ToUserFriendlyString();

            OnPropertyChanged(nameof(ResultValues));
        }
    }
}