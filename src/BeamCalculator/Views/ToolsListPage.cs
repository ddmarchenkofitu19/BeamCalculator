using BeamCalculator.Components;
using BeamCalculator.Helpers;
using BeamCalculator.Models;
using System.Windows.Input;

namespace BeamCalculator.Views;

public class ToolsListPage : ContentPage
{
    public ToolsListPage()
	{
        // set title view 
        if (DeviceInfo.Platform == DevicePlatform.Android)
            NavigationPage.SetTitleView(this, new ButtonsTitleView() 
            { 
                Title = "Tools",
                Buttons = new TitleViewButton[]
                {
                    new() 
                    { 
                        ImageSource = "settings_black_24dp.png",
                        Command = new Command(this.OpenSettingsPopup)
                    }
                }
            });
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            NavigationPage.SetTitleView(this, new DefaultTitleView() { Title = "Tools" });

        // set color for menu/back icons
        NavigationPage.SetIconColor(this, Colors.Black);


        // generate page content
		var scroll = new ScrollView() { Padding = new Thickness(0, 0, 0, 20) };
		var layout = new VerticalStackLayout() { Margin = new Thickness(10, 10, 10, 0), Spacing = 8 };

		scroll.Content = layout;

		layout.Add(CreateSectionToolsButtons());

        Content = scroll;
	}


    void SectionToolButton_Tapped(SectionTypes tappedToolType)
    {
        if(tappedToolType == SectionTypes.Custom)
        {
            var page = MauiProgram.Services.GetRequiredService<SectionDesignerPage>();
            Navigation.PushAsync(page);
        }
        else
        {
            var page = MauiProgram.Services.GetRequiredService<SectionToolPage>();
            page.ToolType = tappedToolType;
            Navigation.PushAsync(page);
        }
    }


    private IView CreateSectionToolsButtons()
	{
		var layout = new VerticalStackLayout() { Spacing = 8 };

		foreach(SectionTypes tool in Enum.GetValues(typeof(SectionTypes)))
		{
			if (tool == SectionTypes.None) continue;

            var cmd = new Command<SectionTypes>(SectionToolButton_Tapped);

			layout.Add(CreateListButton(tool.ToUserFriendlyString(), cmd, tool));
		}

		return layout;
	}

    private IView CreateListButton(string text, ICommand command, object commandParam)
	{
		// create button inner  layout
        var layout = new Grid();
		layout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
		layout.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));

        // create button farame and add tap recongnizer
        var frame = new Frame() { Content = layout, CornerRadius = 8, Padding = new Thickness(15)};
        var tapRecognizer = new TapGestureRecognizer();
        tapRecognizer.SetValue(TapGestureRecognizer.CommandProperty, command);
        tapRecognizer.SetValue(TapGestureRecognizer.CommandParameterProperty, commandParam);
        frame.GestureRecognizers.Add(tapRecognizer);

        // button text label
        var btnTextLabel = new Label()
        {
            Text = text,
            HorizontalOptions = LayoutOptions.Start
        };

        // button arrow label
        var arrowLabel = new Label()
        {
            Text = "\u276f",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.LightGrey,
            HorizontalOptions = LayoutOptions.End
        };

        // add to layout
        layout.Add(btnTextLabel, 0, 0);
        layout.Add(arrowLabel, 1, 0);

        return frame;
    }

    private IView CreateListButton(string text, ICommand command)
    {
        // create button inner  layout
        var layout = new Grid();
        layout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        layout.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));

        // create button farame and add tap recongnizer
        var frame = new Frame() { Content = layout, CornerRadius = 8, Padding = new Thickness(15) };
        var tapRecognizer = new TapGestureRecognizer();
        tapRecognizer.Command = command;
        frame.GestureRecognizers.Add(tapRecognizer);

        // button text label
        var btnTextLabel = new Label()
        {
            Text = text,
            HorizontalOptions = LayoutOptions.Start
        };

        // button arrow label
        var arrowLabel = new Label()
        {
            Text = "\u276f",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.LightGrey,
            HorizontalOptions = LayoutOptions.End
        };

        // add to layout
        layout.Add(btnTextLabel, 0, 0);
        layout.Add(arrowLabel, 1, 0);

        return frame;
    }
}