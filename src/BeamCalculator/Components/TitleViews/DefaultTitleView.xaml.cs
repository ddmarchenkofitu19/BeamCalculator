namespace BeamCalculator.Components;

public partial class DefaultTitleView : ContentView, ITitleView
{
    static readonly BindableProperty TitleProperty = 
        BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(DefaultTitleView),
            defaultValue: "",
            defaultBindingMode: BindingMode.OneWay);

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


    public DefaultTitleView()
	{
		InitializeComponent();
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

    private void OnParentSizeChanged(object sender, EventArgs e)
    {
        var page = (Page)Parent;
        container.WidthRequest = page.Width;
    }
}