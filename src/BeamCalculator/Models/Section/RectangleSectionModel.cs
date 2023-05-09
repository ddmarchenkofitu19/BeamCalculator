using BeamCalculator.Helpers.Drawing;
using SkiaSharp;

namespace BeamCalculator.Models.Section;


public partial class RectangleSectionModel : CommonSectionModel
{
    private static readonly Dictionary<string, SectionDimensionData> rectangleDimensions = new Dictionary<string, SectionDimensionData>()
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
    };

    private double _dimWidth => _dimesions["width"].Value;
    private double _dimHeight => _dimesions["height"].Value;


    public override SectionTypes Type => SectionTypes.Rectangle;
    public override List<Point> Points => new List<Point>()
    {
        new Point(-_dimWidth / 2, _dimHeight / 2),
        new Point(_dimWidth / 2, _dimHeight / 2),
        new Point(_dimWidth / 2, -_dimHeight / 2),
        new Point(-_dimWidth / 2, -_dimHeight / 2),
    };
    public override List<List<int>> ContoursIndices => new List<List<int>>()
    {
        new List<int> { 0,1,2,3 },
    };
    public override List<Fragment> Fragments => new List<Fragment>()
    {
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2),
            new Point(_dimWidth / 2, -_dimHeight / 2),
            new Point(-_dimWidth / 2, -_dimHeight / 2)),
        new Fragment(
            new Point(-_dimWidth / 2, _dimHeight / 2),
            new Point(_dimWidth / 2, _dimHeight / 2),
            new Point(_dimWidth / 2, -_dimHeight / 2)),
    };


    public override DimensionLabel[] DimensionLabels => new DimensionLabel[] 
    {
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
            ShortName = "B",
            Name = "width",
            Orientation = DimensionLabelOrientation.HorizontalDown,
            Level = 0,
            Pos1 = (float)-Width / 2,
            Pos2 = (float)Width / 2,
        },
    };

    public override double Width => _dimWidth;
    public override double Height => _dimHeight;


    public RectangleSectionModel() : base(rectangleDimensions)
    { }
}
