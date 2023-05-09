using BeamCalculator.Helpers;
using BeamCalculator.Models;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace BeamCalculator.Views.Popups;

public partial class ColorPickerPopup : Popup
{
    public ColorPickerPopup(Color selectedColor)
    {
        InitializeComponent();

        var colors = CommonData.PossiblePickerColors;

        ResultWhenUserTapsOutsideOfPopup = null;
        View colorItem;
        var tapRecognizer = new TapGestureRecognizer();
        tapRecognizer.Tapped += ColorTapped;
        for (var i = 0; i < colors.Count; i++)
        {
            colorItem = new Border()
            {
                WidthRequest = 40,
                HeightRequest = 40,
                Margin = new Thickness(3),
                Stroke = Color.FromArgb("#585858"),
                StrokeThickness = 1,
                StrokeShape = selectedColor.Equals(colors[i]) 
                    ? new Ellipse()
                    : new RoundRectangle() { CornerRadius = new CornerRadius(3) },
                BackgroundColor = colors[i]
            };
            colorItem.GestureRecognizers.Add(tapRecognizer);
        
            colorsContainer.Add(colorItem);
        }
    }

    private Size CalcPopupSize()
    {
        var page = this.FindParentPage();
        double w = page.Width;
        double h = (page.Height / 1.6) + 50;

        return new Size(w, h);
    }

    private void OnOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        Size = CalcPopupSize();
    }

    private void ColorTapped(object sender, TappedEventArgs e)
    {
        var border = sender as Border;
        this.Close(border.BackgroundColor);
    }
}