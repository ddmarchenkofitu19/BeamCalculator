using BeamCalculator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Collections.ObjectModel;

namespace BeamCalculator.Helpers;


[ObservableObject]
public partial class UserSettings
{
    private readonly ReadOnlyDictionary<string, object> defaultValues = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>()
    {
        [nameof(SectionFillColor)] = CommonData.PossiblePickerColors[46].ToSKColor().WithAlpha(CommonData.OpacityPickerSource.Values.ElementAt(7)).ToString(),
        [nameof(SectionOutlineColor)] = CommonData.PossiblePickerColors[0].ToSKColor().ToString(),
        [nameof(InputDistanceUnitType)] = DistanceUnitType.Millimeters,
        [nameof(OutputDistanceUnitType)] = DistanceUnitType.Millimeters,
    });

    private readonly IPreferences prefs;


    public UserSettings(IPreferences preferences)
    {
        this.prefs = preferences;
    }


    public SKColor SectionFillColor
    {
        get 
        {
            if (!prefs.ContainsKey(nameof(SectionFillColor)))
                prefs.Set(nameof(SectionFillColor), (string)defaultValues[nameof(SectionFillColor)]);

            return SKColor.Parse(prefs.Get<string>(nameof(SectionFillColor), (string)defaultValues[nameof(SectionFillColor)])); 
        }
        set 
        {
            var val = value.WithAlpha(SectionFillOpacity);
            if (val == SectionFillColor) return;

            OnPropertyChanging(nameof(SectionFillColor));
            prefs.Set(nameof(SectionFillColor), val.ToString());
            OnPropertyChanged(nameof(SectionFillColor));
        }
    }

    public byte SectionFillOpacity
    {
        get
        {
            if (!prefs.ContainsKey(nameof(SectionFillColor)))
                prefs.Set(nameof(SectionFillColor), (string)defaultValues[nameof(SectionFillColor)]);

            var color = SKColor.Parse(prefs.Get<string>(nameof(SectionFillColor), (string)defaultValues[nameof(SectionFillColor)]));
            return color.Alpha;
        }
        set
        {
            if (value == SectionFillColor.Alpha) return;

            var color = SectionFillColor.WithAlpha(value);

            OnPropertyChanging(nameof(SectionFillColor));
            OnPropertyChanging(nameof(SectionFillOpacity));
            prefs.Set(nameof(SectionFillColor), color.ToString());
            OnPropertyChanged(nameof(SectionFillColor));
            OnPropertyChanged(nameof(SectionFillOpacity));
        }
    }

    public SKColor SectionOutlineColor
    {
        get
        {
            if (!prefs.ContainsKey(nameof(SectionOutlineColor)))
                prefs.Set(nameof(SectionOutlineColor), (string)defaultValues[nameof(SectionOutlineColor)]);

            return SKColor.Parse(prefs.Get<string>(nameof(SectionOutlineColor), (string)defaultValues[nameof(SectionOutlineColor)]));
        }
        set 
        {
            if (value == SectionOutlineColor) return;

            OnPropertyChanging(nameof(SectionOutlineColor));
            prefs.Set(nameof(SectionOutlineColor), value.ToString());
            OnPropertyChanged(nameof(SectionOutlineColor));
        }
    }

    public DistanceUnitType InputDistanceUnitType
    {
        get
        {
            if (!prefs.ContainsKey(nameof(InputDistanceUnitType)))
                prefs.Set<int>(nameof(InputDistanceUnitType), (int)defaultValues[nameof(InputDistanceUnitType)]);

            return (DistanceUnitType)prefs.Get<int>(nameof(InputDistanceUnitType), (int)defaultValues[nameof(InputDistanceUnitType)]);
        }
        set 
        {
            if (value == InputDistanceUnitType) return;

            OnPropertyChanging(nameof(InputDistanceUnitType));
            prefs.Set<int>(nameof(InputDistanceUnitType), (int)value);
            OnPropertyChanged(nameof(InputDistanceUnitType));
        }
    }

    public DistanceUnitType OutputDistanceUnitType
    {
        get
        {
            if (!prefs.ContainsKey(nameof(OutputDistanceUnitType)))
                prefs.Set<int>(nameof(OutputDistanceUnitType), (int)defaultValues[nameof(OutputDistanceUnitType)]);

            return (DistanceUnitType)prefs.Get<int>(nameof(OutputDistanceUnitType), (int)defaultValues[nameof(OutputDistanceUnitType)]);
        }
        set 
        {
            if (value == OutputDistanceUnitType) return;

            OnPropertyChanging(nameof(OutputDistanceUnitType));
            prefs.Set<int>(nameof(OutputDistanceUnitType), (int)value);
            OnPropertyChanged(nameof(OutputDistanceUnitType));
        }
    }

    public void Reset()
    {
        var pArr = this.GetType().GetProperties();
        foreach(var prop in pArr)
        {
            OnPropertyChanging(prop.Name);
            if(prefs.ContainsKey(prop.Name)) prefs.Remove(prop.Name);
            OnPropertyChanged(prop.Name); 
        }
    }
}
