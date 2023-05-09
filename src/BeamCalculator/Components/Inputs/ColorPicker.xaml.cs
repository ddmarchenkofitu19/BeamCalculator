using BeamCalculator.Helpers;
using BeamCalculator.Views.Popups;
using CommunityToolkit.Maui.Views;

namespace BeamCalculator.Components;

public partial class ColorPicker : ContentView
{
    private static readonly Color LightBorderStroke = Color.FromArgb("#a0a0a0");
    private static readonly Color DarkBorderStroke = Color.FromArgb("#606060");

    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(
            propertyName: nameof(SelectedColor),
            returnType: typeof(Color),
            declaringType: typeof(ColorPicker),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanging: SelectedColorPropertyChanging);

    public Color SelectedColor 
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }


    public event EventHandler SelectedColorChanged;


    public ColorPicker()
	{
		InitializeComponent();
	}
    

    private async void OnTapped(object sender, TappedEventArgs e)
    {
        var page = this.FindParentPage();
        var color = (Color)await page.ShowPopupAsync(new ColorPickerPopup(SelectedColor));
        if (color != null && color != SelectedColor)
        {
            SelectedColor = color;
            RaiseSelectedColorChanged();
        }
    }

    private static void SelectedColorPropertyChanging(BindableObject bindable, object oldValue, object newValue)
    {
        var picker = (ColorPicker)bindable;
        if (newValue != null)
        {
            var newColor = (Color)newValue;
            picker.border.BackgroundColor = newColor;

            var strokeColor = newColor.GetLuminosity() >= 0.5 ? DarkBorderStroke : LightBorderStroke;
            picker.border.Stroke = strokeColor;
        }
    }

    protected virtual void RaiseSelectedColorChanged()
        => SelectedColorChanged?.Invoke(this, new EventArgs());
}