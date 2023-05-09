using BeamCalculator.Helpers.Drawing;
using BeamCalculator.Helpers.Geometry;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Globalization;

namespace BeamCalculator.Models.Section;


public partial class CustomSectionModel : SectionModel, ICustomSectionDrawable
{
    private static readonly DrawMapper<SectionDrawingOptions> defaultDrawMapper = new DrawMapper<SectionDrawingOptions>()
    {
        [nameof(ICustomSectionDrawable.DrawSectionFill)] = MapDrawSectionFill,
        [nameof(ICustomSectionDrawable.DrawSectionOutline)] = MapDrawSectionOutline,
        [nameof(ICustomSectionDrawable.DrawPointsLabels)] = MapDrawPointsLabels,
        [nameof(ICustomSectionDrawable.DrawInertiaAxes)] = MapDrawInertiaAxes,
        [nameof(ICustomSectionDrawable.DrawInertiaEllipse)] = MapDrawInertiaEllipse,
        [nameof(ICustomSectionDrawable.DrawSectionCore)] = MapDrawSectionCore,
    };
    private static readonly List<string> defaultLayerDrawingOrder = new List<string>()
    {
        nameof(ICustomSectionDrawable.DrawSectionFill),
        nameof(ICustomSectionDrawable.DrawSectionOutline),
        nameof(ICustomSectionDrawable.DrawInertiaEllipse),
        nameof(ICustomSectionDrawable.DrawSectionCore),
        nameof(ICustomSectionDrawable.DrawInertiaAxes),
        nameof(ICustomSectionDrawable.DrawPointsLabels),
    };

    private static void MapDrawSectionFill(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICustomSectionDrawable).DrawSectionFill(canvas, options);
    private static void MapDrawSectionOutline(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICustomSectionDrawable).DrawSectionOutline(canvas, options);
    private static void MapDrawPointsLabels(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICustomSectionDrawable).DrawPointsLabels(canvas, options);
    private static void MapDrawInertiaAxes(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICustomSectionDrawable).DrawInertiaAxes(canvas, options);
    private static void MapDrawInertiaEllipse(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICustomSectionDrawable).DrawInertiaEllipse(canvas, options);
    private static void MapDrawSectionCore(SectionModel section, SKCanvas canvas, SectionDrawingOptions options)
        => (section as ICustomSectionDrawable).DrawSectionCore(canvas, options);


    private List<Point> _points = new List<Point>()
    {
        new Point(-60,  60),
        new Point( 60,  60),
        new Point( 60, -60),
        new Point(-60, -60),
        new Point(-10,  10),
        new Point( 10,  10),
        new Point( 10, -10),
        new Point(-10, -10),
    };
    private List<List<int>> _contoursIndices = new List<List<int>>()
    {
        new List<int>()
        {
            0,1,2,3
        },
        new List<int>()
        {
            4,5,6,7
        },
    };


    public override SectionTypes Type => SectionTypes.Custom;

    public override List<Point> Points => _points;

    public override List<List<int>> ContoursIndices => _contoursIndices;

    public override List<Fragment> Fragments => new Triangulator().Triangulate(Points, ContoursIndices);


    public CustomSectionModel() : base(defaultDrawMapper, defaultLayerDrawingOrder)
    { }


    public int AddPoint(Point position, int contourIndex)
    {
        var contour = ContoursIndices[contourIndex];

        var edges = GeometryUtils.EdgesIndicesOrderedByDistanceToPoint(GetContour(contourIndex), position);

        foreach(var e in edges)
        {
            var addPointIndex = (e + 1) % contour.Count;
            var modifiedContour = GetModifiedContourAdd(contourIndex, addPointIndex, position);
            var modifiedEdges = new int[]
            {
                e,
                addPointIndex
            };

            if (SectionValidWithModifiedContour(contourIndex, modifiedContour, modifiedEdges) && GeometryUtils.IsContourClockwiseOriented(modifiedContour))
            {
                return InsertPoint(contourIndex, addPointIndex, position);
            }
        }

        return -1;
    }


    public void MovePoint(int pointIndex, Point position)
    {
        // point not moved
        if (position == Points[pointIndex]) return;

        // find point contour and point index in contour
        int movedIndexInContour;
        var contourIndex = FindPointContour(pointIndex, out movedIndexInContour);

        // get modified contour
        var modifiedContour = GetModifiedContourMove(contourIndex, movedIndexInContour, position);
        var modifiedEdges = new int[]
        {
            movedIndexInContour - 1 >= 0 ? movedIndexInContour - 1 : modifiedContour.Count - 1,
            movedIndexInContour
        };

        //if (!ModifiedContourValid(modifiedContour, modifiedEdges))
        if (!SectionValidWithModifiedContour(contourIndex, modifiedContour, modifiedEdges))
            return;

        if (!GeometryUtils.IsContourClockwiseOriented(modifiedContour))
            return;

        UpdatePoint(pointIndex, position);
    }

    public bool DeletePoint(int pointIndex)
    {
        int indexInContour;
        var contourIndex = FindPointContour(pointIndex, out indexInContour);

        if (ContoursIndices[contourIndex].Count <= 3) 
            return false;

        var modifiedContour = GetModifiedContourDelete(contourIndex, indexInContour);
        var modifiedEdges = new int[]
        {
            indexInContour - 1 >= 0 ? indexInContour - 1 : modifiedContour.Count - 1,
        };

        if (SectionValidWithModifiedContour(contourIndex, modifiedContour, modifiedEdges) && GeometryUtils.IsContourClockwiseOriented(modifiedContour))
        {
            RemovePoint(contourIndex, indexInContour);

            return true;
        }

        return false;
    }

    public int AddHole(Point position, double size)
    {
        if(!GeometryUtils.PointIsInsideContour(GetContour(0), position))
            return -1;

        var contour = new List<Point>() 
        { 
            new Point(position.X - size, position.Y + size),
            new Point(position.X, position.Y + size),
            position,
            new Point(position.X - size, position.Y),
        };
        if(SectionValidWithModifiedContour(-1, contour, new int[] { 0,1,2,3 }))
            return AddContour(contour);

        contour = new List<Point>()
        {
            new Point(position.X, position.Y + size),
            new Point(position.X + size, position.Y + size),
            new Point(position.X + size, position.Y),
            position,
        };
        if (SectionValidWithModifiedContour(-1, contour, new int[] { 0, 1, 2, 3 }))
            return AddContour(contour);

        contour = new List<Point>() 
        { 
            position,
            new Point(position.X + size, position.Y),
            new Point(position.X + size, position.Y - size),
            new Point(position.X, position.Y - size),
        };
        if (SectionValidWithModifiedContour(-1, contour, new int[] { 0, 1, 2, 3 }))
            return AddContour(contour);

        contour = new List<Point>() 
        { 
            new Point(position.X - size, position.Y),
            position,
            new Point(position.X, position.Y - size),
            new Point(position.X - size, position.Y - size),
        };
        if (SectionValidWithModifiedContour(-1, contour, new int[] { 0, 1, 2, 3 }))
            return AddContour(contour);

        return -1;
    }

    public void DeleteHole(int contourIndex)
    {
        if(contourIndex < 1) return;

        RemoveContour(contourIndex);
    }

    private bool SectionValidWithModifiedContour(int modContourIndex, List<Point> modContour, int[] modEdgesIndices)
    {
        double tolerance = 0.00001;

        Tuple<Point, Point>[] modEdgesPoints = new Tuple<Point, Point>[modEdgesIndices.Length];
        for (var i = 0; i < modEdgesIndices.Length; i++)
        {
            modEdgesPoints[i] = new Tuple<Point, Point>(
                modContour[modEdgesIndices[i]],
                modContour[(modEdgesIndices[i] + 1) % modContour.Count]);

            if (modEdgesPoints[i].Item1 == modEdgesPoints[i].Item2)
                return false;
        }

        for (var i = 0; i < ContoursIndices.Count; i++)
        {
            if(i != modContourIndex)
            {
                var contour = ContoursIndices[i];

                for (var j = 0; j < contour.Count; j++)
                {
                    var p2Ind = (j + 1) % contour.Count;

                    var p1 = Points[contour[j]];
                    var p2 = Points[contour[p2Ind]];

                    for (var n = 0; n < modEdgesPoints.Length; n++)
                    {
                        var intersection = new LinesSegmentsIntersection(
                            modEdgesPoints[n].Item1,
                            modEdgesPoints[n].Item2,
                            p1,
                            p2,
                            tolerance);

                        if (intersection.Intersection == IntersectionType.IntersectInsideSegmentsBounds ||
                            intersection.Intersection == IntersectionType.IntersectOnSegmentBoundary)
                            return false;
                    }
                }
            }
            else
            {
                for (int j = 0; j < modContour.Count; j++)
                {
                    if (modEdgesIndices.Contains(j))
                        continue;

                    var p2Ind = (j + 1) % modContour.Count;

                    var p1 = modContour[j];
                    var p2 = modContour[p2Ind];

                    for (var n = 0; n < modEdgesPoints.Length; n++)
                    {
                        var intersection = new LinesSegmentsIntersection(
                            modEdgesPoints[n].Item1,
                            modEdgesPoints[n].Item2,
                            p1, 
                            p2, 
                            tolerance);

                        if (intersection.Intersection == IntersectionType.IntersectInsideSegmentsBounds ||
                            intersection.Intersection == IntersectionType.CollinearWithOverlapping)
                            return false;

                        if ((intersection.Intersection == IntersectionType.IntersectOnSegmentBoundary || intersection.Intersection == IntersectionType.CollinearWithTouching) &&
                            p2Ind != modEdgesIndices[n] && j != (modEdgesIndices[n] + 1) % modContour.Count)
                            return false;
                    }
                }
            }
        }

        // if modified outer contour
        if(modContourIndex == 0)
        {
            // check if all holes placed inside outer contour
            for (var i = 1; i < ContoursIndices.Count; i++)
            {
                // since contours have no intersections, it is only necessary to
                // check that first point of each contour-hole is inside outer contour
                if(!GeometryUtils.PointIsInsideContour(modContour, Points[ContoursIndices[i][0]]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void UpdatePoint(int pointIndex, Point position)
    {
        _points[pointIndex] = position;
        OnPropertyChanged(nameof(Points));
        OnPropertyChanged(nameof(DrawingPoints));
    }

    private int InsertPoint(int contourIndex, int indexInContour, Point point)
    {
        _points.Add(point);
        _contoursIndices[contourIndex].Insert(indexInContour, Points.Count - 1);

        OnPropertyChanged(nameof(Points));
        OnPropertyChanged(nameof(DrawingPoints));

        return Points.Count - 1;
    }

    private void RemovePoint(int contourIndex, int indexInContour)
    {
        var pointInd = _contoursIndices[contourIndex][indexInContour];
        _contoursIndices[contourIndex].RemoveAt(indexInContour);
        _points.RemoveAt(pointInd);

        for(var i = 0; i < ContoursIndices.Count; i++)
        {
            for(var j = 0; j < ContoursIndices[i].Count; j++)
            {
                if (ContoursIndices[i][j] > pointInd)
                    ContoursIndices[i][j]--;
            }
        }

        OnPropertyChanged(nameof(Points));
        OnPropertyChanged(nameof(DrawingPoints));
    }

    private int AddContour(List<Point> points)
    {
        var contourIndices = new List<int>();

        for(var i = 0; i < points.Count; i++)
        {
            _points.Add(points[i]);
            contourIndices.Add(_points.Count - 1);
        }
        _contoursIndices.Add(contourIndices);

        OnPropertyChanged(nameof(Points));
        OnPropertyChanged(nameof(ContoursIndices));
        OnPropertyChanged(nameof(DrawingPoints));

        return _contoursIndices.Count - 1;
    }

    private void RemoveContour(int contourIndex)
    {
        for (int i = ContoursIndices[contourIndex].Count - 1; i >= 0; i--)
        {
            RemovePoint(contourIndex, i);
        }

        ContoursIndices.RemoveAt(contourIndex);

        OnPropertyChanged(nameof(Points));
        OnPropertyChanged(nameof(ContoursIndices));
        OnPropertyChanged(nameof(DrawingPoints));
    }

    private List<Point> GetModifiedContourMove(int contourIndex, int movePointIndexInContour, Point movedPoint)
    {
        var res = new List<Point>();

        for(var i = 0; i < ContoursIndices[contourIndex].Count; i++)
        {
            res.Add(i != movePointIndexInContour
                ? Points[ContoursIndices[contourIndex][i]]
                : movedPoint);
        }

        return res;
    }

    private List<Point> GetModifiedContourAdd(int contourIndex, int addPointIndexInContour, Point addedPoint)
    {
        var res = new List<Point>();

        for (var i = 0; i < ContoursIndices[contourIndex].Count; i++)
        {
            if (i == addPointIndexInContour)
                res.Add(addedPoint);

            res.Add(Points[ContoursIndices[contourIndex][i]]);
        }

        return res;
    }

    private List<Point> GetModifiedContourDelete(int contourIndex, int deletePointIndexInContour)
    {
        var res = new List<Point>();

        for (var i = 0; i < ContoursIndices[contourIndex].Count; i++)
        {
            if (i == deletePointIndexInContour)
                continue;

            res.Add(Points[ContoursIndices[contourIndex][i]]);
        }

        return res;
    }

    private int FindPointContour(int pointIndex, out int pointIndexInContour)
    {
        for (var i = 0; i < ContoursIndices.Count; i++)
        {
            for (var j = 0; j < ContoursIndices[i].Count; j++)
            {
                if (ContoursIndices[i][j] == pointIndex)
                {
                    pointIndexInContour = j;
                    return i;
                }
            }
        }

        pointIndexInContour = -1;
        return -1;
    }


    void ICustomSectionDrawable.DrawSectionFill(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = options.SectionFillColor,
        };
        using var hole = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Clear,
        };
        var points = DrawingPoints;

        for (var i = 0; i < ContoursIndices.Count; i++)
        {
            var path = new SKPath();
            for (var j = 0; j < ContoursIndices[i].Count; j++)
            {
                if (j == 0)
                    path.MoveTo(points[ContoursIndices[i][j]]);
                else
                    path.LineTo(points[ContoursIndices[i][j]]);
            }
            path.Close();

            canvas.DrawPath(path, i == 0 ? fill : hole);
        }
    }

    void ICustomSectionDrawable.DrawSectionOutline(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var stroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = _settings.SectionOutlineColor,
            StrokeWidth = options.SectionOutlineWidth / canvas.TotalMatrix.ScaleX,
        };
        var points = DrawingPoints;

        for (var i = 0; i < ContoursIndices.Count; i++)
        {
            var path = new SKPath();
            for (var j = 0; j < ContoursIndices[i].Count; j++)
            {
                if (j == 0)
                    path.MoveTo(points[ContoursIndices[i][j]]);
                else
                    path.LineTo(points[ContoursIndices[i][j]]);
            }
            path.Close();

            canvas.DrawPath(path, stroke);
        }
    }

    void ICustomSectionDrawable.DrawPointsLabels(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var text = new SKPaint()
        {
            TextSize = options.DimLabelTextSize / canvas.TotalMatrix.ScaleX,
            Color = SKColors.Black,
            TextAlign = SKTextAlign.Left,
            //IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", new SKFontStyle(SKFontStyleWeight.Normal, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright)),
        };
        var points = DrawingPoints;

        for (var i = 0; i < points.Count; i++)
        {
            var adjustedx = _points[i].X - Center.X;
            var adjustedy = _points[i].Y - Center.Y;

            var strX = adjustedx.ToString("F4", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
            var strY = adjustedy.ToString("F4", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
            
            var str = $"({strX}mm,{strY}mm)";

            var pos = points[i];
            pos.Y -= options.DimLabelTextPadding / canvas.TotalMatrix.ScaleX;

            canvas.DrawText(str, pos, text);
        }
    }

    void ICustomSectionDrawable.DrawInertiaAxes(SKCanvas canvas, SectionDrawingOptions options)
    {
        using var axisStroke = new SKPaint
        {
            Color = SKColors.Blue,
            StrokeWidth = options.DimLabelLineWidth * 2 / canvas.TotalMatrix.ScaleX,
            IsAntialias = true,
        };
        using var textPaint = new SKPaint
        {
            TextSize = options.DimLabelTextSize / canvas.TotalMatrix.ScaleX,
            Color = SKColors.Black,
            TextAlign = SKTextAlign.Left,
            Typeface = SKTypeface.FromFamilyName("Arial", new SKFontStyle(SKFontStyleWeight.Normal, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright)),
        };

        var b = canvas.LocalClipBounds;

        // calc axes arrows size and target points
        var axisSize = MathF.Min(b.Width, b.Height) / 10f;
        var pZ = new SKPoint(0, 0 - axisSize);
        var pY = new SKPoint(0 + axisSize, 0);

        // save matrix and rotate canvas
        var saved = canvas.TotalMatrix;
        canvas.RotateRadians((float)-Results.InertiaAxesAngle, 0, 0);

        // Z axis
        canvas.DrawArrow(
            new SKPoint(0, 0),
            pZ,
            13 / saved.ScaleX,
            27,
            axisStroke);

        // add padding between arrow and label
        pZ.Y -= options.DimLabelTextPadding / saved.ScaleX;

        // rotate canvas back around label drawing point
        canvas.Save();
        canvas.RotateRadians((float)Results.InertiaAxesAngle, pZ.X, pZ.Y);

        // draw label verticaly
        canvas.DrawText(
            "Z1",
            pZ,
            textPaint);

        // restore canvas state
        canvas.Restore();


        // Y axis
        axisStroke.Color = SKColors.Green;
        canvas.DrawArrow(
            new SKPoint(0, 0),
            pY,
            13 / saved.ScaleX,
            27,
            axisStroke);

        // add padding between arrow and label
        pY.Y -= options.DimLabelTextPadding / saved.ScaleX;

        // rotate canvas back around label drawing point
        canvas.Save();
        canvas.RotateRadians((float)Results.InertiaAxesAngle, pY.X, pY.Y);

        // draw label verticaly
        canvas.DrawText(
            "Y1",
            pY,
            textPaint);

        // restore matrix
        canvas.SetMatrix(saved);
    }

    void ICustomSectionDrawable.DrawInertiaEllipse(SKCanvas canvas, SectionDrawingOptions options)
    {
        var fill = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gold.WithAlpha(100),
        };
        var stroke = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            PathEffect = SKPathEffect.CreateDash(new float[] { 1, 1 }, 0),
            StrokeWidth = options.SectionOutlineWidth / canvas.TotalMatrix.ScaleX,
            Color = SKColors.Yellow,
        };

        var p = Results.MassCenter.ToSKPoint().InvertY();
        var s = new SKSize((float)Results.InertiaRadiusY, (float)Results.InertiaRadiusZ);
        canvas.DrawOval(p, s, fill);
        canvas.DrawOval(p, s, stroke);

        stroke.Color = SKColors.Red;
        canvas.DrawPoint(p, stroke);
    }

    void ICustomSectionDrawable.DrawSectionCore(SKCanvas canvas, SectionDrawingOptions options)
    {
        var fill = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.BlanchedAlmond.WithAlpha(100),
        };
        var stroke = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            PathEffect = SKPathEffect.CreateDash(new float[] { 1, 1 }, 0),
            StrokeWidth = options.SectionOutlineWidth / canvas.TotalMatrix.ScaleX,
            Color = SKColors.Brown,
        };
        var core = Results.SectionCore;

        var path = new SKPath();
        for (var i = 0; i < core.Count; i++)
            if (i == 0)
                path.MoveTo(core[i].ToSKPoint().InvertY());
            else
                path.LineTo(core[i].ToSKPoint().InvertY());
        path.Close();

        canvas.DrawPath(path, fill);
        canvas.DrawPath(path, stroke);
    }
}
