using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using CvEnum = Emgu.CV.CvEnum;

namespace ImageImportUI;

public class ImageImporter
{
    static Tesseract tesseract = null!;

    public ImageImporter()
    {
        tesseract = new Tesseract("tessdata", "eng", OcrEngineMode.LstmOnly, "123456789") { PageSegMode = PageSegMode.SingleChar };
    }

    public int ExtractDigits(List<Cell> cells)
    {
        int recognition_failures = 0;
        foreach (Cell cell in cells)
            ExtractDigit(cell, ref recognition_failures);
        return recognition_failures;
    }

    public void ExtractDigit(Cell cell, ref int failures)
    {
        // Basic preprocessing of the image
        var img = cell.Image.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(5));
        cell.Processed = img;

        // Filter out small elements
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);
            if (area < 200)
                CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 0), -1);
        }
        
        var filled_percent = img.CountNonzero()[0] / (double)(cell.Image.Width * cell.Image.Height);
        //Console.WriteLine($"Cells was filled {filled_percent} percentage");

        if (filled_percent > 0.02)
        {
            // Apparently a border makes the OCR work better (https://stackoverflow.com/questions/52823336/why-do-i-get-such-poor-results-from-tesseract-for-simple-single-character-recogn)
            CvInvoke.Rectangle(img, new Rectangle(0, 0, img.Width, img.Height), new MCvScalar(255, 255, 255), 5);

            tesseract.SetImage(img);
            tesseract.Recognize();

            cell.Digit = tesseract.GetUTF8Text().TrimEnd();
            Debug.WriteLine($"Recognized word for cell: {cell.Digit}");

            if (string.IsNullOrWhiteSpace(cell.Digit))
                failures++;
        }
    }

    public (Image<Rgb, byte>, List<Cell>, int) ExtractCells(Image<Rgb, byte> source, int lower_threshold, int iterations, bool show_debug)
    {
        // Basic preprocessing of the image
        var img = source.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(lower_threshold));

        var kernel = CvInvoke.GetStructuringElement(CvEnum.ElementShape.Rectangle, new Size(9, 9), new Point(-1, -1));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

        // Filter out small elements
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);
            if (show_debug)
                Debug.WriteLine($"Area {area}");
            if (area < 500)
                CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 0), -1);
        }
        if (show_debug)
            Debug.WriteLine($"Found {contours.Size}");

        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

        // Draw a fake border (incase not all of it is present)
        var border = new Rectangle(0, 0, source.Width, source.Height);
        CvInvoke.Rectangle(img, border, new MCvScalar(255, 255, 255), 3);

        // Find cells
        var cells_image = source.Clone();
        contours = new();
        var buffer = 0.5;
        var approx_cell_size_min = (source.Width * source.Height / 81.0) * (1 - buffer);
        var approx_cell_size_max = (source.Width * source.Height / 81.0) * (1 + buffer);
        var approx_image_size = (source.Width * source.Height) * (1.0 - buffer);
        var cells_count = 0;
        List<Cell> cells = [];
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);

            var bound = CvInvoke.BoundingRectangle(contours[i]);
            var aspect_ratio = bound.Width / (double)bound.Height;

            if (show_debug)
                Debug.WriteLine($"Area of contour {i} is {area} and aspect ratio is {aspect_ratio}");

            if ((area > approx_cell_size_min && area < approx_cell_size_max) &&
                (aspect_ratio > 0.6 && aspect_ratio < 1.4))
            {
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(0, 0, 255), 3);
                cells_count++;

                var moments = CvInvoke.Moments(contours[i]);
                var pt = new Point((int)Math.Round(moments.GravityCenter.X), (int)Math.Round(moments.GravityCenter.Y));

                // Shrink bounding rect until all points are inside the contour
                var rect = CvInvoke.BoundingRectangle(contours[i]);
                var pts = new Point[]
                {
                    new Point(rect.X, rect.Y),
                    new Point(rect.X + rect.Width, rect.Y),
                    new Point(rect.X, rect.Y + rect.Height),
                    new Point(rect.X + rect.Width, rect.Y + rect.Height)
                };
                var dists = pts.Select(p => CvInvoke.PointPolygonTest(contours[i], p, true));
                var max_dist = (int)dists.Select(d => Math.Ceiling(Math.Abs(d))).Max();

                // Update bounding rect and cut out the cell
                rect.X += max_dist;
                rect.Y += max_dist;
                rect.Width -= 2 * max_dist;
                rect.Height -= 2 * max_dist;

                cells.Add(new Cell { Image = source.Copy(rect), Center = pt});
            }
            else if (area < approx_cell_size_min)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(0, 255, 0), 3);
            else if (area > approx_image_size)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 0, 0), 3);
            else
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 255, 0), 3);
        }
        if (show_debug)
            Debug.WriteLine($"Found {contours.Size} contours and {cells_count} cells (approx cell size is estimated to be between {approx_cell_size_min} and {approx_cell_size_max})");

        // Sort cells here...
        cells = cells
            .OrderBy(c => c.Center.Y)
            .Chunk(9)
            .SelectMany(c => c.OrderBy(c => c.Center.X))
            .ToList();
        
        return (cells_image, cells, cells_count);
    }

    public Image<Rgb, byte> ExtractGrid(Image<Rgb, byte> source, int margin, bool show_debug)
    {
        // Basic preprocessing of the image
        var img = source.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(5));

        // Find the largest contour in the image
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.External, CvEnum.ChainApproxMethod.ChainApproxSimple);
        var max_area = 0.0;
        var max_area_index = -1;
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);

            if (show_debug)
                Debug.WriteLine($"Contour {i} area {area}");

            if (area > max_area)
            {
                max_area = area;
                max_area_index = i;
            }
        }
        var source_area = source.Width * source.Height;
        var contour_area_percentage_of_image_size = max_area / source_area * 100;
        if (show_debug)
            Debug.WriteLine($"Largest area is {max_area} ({contour_area_percentage_of_image_size}% of image) for contour {max_area_index}");

        // Find the bounding rectangle around the largest contour
        var bound = CvInvoke.BoundingRectangle(contours[max_area_index]);
        var bound_pts = new List<Point>
        {
            new(bound.X, bound.Y),
            new(bound.X + bound.Width, bound.Y),
            new(bound.X + bound.Width, bound.Y + bound.Height),
            new(bound.X, bound.Y + bound.Height)
        };

        // Approximate contour
        var bound_approx = new VectorOfPoint();
        CvInvoke.ApproxPolyDP(contours[max_area_index], bound_approx, 5, true);
        if (show_debug)
            Debug.WriteLine($"Grid poly {bound_approx.Size}");

        // Find closest points on approximated largest contour
        var grid_pts = bound_pts.Select(p => GetClosestPoint(p, bound_approx)).ToList();

        var grid = source.Clone();
        CvInvoke.DrawContours(grid, contours, max_area_index, new MCvScalar(0, 0, 255), 3);
        CvInvoke.Rectangle(grid, bound, new MCvScalar(255, 0, 0), 3);
        grid_pts.ForEach(p => CvInvoke.Circle(grid, p, 10, new MCvScalar(0, 255, 0), -1));

        // Get perspective transform
        var size = source.Width;
        var src = new PointF[] { grid_pts[0], grid_pts[1], grid_pts[2], grid_pts[3] };
        var dst = new PointF[] { new(margin, margin), new(size - margin, margin), new(size - margin, size - margin), new(margin, size - margin) };
        var perspective = CvInvoke.GetPerspectiveTransform(src, dst);

        // Warp perspective
        var source_perspective_corrected = new Image<Rgb, byte>(size, size);
        CvInvoke.WarpPerspective(source, source_perspective_corrected, perspective, source_perspective_corrected.Size, CvEnum.Inter.Cubic, CvEnum.Warp.Default, CvEnum.BorderType.Default);
        
        // Return img with perspective corrected
        return source_perspective_corrected;
    }

    private static Point GetClosestPoint(Point p, VectorOfPoint points)
    {
        var min_dist = double.MaxValue;
        Point min_pt = p;
        for (var i = 0; i < points.Size; i++)
        {
            var dist = Dist(p, points[i]);
            if (dist < min_dist)
            {
                min_dist = dist;
                min_pt = points[i];
            }
        }

        return min_pt;
    }

    private static double Dist(Point p1, Point p2)
    {
        return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
    }
}
