using BeamCalculator.Models;
using BeamCalculator.Models.Section;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BeamCalculator.ViewModels;


[ObservableObject]
public partial class CustomSectionViewModel
{
    [ObservableProperty]
    CustomSectionModel model;
    [ObservableProperty]
    bool hasError = false;
    [ObservableProperty]
    CalculationResults calcResults = new CalculationResults();

    public CustomSectionViewModel()
    {
        model = MauiProgram.Services.GetRequiredService<CustomSectionModel>();
        model.Recalculate();
        CalcResults = model.Results;
    }
}
