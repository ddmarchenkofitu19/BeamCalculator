using BeamCalculator.Views.Popups;
using CommunityToolkit.Maui.Views;

namespace BeamCalculator.Helpers;


public static class ElementExtensions
{
    public static Page FindParentPage(this Element elm)
    {
        var p = elm.Parent;
        while(!(p is Page))
        {
            if (p == null)
                return null;
            p = p.Parent;
        }

        return (Page)p;
    }

    public static void OpenSettingsPopup(this Element elm)
    {
        if(elm is Page)
            (elm as Page).ShowPopup(new SettingsPopup());
        else
            elm.FindParentPage().ShowPopup(new SettingsPopup());
    }
}
