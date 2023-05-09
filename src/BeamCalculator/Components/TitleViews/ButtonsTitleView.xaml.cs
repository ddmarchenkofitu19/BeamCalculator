using BeamCalculator.Helpers;
using BeamCalculator.Views.Popups;
using CommunityToolkit.Maui.Views;

namespace BeamCalculator.Components;

public partial class ButtonsTitleView : ContentView, ITitleView
{
    public static readonly BindableProperty TitleProperty = 
        BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(ButtonsTitleView),
            defaultValue: "",
            defaultBindingMode: BindingMode.OneWay);

    public static readonly BindableProperty ButtonsProperty =
        BindableProperty.Create(
            propertyName: nameof(Buttons),
            returnType: typeof(TitleViewButton[]),
            declaringType: typeof(ButtonsTitleView),
            defaultValue: new TitleViewButton[0],
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: Buttons_Changed);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set
        {
            if (value == Title)
                return;
            lblTitle.Text = value;
            SetValue(TitleProperty, value);
        }
    }

    public TitleViewButton[] Buttons
    {
        get => (TitleViewButton[])GetValue(ButtonsProperty);
        set
        {
            if (value == Buttons)
                return;
            SetValue(ButtonsProperty, value);
        }
    }


    public ButtonsTitleView()
	{
		InitializeComponent();
	}


    private void CreateButtons()
    {
        buttonsContainer.Clear();

        foreach(var btn in Buttons)
        {
            var layout = new VerticalStackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            layout.GestureRecognizers.Add(
                new TapGestureRecognizer() { Command = btn.Command });

            layout.Add(new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Source = btn.ImageSource
            });

            buttonsContainer.Add(layout);
        }
    }

    public static void Buttons_Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var instance = bindable as ButtonsTitleView;
        instance.CreateButtons();
    }


    private void OnLoaded(object sender, EventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            var page = (Page)Parent;
            container.WidthRequest = page.Width;
            page.SizeChanged += OnParentSizeChanged;
        }
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        var page = (Page)Parent;
        page.SizeChanged -= OnParentSizeChanged;
    }

    private void OnParentSizeChanged(object sender, EventArgs e)
    {
        var page = (Page)Parent;
        container.WidthRequest = page.Width;
    }
}