using BeamCalculator.Helpers;
using BeamCalculator.Models;
using BeamCalculator.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using BeamCalculator.Components;
using CommunityToolkit.Maui.Storage;
using Path = System.IO.Path;

namespace BeamCalculator.Views;

public partial class SectionToolPage : TabbedPage
{
    private CommonSectionViewModel commonVM;
    private CustomSectionViewModel customVM;
    private SectionTypes toolType = SectionTypes.None;
    private ITitleView titleView;


    public SectionTypes ToolType
    {
        get => toolType;
        set
        {
            var oldValue = toolType;
            toolType = value;

            if (DeviceInfo.Platform == DevicePlatform.Android && oldValue != SectionTypes.None)
            {
                // select first tab
                this.CurrentPage = this.Children[0];

                // clear input and error container
                errAndInputsContainer.Children.Clear();
            }

            if (value == SectionTypes.Custom)
            {
                customVM = MauiProgram.Services.GetRequiredService<CustomSectionViewModel>();
                this.BindingContext = customVM;
            }
            else
            {
                commonVM = MauiProgram.Services.GetRequiredService<CommonSectionViewModel>();
                this.BindingContext = commonVM;
                commonVM.PropertyChanged += CommonViewModel_PropertyChanged;
                commonVM.SectionVisualizer = sectionVisualizerView;
                commonVM.ToolType = value;
            }
        }
    }


    public SectionToolPage()
    {
        InitializeComponent();
    }



    private void TabbedPage_Appearing(object sender, EventArgs e)
        => SetTitleView();

    private void CommonViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(commonVM.Dimensions))
        {
            CreateInputsAndErrorLabel();
        }
    }

    
    private void SetTitleView()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
            titleView = new ButtonsTitleView()
            {
                Title = toolType.ToUserFriendlyString(),
                Buttons = new TitleViewButton[]
                {
                    new()
                    {
                        ImageSource = "share_black_24dp.png",
                        Command = new Command(this.ShareReportTapped)
                    },
                    new()
                    {
                        ImageSource = "settings_black_24dp.png",
                        Command = new Command(this.OpenSettingsPopup)
                    }
                }
            };
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            titleView = new DefaultTitleView() { Title = toolType.ToUserFriendlyString() };

        NavigationPage.SetTitleView(this, (View)titleView);
    }

    private async void ShareReportTapped()
    {
        var c = new PdfReportCreator(ToolType == SectionTypes.Custom ? customVM.Model : commonVM.Model);
        var fpath = c.CreateFile();

        if(!String.IsNullOrEmpty(fpath))
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Sharing the report",
                File = new ShareFile(fpath)
            });
    }

    private async void ShareReportClicked(object sender, EventArgs e)
    {
        var c = new PdfReportCreator(ToolType == SectionTypes.Custom ? customVM.Model : commonVM.Model);
        var fpath = c.CreateFile();
        
        if(!String.IsNullOrEmpty(fpath))
        {
            var pdfstream = File.Open(fpath, FileMode.Open);
            await FileSaver.Default.SaveAsync(Path.GetFileName(fpath), pdfstream, new CancellationToken());
        }
    }

    private void CreateInputsAndErrorLabel()
    {
        // create Error label
        var errLabel = new Label() { TextColor = Colors.Red };
        errLabel.SetBinding(Label.TextProperty, new Binding("ErrorMessage"));
        errAndInputsContainer.Add(errLabel);
        

        // Dynamically create cross section dimensions inputs on page, generate and pass observable coolection 
        // of inputs data into view model 
        var grid = new Grid() { RowSpacing = 5 };
        grid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        grid.AddColumnDefinition(new ColumnDefinition(GridLength.Star));

        // get info about inputs that need to be generated
        var inputsInfo = commonVM.Dimensions;
        // create observable collection for view model access to input properties
        var observColl = new ObservableCollectionWithItemNotify<InputItemData>();

        for (var i = 0; i < inputsInfo.Count; i++)
        {
            var item = inputsInfo[i];
            grid.AddRowDefinition(new RowDefinition { Height = GridLength.Star });

            // add input item data into view model observable collection
            observColl.Add(new InputItemData(InputItemDataType.Entry, item.Value, item.Name));

            // create label component
            var label = new Label()
            {
                Text = item.ShortName + "=",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            

            // create border for entry component
            var border = new Border()
            {
                Padding = new Thickness(0),
                StrokeThickness = DeviceInfo.Platform == DevicePlatform.WinUI ? 2 : 0,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(5) }
            };
            // bind border property to ViewModel
            border.SetBinding(
                Border.StrokeProperty,
                new Binding(
                    path: $"Inputs[{observColl.Count - 1}].ErrorDisplayingColor",
                    converter: new ColorToBrushConverter()));


            // create entry
            var entry = new Entry()
            {
                Keyboard = Keyboard.Numeric,
                Placeholder = "Enter " + item.ShortName,
            };
            // bind entry property to ViewModel
            entry.SetBinding(Entry.TextProperty, new Binding($"Inputs[{observColl.Count - 1}].Value"));
            entry.SetBinding(Entry.IsEnabledProperty, new Binding($"Inputs[{observColl.Count - 1}].IsEnabled"));
            // add entry into border
            border.Content = entry;


            // add components into layout
            grid.Add(label, 0, i);
            grid.Add(border, 1, i);
        }

        // set generated inputs data into viewmodel
        commonVM.Inputs = observColl;
        // add inputs grid on page
        errAndInputsContainer.Add(grid);
    }
}