using BeamCalculator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using BeamCalculator.Helpers;
using System.Collections.Specialized;
using System.Globalization;
using BeamCalculator.Models.Section;
using System.ComponentModel;
using BeamCalculator.Components;

namespace BeamCalculator.ViewModels;

[ObservableObject]
public partial class CommonSectionViewModel
{
    private readonly SectionModelProvider _sectionProvider;
    private readonly UserSettings _settings;

    [ObservableProperty]
    CommonSectionModel model;
    [ObservableProperty]
    SectionTypes toolType = SectionTypes.None;
    [ObservableProperty]
    List<SectionDimensionData> dimensions;
    [ObservableProperty]
    ObservableCollectionWithItemNotify<InputItemData> inputs = new ObservableCollectionWithItemNotify<InputItemData>();
    [ObservableProperty]
    bool hasError = false;
    [ObservableProperty]
    string errorMessage = "";
    [ObservableProperty]
    CalculationResults calcResults = new CalculationResults();

    public ISectionVisualizer SectionVisualizer { get; set; }


    public CommonSectionViewModel(SectionModelProvider sectionProvider)
	{
        _sectionProvider = sectionProvider;

        _settings = MauiProgram.Services.GetRequiredService<UserSettings>();
        _settings.PropertyChanged += Settings_PropertyChanged;
    }


    partial void OnToolTypeChanged(SectionTypes value)
    {
        Model = _sectionProvider.GetSectionModel(value);
        model.PropertyChanged += Model_PropertyChanged;

        Dimensions = model.GetDimensions().Values.ToList();
        model.Recalculate();
    }

    private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(UserSettings.InputDistanceUnitType))
        {
            foreach(var inp in inputs)
            {
                if(TryParseInputNum(inp, out double num))
                {
                    model.SetDimensionValue(inp.ItemName, num);
                }
            }
        }
    }

    private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(model.Results))
        {
            if(!InputsHasErrors())
                CalcResults = model.Results;
        }
        else if(e.PropertyName == nameof(model.ErrorString))
        {
            if (!InputsHasErrors())
            {
                ErrorMessage = model.ErrorString;
                HasError = ErrorMessage != "";
            }
        }
        else if(e.PropertyName == nameof(model.NeedRedraw))
        {
            if (!InputsHasErrors() && model.NeedRedraw)
            {
                SectionVisualizer.InvalidateResetScale();
                model.NeedRedraw = false;
            }
        }
    }

    partial void OnInputsChanged(ObservableCollectionWithItemNotify<InputItemData> value)
    {
        inputs.CollectionChanged += OnInputItemsCollectionUpdated;
    }

    private void OnInputItemsCollectionUpdated(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Replace)
            return;

        var input = (InputItemData)e.NewItems[0];

        if(TryParseInputNum(input, out double num))
        {
            if (!InputsHasErrors() && ErrorMessage == "")
                ClearError();

            model.SetDimensionValue(input.ItemName, num);
        }
    }

    private bool TryParseInputNum(InputItemData input, out double result)
    {
        double num;
        result = Double.NaN;

        // if input value nonparseble
        if (!Double.TryParse((string)input.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
        {
            input.HasError = true; // set error for input(change entry border color)
            SetError(""); // set page error and disable button
            return false;
        }

        input.HasError = false; // clear input error
        result = num;
        return true;
    }

    private bool InputsHasErrors()
    {
        foreach (var input in Inputs)
            if (input.HasError)
                return true;

        return false;
    }

    private void SetError(string error)
    {
        HasError = true;
    }

    private void ClearError()
    {
        HasError = false;
    }
}