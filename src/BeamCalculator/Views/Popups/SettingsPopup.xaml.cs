using BeamCalculator.Models;
using BeamCalculator.Helpers;
using CommunityToolkit.Maui.Views;
using SkiaSharp.Views.Maui;

namespace BeamCalculator.Views.Popups;

public partial class SettingsPopup : Popup
{
    public SettingsPopup()
	{
        InitializeComponent();
    }

    private void OnOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        Size = CalcPopupSize();
    }

    private Size CalcPopupSize()
    {
        var parent = this.FindParentPage();
        double w = 0, h = 0;

#if ANDROID
        w = parent.Width;
        h = parent.Height;
        if (parent.Parent is TabbedPage)
        {
            var activity = Platform.CurrentActivity;
            var resId = activity.Resources.GetIdentifier("design_bottom_navigation_height", "dimen", activity.PackageName);
            if (resId > 0)
            {
                var bottomNavHeight = activity.Resources.GetDimensionPixelSize(resId) / activity.Resources.DisplayMetrics.Density;
                h += bottomNavHeight;
            }
        }

        VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.End;
        HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Start;
#elif WINDOWS
        w = Math.Max(parent.Width / 2, 540);
        h = Math.Max(parent.Height / 2, 450);

        VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
#endif

        return new Size(w, h);
    }

    private void CloseClicked(object sender, EventArgs e) 
        => this.Close();



}