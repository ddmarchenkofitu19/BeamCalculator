using BeamCalculator.Helpers.Drawing;

namespace BeamCalculator.Models.Section;


public class RectangleTubingSectionModel : CommonSectionModel
{
    private static readonly Dictionary<string, SectionDimensionData> rectangleTubingDimensions = new Dictionary<string, SectionDimensionData>()
    {
        ["width"] = new SectionDimensionData()
        {
            Value = 40,
            Name = "width",
            ShortName = "B",
        },
        ["web width 1"] = new SectionDimensionData()
        {
            Value = 10,
            Name = "web width 1",
            ShortName = "b1",
        },
        ["web width 2"] = new SectionDimensionData()
        {
            Value = 10,
            Name = "web width 2",
            ShortName = "b2",
        },
        ["height"] = new SectionDimensionData()
        {
            Value = 40,
            Name = "height",
            ShortName = "H",
        },
        ["flange height 1"] = new SectionDimensionData()
        {
            Value = 10,
            Name = "flange height 1",
            ShortName = "h1",
        },
        ["flange height 2"] = new SectionDimensionData()
        {
            Value = 10,
            Name = "flange height 2",
            ShortName = "h2",
        },
    };

    private double _dimWidth => _dimesions["width"].Value;
    private double _dimHeight => _dimesions["height"].Value;
    private double _dimWebWidth1 => _dimesions["web width 1"].Value;
    private double _dimWebWidth2 => _dimesions["web width 2"].Value;
    private double _dimFlangeHeight1 => _dimesions["flange height 1"].Value;
    private double _dimFlangeHeight2 => _dimesions["flange height 2"].Value;


    public override SectionTypes Type => SectionTypes.RectangleTubing;
    public override List<Point> Points => new List<Point>()
    {
        new Point(-_dimWidth / 2, _dimHeight / 2),
        new Point(_dimWidth / 2, _dimHeight / 2),
        new Point(_dimWidth / 2, -_dimHeight / 2),
        new Point(-_dimWidth / 2, -_dimHeight / 2),
        new Point(-_dimWidth / 2 + _dimWebWidth1, _dimHeight / 2 - _dimFlangeHeight1),
        new Point(_dimWidth / 2 - _dimWebWidth2, _dimHeight / 2 - _dimFlangeHeight1),
        new Point(_dimWidth / 2 - _dimWebWidth2, -_dimHeight / 2 + _dimFlangeHeight2),
        new Point(-_dimWidth / 2 + _dimWebWidth1, -_dimHeight / 2 + _dimFlangeHeight2),
    };
    public override List<List<int>> ContoursIndices => new List<List<int>>()
    {
        new List<int> { 0,1,2,3 },
        new List<int> { 4,5,6,7 }
    };
    public override List<Fragment> Fragments => new List<Fragment>()
    {
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2), 
            new Point(_dimWidth / 2, _dimHeight / 2 - _dimFlangeHeight1), 
            new Point(-_dimWidth / 2, _dimHeight / 2 - _dimFlangeHeight1)),
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2),
            new Point(_dimWidth / 2, _dimHeight / 2), 
            new Point(_dimWidth / 2, _dimHeight / 2 - _dimFlangeHeight1)),
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2 - _dimFlangeHeight1), 
            new Point(-_dimWidth / 2 + _dimWebWidth1, -_dimHeight / 2 + _dimFlangeHeight2), 
            new Point(-_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight2)),
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2 - _dimFlangeHeight1),
            new Point(-_dimWidth / 2 + _dimWebWidth1, _dimHeight / 2 - _dimFlangeHeight1),
            new Point(-_dimWidth / 2 + _dimWebWidth1, -_dimHeight / 2 + _dimFlangeHeight2)),
        new Fragment(
            new Point(_dimWidth / 2 - _dimWebWidth2, _dimHeight / 2 - _dimFlangeHeight1), 
            new Point(_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight2),
            new Point(_dimWidth / 2 - _dimWebWidth2, -_dimHeight / 2 + _dimFlangeHeight2)),
        new Fragment(
            new Point(_dimWidth / 2 - _dimWebWidth2, _dimHeight / 2 - _dimFlangeHeight1), 
            new Point(_dimWidth / 2, _dimHeight / 2 - _dimFlangeHeight1), 
            new Point(_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight2)),
        new Fragment(
            new Point(-_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight2),
            new Point(_dimWidth / 2, -_dimHeight / 2),
            new Point(-_dimWidth / 2, -_dimHeight / 2)),
        new Fragment(
            new Point(-_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight2), 
            new Point(_dimWidth / 2, -_dimHeight / 2 + _dimFlangeHeight2), 
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
            ShortName = "b1",
            Name = "web width 1",
            Orientation = DimensionLabelOrientation.HorizontalUp,
            Level = 0,
            Pos1 = (float)-Width / 2,
            Pos2 = (float)(-Width / 2 + WebWidth1),
        },
        new DimensionLabel()
        {
            ShortName = "b2",
            Name = "web width 2",
            Orientation = DimensionLabelOrientation.HorizontalUp,
            Level = 0,
            Pos1 = (float)(Width / 2 - WebWidth2),
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
            ShortName = "h1",
            Name = "flange height 1",
            Orientation = DimensionLabelOrientation.VerticalRight,
            Level = 0,
            Pos1 = (float)-(Height / 2),
            Pos2 = (float)-(Height / 2 - FlangeHeight1),
        },
        new DimensionLabel()
        {
            ShortName = "h2",
            Name = "flange height 2",
            Orientation = DimensionLabelOrientation.VerticalRight,
            Level = 0,
            Pos1 = (float)-(-Height / 2 + FlangeHeight2),
            Pos2 = (float)-(-Height / 2),
        },
    };

    public override double Width => _dimWidth;
    public override double Height => _dimHeight;
    public double WebWidth1 => _dimWebWidth1;
    public double WebWidth2 => _dimWebWidth2;
    public double FlangeHeight1 => _dimFlangeHeight1;
    public double FlangeHeight2 => _dimFlangeHeight2;


    public RectangleTubingSectionModel() : base(rectangleTubingDimensions)
    { }


    protected override bool CheckSectionValidity()
    {
        if (!base.CheckSectionValidity())
            return false;

        var err = "";
        if (_dimFlangeHeight1 + _dimFlangeHeight2 >= _dimHeight)
            err = "h1 + h2 must be less than H";
        if (_dimWebWidth1 + _dimWebWidth1 >= _dimWidth)
            err = "b1 + b2 must be less than B";

        ErrorString = err;
        if (err == "")
            return true;

        return false;
    }
}
