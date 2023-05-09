using BeamCalculator.Helpers;
using BeamCalculator.Models;
using SkiaSharp.Views.Maui;

namespace BeamCalculator.Components;

public partial class SettingsContent : ContentView
{
    private UserSettings _settings;

    public SettingsContent()
	{
		InitializeComponent();

        _settings = MauiProgram.Services.GetRequiredService<UserSettings>();
        ConfigurateInputs();
    }

    private void ConfigurateInputs()
    {
        // input distance units
        inUnitPicker.ItemsSource = CommonData.DistanceUnitsItemSource;
        inUnitPicker.SelectedIndex = (int)_settings.InputDistanceUnitType;

        // output distance units
        outUnitPicker.ItemsSource = CommonData.DistanceUnitsItemSource;
        outUnitPicker.SelectedIndex = (int)_settings.OutputDistanceUnitType;

        // setion fill color
        fillColorPicker.SelectedColor = _settings.SectionFillColor.ToMauiColor().WithAlpha(1);

        // section fill opacity

        if (fillOpacityPicker.ItemsSource == null)
            fillOpacityPicker.ItemsSource = CommonData.OpacityPickerSource.Keys.ToList();
        var op = _settings.SectionFillOpacity;
        fillOpacityPicker.SelectedItem = CommonData.OpacityPickerSource.SingleOrDefault(x => x.Value == _settings.SectionFillOpacity).Key;

        // section outline color
        outlineColorPicker.SelectedColor = _settings.SectionOutlineColor.ToMauiColor().WithAlpha(1);
    }

    private async void ResetTapped(object sender, TappedEventArgs e)
    {
        var answer = await this.FindParentPage()
            .DisplayAlert("Reset all settings", "Are you sure you want to reset all settings to default?", "Yes", "No");

        if (answer)
        {
            _settings.Reset();
            ConfigurateInputs();
        }
    }

    private void inUnitChanged(object sender, EventArgs e)
        => _settings.InputDistanceUnitType = (DistanceUnitType)inUnitPicker.SelectedIndex;

    private void outUnitChanged(object sender, EventArgs e)
        => _settings.OutputDistanceUnitType = (DistanceUnitType)outUnitPicker.SelectedIndex;

    private void fillColorChanged(object sender, EventArgs e)
        => _settings.SectionFillColor = fillColorPicker.SelectedColor.ToSKColor();

    private void fillOpacityChanged(object sender, EventArgs e)
        => _settings.SectionFillOpacity = CommonData.OpacityPickerSource[(string)fillOpacityPicker.SelectedItem];

    private void outlineColorChanged(object sender, EventArgs e)
        => _settings.SectionOutlineColor = outlineColorPicker.SelectedColor.ToSKColor();
}