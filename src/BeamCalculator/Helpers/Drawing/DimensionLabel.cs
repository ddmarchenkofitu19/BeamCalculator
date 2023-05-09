namespace BeamCalculator.Helpers.Drawing;


public class DimensionLabel
{
    public string ShortName { get; set; }

    public string Name { get; set; }

    public DimensionLabelOrientation Orientation { get; set; }

    public float Pos1 { get; set; }
    public float Pos2 { get; set; }

    public int Level { get; set; }
}

public enum DimensionLabelOrientation
{
    VerticalLeft,
    VerticalRight,
    HorizontalUp,
    HorizontalDown
}
