using BeamCalculator.Helpers.Drawing;

namespace BeamCalculator.Models.Section;


public class LProfileSectionModel : CommonSectionModel
{
    private static readonly Dictionary<string, SectionDimensionData> lProfileDimensions = new Dictionary<string, SectionDimensionData>()
    {
        ["width"] = new SectionDimensionData()
        {
            Value = 40,
            Name = "width",
            ShortName = "B",
        },
        ["height"] = new SectionDimensionData()
        {
            Value = 40,
            Name = "height",
            ShortName = "H",
        },
        ["web width"] = new SectionDimensionData()
        {
            Value = 20,
            Name = "web width",
            ShortName = "b",
        },
        ["flange height"] = new SectionDimensionData()
        {
            Value = 15,
            Name = "flange height",
            ShortName = "h",
        },
    };

    private double _dimWidth => _dimesions["width"].Value;
    private double _dimHeight => _dimesions["height"].Value;
    private double _dimWebWidth => _dimesions["web width"].Value;
    private double _dimFlangeHeight => _dimesions["flange height"].Value;


    public override SectionTypes Type => SectionTypes.LProfile;
    public override List<Point> Points => new List<Point>()
    {
        new Point(-_dimWidth / 2, _dimHeight / 2),
        new Point(-_dimWidth / 2 + _dimWebWidth, _dimHeight / 2),
        new Point(-_dimWidth / 2 + _dimWebWidth, -_dimHeight / 2 + _dimFlangeHeight),
        new Point(_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight),
        new Point(_dimWidth / 2, -_dimHeight / 2),
        new Point(-_dimWidth / 2,  -_dimHeight / 2),
    };
    public override List<List<int>> ContoursIndices => new List<List<int>>()
    {
        new List<int>() { 0,1,2,3,4,5 }
    };
    public override List<Fragment> Fragments => new List<Fragment>()
    {
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2),
            new Point(-_dimWidth / 2 + _dimWebWidth, -_dimHeight / 2 + _dimFlangeHeight),
            new Point(-_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight)),
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2),
            new Point(-_dimWidth / 2 + _dimWebWidth, _dimHeight / 2),
            new Point(-_dimWidth / 2 + _dimWebWidth, -_dimHeight / 2 + _dimFlangeHeight)),
        new Fragment(
            new Point(-_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight),
            new Point(_dimWidth / 2, -_dimHeight / 2),
            new Point(-_dimWidth / 2,  -_dimHeight / 2)),
        new Fragment(
            new Point(-_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight),
            new Point(_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight),
            new Point(_dimWidth / 2, -_dimHeight / 2)),
    };
    public override DimensionLabel[] DimensionLabels => new DimensionLabel[]
    {
        new DimensionLabel()
        {
            ShortName = "B",
            Name = "width",
            Orientation = DimensionLabelOrientation.HorizontalDown,
            Level = 0,
            Pos1 = (float)-Width / 2,
            Pos2 = (float)Width / 2,
        },
        new DimensionLabel()
        {
            ShortName = "b",
            Name = "web width",
            Orientation = DimensionLabelOrientation.HorizontalUp,
            Level = 0,
            Pos1 = (float)-Width / 2,
            Pos2 = (float)(-Width / 2 + WebWidth),
        },
        new DimensionLabel()
        {
            ShortName = "H",
            Name = "height",
            Orientation = DimensionLabelOrientation.VerticalLeft,
            Level = 0,
            Pos1 = (float)-Height / 2,
            Pos2 = (float)Height / 2,
        },
        new DimensionLabel()
        {
            ShortName = "h",
            Name = "flange height",
            Orientation = DimensionLabelOrientation.VerticalRight,
            Level = 0,
            Pos1 = (float)(Height / 2 - FlangeHeight),
            Pos2 = (float)Height / 2,
        },
    };

    public override double Width => _dimWidth;
    public override double Height => _dimHeight;
    public double WebWidth => _dimWebWidth;
    public double FlangeHeight => _dimFlangeHeight;


    public LProfileSectionModel() : base(lProfileDimensions)
	{ }

    protected override bool CheckSectionValidity()
    {
        if (!base.CheckSectionValidity())
            return false;

        var err = "";
        if (_dimFlangeHeight >= _dimHeight)
            err = "h must be less than H";
        if (_dimWebWidth >= _dimWidth)
            err = "b must be less than B";

        ErrorString = err;
        if (err == "")
            return true;

        return false;
    }
}
