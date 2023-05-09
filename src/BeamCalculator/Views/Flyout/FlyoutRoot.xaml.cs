using CommunityToolkit.Maui.Views;

namespace BeamCalculator.Views.Flyout;

public partial class FlyoutRoot : FlyoutPage
{
    private FlyoutMenuItem[] menuItems = new FlyoutMenuItem[]
    {
        new()
        {
            Title = "Tools",
            IconSource = "ruler_black_24dp.png",
            ItemType = FlyoutMenuItemType.DetailPage,
            TargetType = typeof(ToolsListPage),
            OnPlatforms = new DevicePlatform[] { DevicePlatform.Android, DevicePlatform.WinUI }
        },
        //new()
        //{
        //    Title = "Section Database",
        //    Description = "WIP",
        //    IconSource = "section_database_black_24dp.png",
        //    ItemType = FlyoutMenuItemType.NavPage,
        //    TargetType = null,
        //    OnPlatforms = new DevicePlatform[] { DevicePlatform.Android, DevicePlatform.WinUI }
        //},
        new FlyoutMenuItem()
        {
            Title = "Settings",
            IconSource = "settings_black_24dp.png",
            ItemType = FlyoutMenuItemType.NavPage,
            TargetType = typeof(SettingsPage),
            OnPlatforms = new DevicePlatform[] { DevicePlatform.WinUI }
        },
        new()
        {
            Title = "About",
            IconSource = "about_black_24dp.png",
            ItemType = FlyoutMenuItemType.NavPage,
            TargetType = typeof(AboutPage),
            OnPlatforms = new DevicePlatform[] { DevicePlatform.Android, DevicePlatform.WinUI }
        },
    };


    public FlyoutRoot()
	{
        InitializeComponent();
        
        //title property must be set on Flyout page
        Title = "FlyoutRoot";

        flyoutMenu.MenuItemsSource = menuItems;
        flyoutMenu.SelectedItem = flyoutMenu.MenuItemsSource[0];

        // create command and attach it to flyout menu buttons
        flyoutMenu.MenuItemTapped += OnMenuItemTapped;

        // event to track nav stack poping
        var navPage = (NavigationPage)Detail;
        navPage.Popped += OnNavigationPopped;
    }

    void OnMenuItemTapped(object sender, MenuItemTappedEventArgs e)
    {
        var mi = e.ItemTapped;

        if (mi.TargetType == null)
            return;

        // get current displayed page from navigation page
        var navPage = (NavigationPage)Detail;
        var curPage = navPage.CurrentPage;

        switch (mi.ItemType)
        {
            case FlyoutMenuItemType.NavPage:
                if(!mi.TargetType.Equals(curPage.GetType()))
                {
                    // create selected page instance
                    var selectedPage = (Page)Activator.CreateInstance(mi.TargetType);

                    // get navigation from current page
                    var nav = (curPage as NavigableElement).Navigation;

                    // navigate to the new page
                    navPage.PushAsync(selectedPage);

                    // select menu item
                    flyoutMenu.SelectedItem = mi;
                }
                break;

            case FlyoutMenuItemType.DetailPage:
                if(!mi.TargetType.Equals(curPage.GetType()))
                {
                    if(mi.TargetType.Equals(navPage.RootPage.GetType()))
                    {
                        navPage.PopToRootAsync();
                        flyoutMenu.SelectedItem = mi;
                    }
                    else
                    {
                        //change detail page
                    }
                }
                break;

            case FlyoutMenuItemType.Popup:
                var popup = (Popup)Activator.CreateInstance(mi.TargetType);
                curPage.ShowPopup(popup);
                break;
            
        }

        // hide flyout menu
        IsPresented = false;
    }

    private void OnNavigationPopped(object sender, NavigationEventArgs e)
    {
        var isMenuItem = menuItems.SingleOrDefault(i => e.Page.GetType().Equals(i.TargetType));
        if(isMenuItem != null)
        {
            var navPage = (NavigationPage)Detail;

            var rootItem = menuItems.SingleOrDefault(i => navPage.RootPage.GetType().Equals(i.TargetType));

            flyoutMenu.SelectedItem = rootItem;
        }
    }
}