using BeamCalculator.Helpers.Drawing;

namespace BeamCalculator.Models.Section;


public class TProfileSectionModel : CommonSectionModel
{
    private static readonly Dictionary<string, SectionDimensionData> tProfileDimensions = new Dictionary<string, SectionDimensionData>()
    {
        ["width"] = new SectionDimensionData()
        {
            Value = 40,
            Name = "width",
            ShortName = "B",
        },
        ["web width"] = new SectionDimensionData()
        {
            Value = 10,
            Name = "web width",
            ShortName = "b",
        },
        ["web position"] = new SectionDimensionData()
        {
            Value = 25,
            Name = "web position",
            ShortName = "b1",
        },
        ["height"] = new SectionDimensionData()
        {
            Value = 40,
            Name = "height",
            ShortName = "H",
        },
        ["flange height"] = new SectionDimensionData()
        {
            Value = 20,
            Name = "flange height",
            ShortName = "h",
        },
    };

    private double _dimWidth => _dimesions["width"].Value;
    private double _dimWebWidth=> _dimesions["web width"].Value;
    private double _dimWebPosition => _dimesions["web position"].Value;
    private double _dimHeight => _dimesions["height"].Value;
    private double _dimFlangeHeight => _dimesions["flange height"].Value;


    public override SectionTypes Type => SectionTypes.TProfile;
    public override List<Point> Points => new List<Point>()
    {
        new Point(-_dimWidth / 2, _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)),
        new Point(_dimWidth / 2 - (_dimWebPosition + _dimWebWidth / 2), _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)),
        new Point(_dimWidth / 2 - (_dimWebPosition + _dimWebWidth / 2), _dimHeight / 2),
        new Point(_dimWidth / 2 - (_dimWebPosition - _dimWebWidth / 2), _dimHeight / 2),
        new Point(_dimWidth / 2 - (_dimWebPosition - _dimWebWidth / 2), _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)),
        new Point(_dimWidth / 2, _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)),
        new Point(_dimWidth / 2, -_dimHeight / 2),
        new Point(-_dimWidth / 2, -_dimHeight / 2),
    };
    public override List<List<int>> ContoursIndices => new List<List<int>>()
    {
        new List<int> { 0,1,2,3,4,5,6,7 },
    };
    public override List<Fragment> Fragments => new List<Fragment>()
    {
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)),
            new Point(_dimWidth / 2, -_dimHeight / 2),
            new Point(-_dimWidth / 2, -_dimHeight / 2)),
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)),
            new Point(_dimWidth / 2, _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)), 
            new Point(_dimWidth / 2, -_dimHeight / 2)),
        new Fragment(
            new Point(_dimWidth / 2 - (_dimWebPosition + _dimWebWidth / 2), _dimHeight / 2),
            new Point(_dimWidth / 2 - (_dimWebPosition - _dimWebWidth / 2), _dimHeight / 2 - (_dimHeight - _dimFlangeHeight)),
            new Point(_dimWidth / 2 - (_dimWebPosition + _dimWebWidth / 2), _dimHeight / 2 - (_dimHeight - _dimFlangeHeight))),
        new Fragment(
            new Point(_dimWidth / 2 - (_dimWebPosition + _dimWebWidth / 2), _dimHeight / 2),
            new Point(_dimWidth / 2 - (_dimWebPosition - _dimWebWidth / 2), _dimHeight / 2), 
            new Point(_dimWidth / 2 - (_dimWebPosition - _dimWebWidth / 2), _dimHeight / 2 - (_dimHeight - _dimFlangeHeight))),
    };
    public override DimensionLabel[] DimensionLabels => new DimensionLabel[]
    {
        new DimensionLabel()
        {
            ShortName = "B",
            Name = "width",
            Orientation = DimensionLabelOrientation.HorizontalDown,
            Level = 1,
            Pos1 = (float)-Width / 2,
            Pos2 = (float)Width / 2,
        },
        new DimensionLabel()
        {
            ShortName = "b",
            Name = "web width",
            Orientation = DimensionLabelOrientation.HorizontalDown,
            Level = 0,
            Pos1 = (float)(Width / 2 - (WebPosition + WebWidth / 2)),
            Pos2 = (float)(Width / 2 - (WebPosition - WebWidth / 2)),
        },
        new DimensionLabel()
        {
            ShortName = "b1",
            Name = "web position",
            Orientation = DimensionLabelOrientation.HorizontalUp,
            Level = 0,
            Pos1 = (float)(Width / 2 - WebPosition),
            Pos2 = (float)Width / 2,
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
    public double WebPosition => _dimWebPosition;
    public double FlangeHeight => _dimFlangeHeight;


    public TProfileSectionModel() : base(tProfileDimensions)
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
        if (_dimWebPosition <= _dimWebWidth / 2)
            err = "b1 must be greater than b/2";
        if (_dimWebPosition >= _dimWidth - (_dimWebWidth / 2))
            err = "b1 must be less than B-(b/2)";

        ErrorString = err;
        if (err == "")
            return true;

        return false;
    }
}
