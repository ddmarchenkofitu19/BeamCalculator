namespace BeamCalculator.Models;


public static class CommonData
{
    public static List<string> DistanceUnitsItemSource = 
        ((DistanceUnitType[])Enum.GetValues(typeof(DistanceUnitType))).Select(a => a.ToUserFriendlyString()).ToList();

    public static Dictionary<string, byte> OpacityPickerSource = new Dictionary<string, byte>()
    {
        ["0%"] = 0,
        ["10%"] = (byte)(255 * 0.1),
        ["20%"] = (byte)(255 * 0.2),
        ["30%"] = (byte)(255 * 0.3),
        ["40%"] = (byte)(255 * 0.4),
        ["50%"] = (byte)(255 * 0.5),
        ["60%"] = (byte)(255 * 0.6),
        ["70%"] = (byte)(255 * 0.7),
        ["80%"] = (byte)(255 * 0.8),
        ["90%"] = (byte)(255 * 0.9),
        ["100%"] = 255,
    };

    public static List<Color> PossiblePickerColors = new List<Color>()
    {
        // 2014 Material color palettes (material.io)
        Color.FromArgb("#000000"), // Black 
        Color.FromArgb("#FFFFFF"), // White 

        //Color.FromArgb("#263238"), // 900 
        //Color.FromArgb("#37474F"), // 800 
        Color.FromArgb("#455A64"), // 700 
        //Color.FromArgb("#546E7A"), // 600 
        Color.FromArgb("#607D8B"), // 500 
        //Color.FromArgb("#78909C"), // 400 
        //Color.FromArgb("#90A4AE"), // 300 
        Color.FromArgb("#B0BEC5"), // 200 
        //Color.FromArgb("#CFD8DC"), // 100 
        //Color.FromArgb("#ECEFF1"), // Blue Gray 50 

        //Color.FromArgb("#212121"), // 900 
        Color.FromArgb("#424242"), // 800 
        //Color.FromArgb("#616161"), // 700 
        Color.FromArgb("#757575"), // 600 
        //Color.FromArgb("#9E9E9E"), // 500 
        Color.FromArgb("#BDBDBD"), // 400 
        //Color.FromArgb("#E0E0E0"), // 300 
        //Color.FromArgb("#EEEEEE"), // 200 
        //Color.FromArgb("#F5F5F5"), // 100 
        //Color.FromArgb("#FAFAFA"), // Gray 50 

        //Color.FromArgb("#3E2723"), // 900 
        //Color.FromArgb("#4E342E"), // 800 
        Color.FromArgb("#5D4037"), // 700 
        //Color.FromArgb("#6D4C41"), // 600 
        Color.FromArgb("#795548"), // 500 
        //Color.FromArgb("#8D6E63"), // 400 
        //Color.FromArgb("#A1887F"), // 300 
        Color.FromArgb("#BCAAA4"), // 200 
        //Color.FromArgb("#D7CCC8"), // 100 
        //Color.FromArgb("#EFEBE9"), // Brown 50 

        Color.FromArgb("#DD2C00"), // A700 
        Color.FromArgb("#FF3D00"), // A400 
        Color.FromArgb("#FF6E40"), // A200 
        Color.FromArgb("#FF9E80"), // A100 
        //Color.FromArgb("#BF360C"), // 900 
        //Color.FromArgb("#D84315"), // 800 
        //Color.FromArgb("#E64A19"), // 700 
        //Color.FromArgb("#F4511E"), // 600 
        //Color.FromArgb("#FF5722"), // 500 
        //Color.FromArgb("#FF7043"), // 400 
        //Color.FromArgb("#FF8A65"), // 300 
        //Color.FromArgb("#FFAB91"), // 200 
        //Color.FromArgb("#FFCCBC"), // 100 
        //Color.FromArgb("#FBE9E7"), // Deep Orange 50 

        Color.FromArgb("#FF6D00"), // A700 
        Color.FromArgb("#FF9100"), // A400 
        Color.FromArgb("#FFAB40"), // A200 
        Color.FromArgb("#FFD180"), // A100 
        //Color.FromArgb("#E65100"), // 900 
        //Color.FromArgb("#EF6C00"), // 800 
        //Color.FromArgb("#F57C00"), // 700 
        //Color.FromArgb("#FB8C00"), // 600 
        //Color.FromArgb("#FF9800"), // 500 
        //Color.FromArgb("#FFA726"), // 400 
        //Color.FromArgb("#FFB74D"), // 300 
        //Color.FromArgb("#FFCC80"), // 200 
        //Color.FromArgb("#FFE0B2"), // 100 
        //Color.FromArgb("#FFF3E0"), // Orange 50 

        //Color.FromArgb("#FFAB00"), // A700 
        //Color.FromArgb("#FFC400"), // A400 
        //Color.FromArgb("#FFD740"), // A200 
        //Color.FromArgb("#FFE57F"), // A100 
        //Color.FromArgb("#FF6F00"), // 900 
        //Color.FromArgb("#FF8F00"), // 800 
        //Color.FromArgb("#FFA000"), // 700 
        //Color.FromArgb("#FFB300"), // 600 
        //Color.FromArgb("#FFC107"), // 500 
        //Color.FromArgb("#FFCA28"), // 400 
        //Color.FromArgb("#FFD54F"), // 300 
        //Color.FromArgb("#FFE082"), // 200 
        //Color.FromArgb("#FFECB3"), // 100 
        //Color.FromArgb("#FFF8E1"), // Amber 50 

        Color.FromArgb("#FFD600"), // A700 
        //Color.FromArgb("#FFEA00"), // A400 
        Color.FromArgb("#FFFF00"), // A200 
        Color.FromArgb("#FFFF8D"), // A100 
        //Color.FromArgb("#F57F17"), // 900 
        //Color.FromArgb("#F9A825"), // 800 
        //Color.FromArgb("#FBC02D"), // 700 
        //Color.FromArgb("#FDD835"), // 600 
        //Color.FromArgb("#FFEB3B"), // 500 
        //Color.FromArgb("#FFEE58"), // 400 
        //Color.FromArgb("#FFF176"), // 300 
        //Color.FromArgb("#FFF59D"), // 200 
        Color.FromArgb("#FFF9C4"), // 100 
        //Color.FromArgb("#FFFDE7"), // Yellow 50 

        Color.FromArgb("#AEEA00"), // A700 
        //Color.FromArgb("#C6FF00"), // A400 
        Color.FromArgb("#EEFF41"), // A200 
        Color.FromArgb("#F4FF81"), // A100 
        //Color.FromArgb("#827717"), // 900 
        //Color.FromArgb("#9E9D24"), // 800 
        //Color.FromArgb("#AFB42B"), // 700 
        //Color.FromArgb("#C0CA33"), // 600 
        //Color.FromArgb("#CDDC39"), // 500 
        //Color.FromArgb("#D4E157"), // 400 
        //Color.FromArgb("#DCE775"), // 300 
        //Color.FromArgb("#E6EE9C"), // 200 
        Color.FromArgb("#F0F4C3"), // 100 
        //Color.FromArgb("#F9FBE7"), // Lime 50 

        Color.FromArgb("#64DD17"), // A700 
        Color.FromArgb("#76FF03"), // A400 
        Color.FromArgb("#B2FF59"), // A200 
        Color.FromArgb("#CCFF90"), // A100 
        //Color.FromArgb("#33691E"), // 900 
        //Color.FromArgb("#558B2F"), // 800 
        //Color.FromArgb("#689F38"), // 700 
        //Color.FromArgb("#7CB342"), // 600 
        //Color.FromArgb("#8BC34A"), // 500 
        //Color.FromArgb("#9CCC65"), // 400 
        //Color.FromArgb("#AED581"), // 300 
        //Color.FromArgb("#C5E1A5"), // 200 
        //Color.FromArgb("#DCEDC8"), // 100 
        //Color.FromArgb("#F1F8E9"), // Light Green 50 

        Color.FromArgb("#00C853"), // A700 
        Color.FromArgb("#00E676"), // A400 
        Color.FromArgb("#69F0AE"), // A200 
        Color.FromArgb("#B9F6CA"), // A100 
        //Color.FromArgb("#1B5E20"), // 900 
        //Color.FromArgb("#2E7D32"), // 800 
        //Color.FromArgb("#388E3C"), // 700 
        //Color.FromArgb("#43A047"), // 600 
        //Color.FromArgb("#4CAF50"), // 500 
        //Color.FromArgb("#66BB6A"), // 400 
        //Color.FromArgb("#81C784"), // 300 
        //Color.FromArgb("#A5D6A7"), // 200 
        //Color.FromArgb("#C8E6C9"), // 100 
        //Color.FromArgb("#E8F5E9"), // Green 50 

        Color.FromArgb("#00BFA5"), // A700 
        Color.FromArgb("#1DE9B6"), // A400 
        Color.FromArgb("#64FFDA"), // A200 
        Color.FromArgb("#A7FFEB"), // A100 
        //Color.FromArgb("#004D40"), // 900 
        //Color.FromArgb("#00695C"), // 800 
        //Color.FromArgb("#00796B"), // 700 
        //Color.FromArgb("#00897B"), // 600 
        //Color.FromArgb("#009688"), // 500 
        //Color.FromArgb("#26A69A"), // 400 
        //Color.FromArgb("#4DB6AC"), // 300 
        //Color.FromArgb("#80CBC4"), // 200 
        //Color.FromArgb("#B2DFDB"), // 100 
        //Color.FromArgb("#E0F2F1"), // Teal 50 

        Color.FromArgb("#00B8D4"), // A700 
        Color.FromArgb("#00E5FF"), // A400 
        Color.FromArgb("#18FFFF"), // A200 
        Color.FromArgb("#84FFFF"), // A100 
        //Color.FromArgb("#006064"), // 900 
        //Color.FromArgb("#00838F"), // 800 
        //Color.FromArgb("#0097A7"), // 700 
        //Color.FromArgb("#00ACC1"), // 600 
        //Color.FromArgb("#00BCD4"), // 500 
        //Color.FromArgb("#26C6DA"), // 400 
        //Color.FromArgb("#4DD0E1"), // 300 
        //Color.FromArgb("#80DEEA"), // 200 
        //Color.FromArgb("#B2EBF2"), // 100 
        //Color.FromArgb("#E0F7FA"), // Cyan 50 

        Color.FromArgb("#0091EA"), // A700 
        Color.FromArgb("#00B0FF"), // A400 
        Color.FromArgb("#40C4FF"), // A200 
        //Color.FromArgb("#80D8FF"), // A100 
        //Color.FromArgb("#01579B"), // 900 
        //Color.FromArgb("#0277BD"), // 800 
        //Color.FromArgb("#0288D1"), // 700 
        //Color.FromArgb("#039BE5"), // 600 
        //Color.FromArgb("#03A9F4"), // 500 
        //Color.FromArgb("#29B6F6"), // 400 
        //Color.FromArgb("#4FC3F7"), // 300 
        Color.FromArgb("#81D4FA"), // 200 
        //Color.FromArgb("#B3E5FC"), // 100 
        //Color.FromArgb("#E1F5FE"), // Light Blue 50 
        //Color.FromArgb("#87CEFA"),// my color

        Color.FromArgb("#2962FF"), // A700 
        Color.FromArgb("#2979FF"), // A400 
        Color.FromArgb("#448AFF"), // A200 
        Color.FromArgb("#82B1FF"), // A100 
        //Color.FromArgb("#0D47A1"), // 900 
        //Color.FromArgb("#1565C0"), // 800 
        //Color.FromArgb("#1976D2"), // 700 
        //Color.FromArgb("#1E88E5"), // 600 
        //Color.FromArgb("#2196F3"), // 500 
        //Color.FromArgb("#42A5F5"), // 400 
        //Color.FromArgb("#64B5F6"), // 300 
        //Color.FromArgb("#90CAF9"), // 200 
        //Color.FromArgb("#BBDEFB"), // 100 
        //Color.FromArgb("#E3F2FD"), // Blue 50 

        //Color.FromArgb("#304FFE"), // A700 
        //Color.FromArgb("#3D5AFE"), // A400 
        //Color.FromArgb("#536DFE"), // A200 
        //Color.FromArgb("#8C9EFF"), // A100 
        //Color.FromArgb("#1A237E"), // 900 
        //Color.FromArgb("#283593"), // 800 
        //Color.FromArgb("#303F9F"), // 700 
        //Color.FromArgb("#3949AB"), // 600 
        //Color.FromArgb("#3F51B5"), // 500 
        //Color.FromArgb("#5C6BC0"), // 400 
        //Color.FromArgb("#7986CB"), // 300 
        //Color.FromArgb("#9FA8DA"), // 200 
        //Color.FromArgb("#C5CAE9"), // 100 
        //Color.FromArgb("#E8EAF6"), // Indigo 50 

        Color.FromArgb("#6200EA"), // A700 
        Color.FromArgb("#651FFF"), // A400 
        Color.FromArgb("#7C4DFF"), // A200 
        Color.FromArgb("#B388FF"), // A100 
        //Color.FromArgb("#311B92"), // 900 
        //Color.FromArgb("#4527A0"), // 800 
        //Color.FromArgb("#512DA8"), // 700 
        //Color.FromArgb("#5E35B1"), // 600 
        //Color.FromArgb("#673AB7"), // 500 
        //Color.FromArgb("#7E57C2"), // 400 
        //Color.FromArgb("#9575CD"), // 300 
        //Color.FromArgb("#B39DDB"), // 200 
        //Color.FromArgb("#D1C4E9"), // 100 
        //Color.FromArgb("#EDE7F6"), // Deep Purple 50 

        Color.FromArgb("#AA00FF"), // A700 
        //Color.FromArgb("#D500F9"), // A400 
        Color.FromArgb("#E040FB"), // A200 
        Color.FromArgb("#EA80FC"), // A100 
        //Color.FromArgb("#4A148C"), // 900 
        //Color.FromArgb("#6A1B9A"), // 800 
        //Color.FromArgb("#7B1FA2"), // 700 
        //Color.FromArgb("#8E24AA"), // 600 
        //Color.FromArgb("#9C27B0"), // 500 
        //Color.FromArgb("#AB47BC"), // 400 
        //Color.FromArgb("#BA68C8"), // 300 
        Color.FromArgb("#CE93D8"), // 200 
        //Color.FromArgb("#E1BEE7"), // 100 
        //Color.FromArgb("#F3E5F5"), // Purple 50 

        //Color.FromArgb("#C51162"), // A700 
        Color.FromArgb("#F50057"), // A400 
        Color.FromArgb("#FF4081"), // A200 
        Color.FromArgb("#FF80AB"), // A100 
        //Color.FromArgb("#880E4F"), // 900 
        //Color.FromArgb("#AD1457"), // 800 
        //Color.FromArgb("#C2185B"), // 700 
        //Color.FromArgb("#D81B60"), // 600 
        //Color.FromArgb("#E91E63"), // 500 
        //Color.FromArgb("#EC407A"), // 400 
        //Color.FromArgb("#F06292"), // 300 
        //Color.FromArgb("#F48FB1"), // 200 
        Color.FromArgb("#F8BBD0"), // 100 
        //Color.FromArgb("#FCE4EC"), // Pink 50 

        //Color.FromArgb("#D50000"), // A700 
        //Color.FromArgb("#FF1744"), // A400 
        //Color.FromArgb("#FF5252"), // A200 
        //Color.FromArgb("#FF8A80"), // A100 
        //Color.FromArgb("#B71C1C"), // 900 
        //Color.FromArgb("#C62828"), // 800 
        //Color.FromArgb("#D32F2F"), // 700 
        //Color.FromArgb("#E53935"), // 600 
        //Color.FromArgb("#F44336"), // 500 
        //Color.FromArgb("#EF5350"), // 400 
        //Color.FromArgb("#E57373"), // 300 
        //Color.FromArgb("#EF9A9A"), // 200 
        //Color.FromArgb("#FFCDD2"), // 100 
        //Color.FromArgb("#FFEBEE"), // Red 50 
    };
}
