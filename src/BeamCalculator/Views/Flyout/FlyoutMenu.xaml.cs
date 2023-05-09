using Microsoft.Maui.Controls.Shapes;

namespace BeamCalculator.Views.Flyout;

public partial class FlyoutMenu : ContentPage
{
    private readonly Color SelectedMenuItemBackgroundColor = Colors.LightGray;

    public static readonly BindableProperty MenuItemsSourceProperty =
        BindableProperty.Create(
            propertyName: nameof(MenuItemsSource),
            returnType: typeof(IList<FlyoutMenuItem>),
            declaringType: typeof(FlyoutMenu),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneTime,
            propertyChanged: MenuItemsSourceChanged);

    public static readonly BindableProperty SelectedItemProperty =
		BindableProperty.Create(
			propertyName: nameof(SelectedItem), 
			returnType: typeof(FlyoutMenuItem), 
			declaringType: typeof(FlyoutMenu), 
			defaultValue: null, 
			defaultBindingMode: BindingMode.OneWay,
            propertyChanged: SelectedItemChanged);


    private List<View> menuItems = new List<View>();

    public IList<FlyoutMenuItem> MenuItemsSource
    {
        get => (IList<FlyoutMenuItem>)GetValue(MenuItemsSourceProperty);
        set => SetValue(MenuItemsSourceProperty, value);
    }

    public FlyoutMenuItem SelectedItem 
	{ 
		get => (FlyoutMenuItem)GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}


    public event EventHandler<MenuItemTappedEventArgs> MenuItemTapped;


	public FlyoutMenu()
	{
		InitializeComponent();
	}


    private void UpdateMenuItemsViews()
    {
        if (menuItems.Count > 0)
        {
            menuItems.Clear();
            menuItemContainer.Children.Clear();
            SelectedItem = null;
        }

        for (int i = 0, c = 0; i < MenuItemsSource.Count; i++)
        {
            if (i == 0)
                menuItemContainer.Add(new Rectangle() { HeightRequest = 8, BackgroundColor = Colors.Transparent });

            if (MenuItemsSource[i].OnPlatforms.Contains(DeviceInfo.Platform))
            {
                var item = MenuItemsSource[i];
                menuItems.Add(CreateMenuItem(item, c));
                menuItemContainer.Add(menuItems[c]);
                c++;
            }
        }
    }

    private View CreateMenuItem(FlyoutMenuItem item, int row)
    {
        var container = new Grid()
        {
            HeightRequest = 48,
            ColumnDefinitions = new ColumnDefinitionCollection( 
                new ColumnDefinition[] { 
                    new ColumnDefinition(GridLength.Star), 
                    new ColumnDefinition(new GridLength(3.9, GridUnitType.Star))}),
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.Transparent,
        };

        // gesture reconizer
        var tgr = new TapGestureRecognizer()
        {
            Command = new Command(OnItemTapped),
            CommandParameter = item
        };
        container.GestureRecognizers.Add(tgr);

        // icon image
        container.Add(new Image()
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Aspect = Aspect.Center,
            Source = item.IconSource
        }, column: 0, row: row);

        // title label
        container.Add(new Label()
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(4, 0),
            Text = item.Title,
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
        }, column: 1, row: row);

        // description label
        var grid = new Grid()
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };
        grid.Add(new Label()
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(10, 0),
            Text = item.Description,
            FontSize = 17,
            TextColor = Color.FromArgb("#0097a7"),
        });
        container.Add(grid, column: 1, row: row);

        return container;
    }

    private void SelectItem(FlyoutMenuItem item)
    {
        foreach (var menuItem in menuItems)
        {
            var tapRecognizer = menuItem.GestureRecognizers[0] as TapGestureRecognizer;
            if(tapRecognizer.CommandParameter == item)
            {
                menuItem.BackgroundColor = SelectedMenuItemBackgroundColor;
            }
        }
    }

    private void UnselectItem(FlyoutMenuItem item)
    {
        foreach (var menuItem in menuItems)
        {
            var tapRecognizer = menuItem.GestureRecognizers[0] as TapGestureRecognizer;
            if(tapRecognizer.CommandParameter == item)
            {
                menuItem.BackgroundColor = Colors.Transparent;
            }
        }
    }

    private static void MenuItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var menu = bindable as FlyoutMenu;
        menu.UpdateMenuItemsViews();
    }

    private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var menu = bindable as FlyoutMenu;

        if(oldValue != null)
        {
            menu.UnselectItem((FlyoutMenuItem)oldValue);
        }

        if (newValue != null)
        {
            menu.SelectItem((FlyoutMenuItem)newValue);
        }
    }

    private void OnItemTapped(object i)
    {
        var item = (FlyoutMenuItem)i;
        RaiseMenuItemTappedEvent(item);
    }

    protected virtual void RaiseMenuItemTappedEvent(FlyoutMenuItem item) 
        => MenuItemTapped?.Invoke(this, new MenuItemTappedEventArgs(item));
}