using CommunityToolkit.Mvvm.ComponentModel;

namespace BeamCalculator.Helpers;


public partial class InputItemData : ObservableObject
{
    private static readonly Color NoErrorColor = Colors.LightGray;
    private static readonly Color ErrorColor = Colors.Red;

    public InputItemDataType ItemType { get; }
    public string ItemName { get; }

    [ObservableProperty]
    private object value;

    [ObservableProperty]
    private object isEnabled = true;

    [ObservableProperty]
    private Color errorDisplayingColor = NoErrorColor;

    private bool hasError;
    public bool HasError
    {
        get { return hasError; }
        set
        {
            if (hasError != value)
            {
                if (value)
                    ErrorDisplayingColor = ErrorColor;
                else
                    ErrorDisplayingColor = NoErrorColor;

                hasError = value;
            }
        }
    }

    public InputItemData(InputItemDataType itemType, object defaultValue, string itemName)
    {
        this.ItemType = itemType;
        this.value = defaultValue;
        this.ItemName = itemName;
    }
}

public enum InputItemDataType
{
    Entry,
}
