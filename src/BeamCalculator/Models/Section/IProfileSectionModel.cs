using BeamCalculator.Helpers.Drawing;

namespace BeamCalculator.Models.Section;


public class IProfileSectionModel : CommonSectionModel
{
    private static readonly Dictionary<string, SectionDimensionData> iProfileDimensions = new Dictionary<string, SectionDimensionData>()
    {
        ["web width"] = new SectionDimensionData()
        {
            Value = 15,
            Name = "web width",
            ShortName = "b",
        },
        ["flange width 1"] = new SectionDimensionData()
        {
            Value = 30,
            Name = "flange width 1",
            ShortName = "b1",
        },
        ["flange width 2"] = new SectionDimensionData()
        {
            Value = 30,
            Name = "flange width 2",
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
            Value = 15,
            Name = "flange height 1",
            ShortName = "h1",
        },
        ["flange height 2"] = new SectionDimensionData()
        {
            Value = 15,
            Name = "flange height 2",
            ShortName = "h2",
        },
    };

    private double _dimWebWidth => _dimesions["web width"].Value;
    private double _dimFlangeWidth1 => _dimesions["flange width 1"].Value;
    private double _dimFlangeWidth2 => _dimesions["flange width 2"].Value;
    private double _dimHeight => _dimesions["height"].Value;
    private double _dimFlangeHeight1 => _dimesions["flange height 1"].Value;
    private double _dimFlangeHeight2 => _dimesions["flange height 2"].Value;


    public override SectionTypes Type => SectionTypes.IProfile;
    public override List<Point> Points => new List<Point>()
    {
        new Point(-_dimFlangeWidth2 / 2, _dimHeight / 2),
        new Point(_dimFlangeWidth2 / 2, _dimHeight / 2),
        new Point(_dimFlangeWidth2 / 2, _dimHeight / 2 - _dimFlangeHeight2),
        new Point(_dimWebWidth / 2, _dimHeight / 2 - _dimFlangeHeight2),
        new Point(_dimWebWidth / 2, -_dimHeight / 2 + _dimFlangeHeight1),
        new Point(_dimFlangeWidth1 / 2, -_dimHeight / 2 + _dimFlangeHeight1),
        new Point(_dimFlangeWidth1 / 2, -_dimHeight / 2),
        new Point(-_dimFlangeWidth1 / 2, -_dimHeight / 2),
        new Point(-_dimFlangeWidth1 / 2, -_dimHeight / 2 + _dimFlangeHeight1),
        new Point(-_dimWebWidth / 2, -_dimHeight / 2 + _dimFlangeHeight1),
        new Point(-_dimWebWidth / 2, _dimHeight / 2 - _dimFlangeHeight2),
        new Point(-_dimFlangeWidth2 / 2, _dimHeight / 2 - _dimFlangeHeight2),
    };
    public override List<List<int>> ContoursIndices => new List<List<int>>()
    { 
        new List<int>
        {
            0,1,2,3,4,5,6,7,8,9,10,11
        }
    };
    public override List<Fragment> Fragments => new List<Fragment>()
    {
        new Fragment(
            new Point(-_dimFlangeWidth2 / 2, _dimHeight / 2),
            new Point(_dimFlangeWidth2 / 2, _dimHeight / 2 - _dimFlangeHeight2),
            new Point(-_dimFlangeWidth2 / 2, _dimHeight / 2 - _dimFlangeHeight2)),
        new Fragment(
            new Point(-_dimFlangeWidth2 / 2, _dimHeight / 2),
            new Point(_dimFlangeWidth2 / 2, _dimHeight / 2),
            new Point(_dimFlangeWidth2 / 2, _dimHeight / 2 - _dimFlangeHeight2)),
        new Fragment(
            new Point(-_dimWebWidth / 2, _dimHeight / 2 - _dimFlangeHeight2),
            new Point(_dimWebWidth / 2, -_dimHeight / 2 + _dimFlangeHeight1),
            new Point(-_dimWebWidth / 2, -_dimHeight / 2 + _dimFlangeHeight1)),
        new Fragment(
            new Point(-_dimWebWidth / 2, _dimHeight / 2 - _dimFlangeHeight2),
            new Point(_dimWebWidth / 2, _dimHeight / 2 - _dimFlangeHeight2),
            new Point(_dimWebWidth / 2, -_dimHeight / 2 + _dimFlangeHeight1)),
        new Fragment(
            new Point(-_dimFlangeWidth1 / 2, -_dimHeight / 2 + _dimFlangeHeight1),
            new Point(_dimFlangeWidth1 / 2, -_dimHeight / 2),
            new Point(-_dimFlangeWidth1 / 2, -_dimHeight / 2)),
        new Fragment(
            new Point(-_dimFlangeWidth1 / 2, -_dimHeight / 2 + _dimFlangeHeight1),
            new Point(_dimFlangeWidth1 / 2, -_dimHeight / 2 + _dimFlangeHeight1),
            new Point(_dimFlangeWidth1 / 2, -_dimHeight / 2)),
    };
    public override DimensionLabel[] DimensionLabels => new DimensionLabel[]
    {
        new DimensionLabel()
        {
            ShortName = "b",
            Name = "web width",
            Orientation = DimensionLabelOrientation.HorizontalDown,
            Level = 0,
            Pos1 = (float)-WebWidth / 2,
            Pos2 = (float)WebWidth / 2,
        },
        new DimensionLabel()
        {
            ShortName = "b1",
            Name = "flange width 1",
            Orientation = DimensionLabelOrientation.HorizontalDown,
            Level = 1,
            Pos1 = (float)-FlangeWidth1 / 2,
            Pos2 = (float)FlangeWidth1 / 2,
        },
        new DimensionLabel()
        {
            ShortName = "b2",
            Name = "flange width 2",
            Orientation = DimensionLabelOrientation.HorizontalUp,
            Level = 0,
            Pos1 = (float)-FlangeWidth2 / 2,
            Pos2 = (float)FlangeWidth2 / 2,
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
            Pos1 = (float)(Height / 2 - FlangeHeight1),
            Pos2 = (float)Height / 2,
        },
        new DimensionLabel()
        {
            ShortName = "h2",
            Name = "flange height 2",
            Orientation = DimensionLabelOrientation.VerticalRight,
            Level = 0,
            Pos1 = (float)-Height / 2,
            Pos2 = (float)(-Height / 2 + FlangeHeight2),
        },
    };

    public override double Width => Math.Max(_dimFlangeWidth1, _dimFlangeWidth2);
    public override double Height => _dimHeight;
    public double WebWidth => _dimWebWidth;
    public double FlangeHeight1 => _dimFlangeHeight1;
    public double FlangeHeight2 => _dimFlangeHeight2;
    public double FlangeWidth1 => _dimFlangeWidth1;
    public double FlangeWidth2 => _dimFlangeWidth2;


    public IProfileSectionModel() : base(iProfileDimensions)
    { }


    protected override bool CheckSectionValidity()
    {
        if (!base.CheckSectionValidity())
            return false;

        var err = "";
        if (_dimFlangeHeight1 + _dimFlangeHeight2 >= _dimHeight)
            err = "h1+h2 must be less than H";
        if (_dimWebWidth >= _dimFlangeWidth1)
            err = "b must be less than b1";
        if (_dimWebWidth >= _dimFlangeWidth1)
            err = "b must be less than b2";

        ErrorString = err;
        if (err == "")
            return true;

        return false;
    }
}
