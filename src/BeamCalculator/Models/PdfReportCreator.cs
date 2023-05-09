using BeamCalculator.Helpers;
using BeamCalculator.Helpers.Drawing;
using BeamCalculator.Models.Section;
using SkiaSharp;
using System.Globalization;

namespace BeamCalculator.Models;


public class PdfReportCreator
{
    // 72 PPi - (595, 842), 150 PPI - (1240, 1754) 200 PPI - (1654, 2339), 300 PPI - (2480, 3508), 400 PPI - (3307, 4677)
    private const float PixelPerInch = 150f;
    private const float pageWidth = 1240;
    private const float pageHeight = 1754;
    private const float LeftPadding = 1f;
    private const float TopPadding = 0.5f;
    private const float RightPadding = 0.5f;
    private const float BottomPadding = 0.5f;
    private const float TextSizeTitle = 36f;
    private const float TextSizeHeader = 32f;
    private const float TextSizeParagraph = 28f;
    private const float TextSizeDetail = 20f;

    private readonly UserSettings _settings;

    private SectionModel _section;
    private SKDocument _pdf;
    private int _pageCount;
    private SKCanvas _pageCanvas;
    private float _currentY;

    private float PPI => PixelPerInch;

    public PdfReportCreator(SectionModel section)
	{
		_section = section;
        _settings = MauiProgram.Services.GetRequiredService<UserSettings>();
	}


	public string CreateFile()
	{
		//var path = Path.Combine(FileSystem.Current.CacheDirectory, Guid.NewGuid().ToString("D") + ".pdf");
		var path = Path.Combine(FileSystem.Current.CacheDirectory, "BeamCalculator_Section_Report.pdf");
        
        GeneratePdfDocument(path);
        
        StartNewPage();

        PrintDocTitle();

        AddSpacing(0.2f);

        PrintSection();

        AddSpacing(0.3f);

        PrintResultProperties();

		_pdf.EndPage();
		_pdf.Close();
        _pdf.Dispose();

        return path;
    }

	private void GeneratePdfDocument(string path)
	{
        var metadata = new SKDocumentPdfMetadata()
        {
            Author = "",
            Creator = "",
            Subject = "",
            Producer = "",
            Keywords = "",
            Creation = DateTime.Now,
            Modified = DateTime.Now,
            RasterDpi = PixelPerInch,
            EncodingQuality = SKDocumentPdfMetadata.DefaultEncodingQuality,
        };

        _pdf = SKDocument.CreatePdf(path, metadata);
    }

    private void StartNewPage()
    {
        _pdf.EndPage();
        _pageCanvas?.Dispose();
        _currentY = 0;

        var contentBounds = new SKRect(PPI * LeftPadding, PPI * TopPadding, pageWidth - (PPI * RightPadding), pageHeight - (PPI * BottomPadding));

        _pageCanvas = _pdf.BeginPage(pageWidth, pageHeight, contentBounds);

        _pageCount++;
        PrintPageNumber();
    }

    private void MoveCurrentY(float delta, bool skipIfNotFit = false)
    {
        var b = _pageCanvas.LocalClipBounds;
        if(delta > 0)
        {
            if(_currentY + delta > b.Bottom-1)
            {
                StartNewPage();

                if(!skipIfNotFit)
                    _currentY += delta;
            }
            else
                _currentY += delta;
        }
    }

    private void PrintPageNumber()
    {
        using var text = new SKPaint()
        {
            TextSize = TextSizeDetail,
            Color = SKColors.Gray,
        };

        var b = _pageCanvas.LocalClipBounds;
        var textSize = new SKRect();
        text.MeasureText(_pageCount.ToString(), ref textSize);
        var width = text.MeasureText(_pageCount.ToString());
        _pageCanvas.DrawText(
            _pageCount.ToString(), 
            new SKPoint(b.Right - width, b.Bottom), 
            text);
    }

    private void AddSpacing(float value)
    {
        MoveCurrentY(PPI * value, true);
    }

    private void PrintDocTitle()
    {
        var description = $"AppName, {DateTime.Now.ToString("dd.MMM.yyyy", CultureInfo.InvariantCulture)}";
        var title = _section.Type.ToUserFriendlyString() + " Section Properties Report";
        var b = _pageCanvas.LocalClipBounds;
        using var text = new SKPaint
        {
            TextSize = TextSizeDetail,
            Color = SKColors.Black
        };
        
        // measure description
        var textSize = new SKRect();
        text.MeasureText(description, ref textSize);
        MoveCurrentY(textSize.Height);

        // draw
        _pageCanvas.DrawText(description, 0, _currentY, text);

        AddSpacing(0.4f);

        // set text props
        text.TextSize = TextSizeTitle;
        text.FakeBoldText = true;
        // measure title
        text.MeasureText(title, ref textSize);
        MoveCurrentY(textSize.Height);
        var x = b.Width/2 - textSize.Width / 2;

        // draw
        _pageCanvas.DrawText(title, x, _currentY, text);
    }

    private void PrintSection()
    {
        var b = _pageCanvas.LocalClipBounds;
        var sectionBounds = new SKRect(1, _currentY, b.Right-1, _currentY + b.Height * 0.4f);
        var options = new SectionDrawingOptions()
        {
            SectionOutlineWidth = 5f,
            DimLabelLineWidth = 2f, 
            DimLabelTextSize = 42,
            DimLabelPaddingFromSection = 50f,
            PaddingBetweenDimLabels = 72f,
            DimLabelLineLedge = 16f,
            DimLabelTextPadding = 12f,
            DimLabelArrowLength = 40f,
            DimLabelHasValue = true,
            DimLabelHasShortName = false,
        };

        // draw border box
        using var line = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 0.5f,
        };
        _pageCanvas.DrawRect(sectionBounds, line);

        // create scale translate matrix to adjust section to bounds size and move (0,0) point in center
        var widthRatio = (sectionBounds.Width * 0.6f) / _section.Width;
        var heightRatio = (sectionBounds.Height * 0.6f) / _section.Height;
        var scale = (float)Math.Min(widthRatio, heightRatio);
        scale = Double.IsInfinity(scale) ? 1 : scale;
        var sectionMatr = SKMatrix.CreateScale(scale, scale);

        // save matrix
        var saved = _pageCanvas.TotalMatrix;
        
        // apply section matrix 
        _pageCanvas.SetMatrix(_pageCanvas.TotalMatrix.PreConcat(sectionMatr));
        var b1 = sectionMatr.Invert().MapRect(sectionBounds);

        // draw section
        _section.Draw(_pageCanvas, b1, options);

        // restore previous matrix
        _pageCanvas.SetMatrix(saved);

        MoveCurrentY(sectionBounds.Height);
    }

    private void PrintResultProperties()
    {
        using var text = new SKPaint()
        {
            TextSize = TextSizeHeader,
            Color = SKColors.Black,
            FakeBoldText = true,
        };
        using var line = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 1f,
        };
        var b = _pageCanvas.LocalClipBounds;
        var header = "Calculated Properties";

        // measure header
        var textSize = new SKRect();
        text.MeasureText(header, ref textSize);
        MoveCurrentY(textSize.Height);

        // draw header
        _pageCanvas.DrawText(header, 0, _currentY, text);

        AddSpacing(0.08f);

        // Draw header underline
        _pageCanvas.DrawLine(0, _currentY, b.Right, _currentY, line);
        MoveCurrentY(line.StrokeWidth);

        AddSpacing(0.25f);

        PrintPropertiesCategory(); // Basic Properties

        AddSpacing(0.4f);

        //PrintPropertiesCategory(); // Other Properties
    }

    private void PrintPropertiesCategory()
    {
        using var text = new SKPaint()
        {
            TextSize = TextSizeParagraph,
            Color = SKColors.Black,
            FakeBoldText = true,
        };
        var b = _pageCanvas.LocalClipBounds;
        var p = _section.Results;
        var categoryHeader = "Basic Properties";

        // measure header
        var textSize = new SKRect();
        text.MeasureText(categoryHeader, ref textSize);
        MoveCurrentY(textSize.Height);

        // draw header
        _pageCanvas.DrawText(categoryHeader, 0, _currentY, text);

        AddSpacing(0.2f);

        text.FakeBoldText = false;


        var outUnit = _settings.OutputDistanceUnitType.ToUserFriendlyString();

        PrintPropertieRow($"{nameof(p.Area)} = {p.AreaString} {outUnit}^2", text);
        PrintPropertieRow($"{nameof(p.InertiaMomentY)} = {p.InertiaMomentYString} {outUnit}^4", text);
        PrintPropertieRow($"{nameof(p.InertiaMomentZ)} = {p.InertiaMomentZString} {outUnit}^4", text);
        PrintPropertieRow($"{nameof(p.InertiaMomentYZ)} = {p.InertiaMomentYZString} {outUnit}^4", text);
        PrintPropertieRow($"{nameof(p.InertiaAxesAngle)} = {p.InertiaAxesAngleString} deg", text);
        PrintPropertieRow($"{nameof(p.MainInertiaMomentY)} = {p.MainInertiaMomentYString} {outUnit}^4", text);
        PrintPropertieRow($"{nameof(p.MainInertiaMomentZ)} = {p.MainInertiaMomentZString} {outUnit}^4", text);
    }

    private void PrintPropertieRow(string text, SKPaint paint)
    {
        // measure header
        var textSize = new SKRect();
        paint.MeasureText(text, ref textSize);
        MoveCurrentY(textSize.Height);

        // draw header
        _pageCanvas.DrawText(text, 0, _currentY, paint);

        AddSpacing(0.2f);
    }
}
