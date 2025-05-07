using System.Drawing;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using CvEnum = Emgu.CV.CvEnum;

namespace ImageImportTest;

internal class Program
{
    static Tesseract tesseract = null!;

    static void Main(string[] args)
    {
        tesseract = new Tesseract("tessdata", "eng", OcrEngineMode.LstmOnly, "123456789")
        {
            PageSegMode = PageSegMode.SingleChar
        };
        Console.WriteLine($"Version: {Tesseract.Version}");

        //ProcessAll();
        ProcessSingle("C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\IMG_20250330_101246.jpg", 3);
    }

    private static void ProcessSingle(string filename, int iterations)
    {
        // Loop over all images in folder
        var img = new Image<Rgb, byte>(filename);
        var grid = ExtractGrid(img, true);
        (var cells_image, var cells, var cells_count) = ExtractCells(grid, 5, iterations, true);
        foreach (var cell in cells)
            ExtractDigit(cell);
    }

    private static void ProcessAll()
    {
        var path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\";
        var output_folder = "Output";
        foreach (var file in Directory.EnumerateFiles(path))
        {
            Console.WriteLine($"Processing file {file}");
            var filename = Path.GetFileNameWithoutExtension(file);
            var extension = Path.GetExtension(file);
            var grid_filename = $"{output_folder}\\{filename}_grid{extension}";
            var cells_filename = $"{output_folder}\\{filename}_cells{extension}";

            var img = new Image<Rgb, byte>(file);

            var grid = ExtractGrid(img);
            CvInvoke.Imwrite(grid_filename, grid);

            Image<Rgb, byte> cells_image = null!;
            List<Image<Rgb, byte>> cells = [];
            int cells_count = 0;
            for (int threshold = 5; threshold >= 2; threshold--)
            {
                for (int i = 1; i <= 7; i += 2)
                {
                    (cells_image, cells, cells_count) = ExtractCells(grid, threshold, i);

                    if (cells_count != 81)
                        Console.WriteLine($" * Error, found {cells_count} cells with {i} iterations and threshold {threshold}");
                    else
                        break;
                }
                if (cells_count == 81)
                    break;
            }

            if (cells_count == 81)
                Console.WriteLine($" * Successfully extracted cells from, saving image");
            else
                Console.WriteLine($" * !!! Failed to extracted cells from");

            // if it still fails here, try the ExtractCellsNaive method

            CvInvoke.Imwrite(cells_filename, cells_image);
        }
        Console.WriteLine("Press any key to continue!");
        Console.ReadKey();
    }

    //private static Image<Rgb, byte> ExtractCellsNaive(Image<Rgb, byte> source)
    //{
    //    var size = (int)(source.Size.Width / 9.0);
    //    var img = source.Clone();

    //    List<Image<Rgb, byte>> cells = [];
    //    for (int y = 0; y < 9; y++)
    //    {
    //        for (int x = 0; x < 9; x++)
    //        {
    //            var rect = new Rectangle(x * size, y * size, size, size);
    //            CvInvoke.Rectangle(img, rect, new MCvScalar(0,0,255),3);

    //            var cell = img.Copy(rect);
    //            cells.Add(cell);
    //            ExtractDigit(cell);
    //        }
    //    }
    //    //CvInvoke.Imshow("Cells", img.Resize(0.4, CvEnum.Inter.Cubic));
    //    //CvInvoke.Imshow("Cell0", cells[0]);
    //    //CvInvoke.WaitKey();

    //    return img;
    //}

    public static void ExtractDigit(Image<Rgb, byte> source)
    {
        // Basic preprocessing of the image
        var img = source.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(5));
        
        // Filter out small elements
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);
            Console.WriteLine($"Area {area}");
            if (area < 200)
                CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 0), -1);
        }
        Console.WriteLine($"Found {contours.Size} contours");

        var filled_percent = img.CountNonzero()[0] / (double)(source.Width * source.Height);
        Console.WriteLine($"Cells was filled {filled_percent} percentage");

        if (filled_percent > 0.02)
        {
            Console.WriteLine("Found a potential digit");

            // Apparently a border makes the OCR work better (https://stackoverflow.com/questions/52823336/why-do-i-get-such-poor-results-from-tesseract-for-simple-single-character-recogn)
            CvInvoke.Rectangle(img, new Rectangle(0, 0, img.Width, img.Height), new MCvScalar(255, 255, 255), 5);

            tesseract.SetImage(img);
            tesseract.Recognize();

            var digit = tesseract.GetUTF8Text().TrimEnd();
            Console.WriteLine($"Recognized word for cell: {digit}");

            CvInvoke.Imshow("Source", img);
            CvInvoke.WaitKey();
        }
    }

    private static (Image<Rgb, byte>, List<Image<Rgb, byte>>, int) ExtractCells(Image<Rgb, byte> source, int lower_threshold = 5, int iterations = 1, bool show_debug = false)
    {
        // Basic preprocessing of the image
        var img = source.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(lower_threshold));

        var kernel = CvInvoke.GetStructuringElement(CvEnum.ElementShape.Rectangle, new Size(9, 9), new Point(-1, -1));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

        if (show_debug)
        {
            CvInvoke.Imshow("Source", source.Resize(0.4, CvEnum.Inter.Cubic));
            CvInvoke.Imshow("Threshold", img.Resize(0.4, CvEnum.Inter.Cubic));
        }

        // Filter out small elements
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);
            if (show_debug)
                Console.WriteLine($"Area {area}");
            if (area < 500)
                CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 0), -1);
        }
        if (show_debug)
        {
            Console.WriteLine($"Found {contours.Size}");
            CvInvoke.Imshow("Filtered", img.Resize(0.4, CvEnum.Inter.Cubic));
        }

        //var kernel = CvInvoke.GetStructuringElement(CvEnum.ElementShape.Rectangle, new Size(9, 9), new Point(-1, -1));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        if (show_debug)
            CvInvoke.Imshow("Dilate", img.Resize(0.4, CvEnum.Inter.Cubic));

        // Draw a fake border (incase not all of it is present)
        var border = new Rectangle(0, 0, source.Width, source.Height);
        CvInvoke.Rectangle(img, border, new MCvScalar(255,255,255), 3);
        if (show_debug)
            CvInvoke.Imshow("Border", img.Resize(0.4, CvEnum.Inter.Cubic));

        // Find cells
        var cells_image = source.Clone();
        contours = new();
        var buffer = 0.5;
        var approx_cell_size_min = (source.Width * source.Height / 81.0) * (1 - buffer);
        var approx_cell_size_max = (source.Width * source.Height / 81.0) * (1 + buffer);
        var approx_image_size = (source.Width * source.Height) *(1.0 - buffer);
        var cells_count = 0;
        List<Image<Rgb, byte>> cells = [];
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);
            
            var bound = CvInvoke.BoundingRectangle(contours[i]);
            var aspect_ratio = bound.Width / (double)bound.Height;

            if (show_debug)
                Console.WriteLine($"Area of contour {i} is {area} and aspect ratio is {aspect_ratio}");

            if ((area > approx_cell_size_min && area < approx_cell_size_max) &&
                (aspect_ratio > 0.6 && aspect_ratio < 1.4))
            {
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(0, 0, 255), 3);
                cells_count++;

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

                cells.Add(source.Copy(rect));
            }
            else if (area <  approx_cell_size_min)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(0, 255, 0), 3);
            else if (area > approx_image_size)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 0, 0), 3);
            else
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 255, 0), 3);
        }
        if (show_debug)
            Console.WriteLine($"Found {contours.Size} contours and {cells_count} cells (approx cell size is estimated to be between {approx_cell_size_min} and {approx_cell_size_max})");

        if (show_debug)
        {
            CvInvoke.Imshow("Cells", cells_image.Resize(0.4, CvEnum.Inter.Cubic));
            CvInvoke.WaitKey();
        }

        return (cells_image, cells, cells_count);
    }

    private static Image<Rgb, byte> ExtractGrid(Image<Rgb, byte> source, bool show_debug = false)
    {
        // Basic preprocessing of the image
        var img = source.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(5));

        if (show_debug)
        {
            CvInvoke.Imshow("Source", source.Resize(0.4, CvEnum.Inter.Cubic));
            CvInvoke.Imshow("Threshold", img.Resize(0.4, CvEnum.Inter.Cubic));
        }

        // Find the largest contour in the image
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.External, CvEnum.ChainApproxMethod.ChainApproxSimple);
        var max_area = 0.0;
        var max_area_index = -1;
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);

            if (show_debug)
                Console.WriteLine($"Contour {i} area {area}");

            if (area > max_area)
            {
                max_area = area;
                max_area_index = i;
            }
        }
        var source_area = source.Width * source.Height;
        var contour_area_percentage_of_image_size = max_area / source_area * 100;
        if (show_debug)
            Console.WriteLine($"Largest area is {max_area} ({contour_area_percentage_of_image_size}% of image) for contour {max_area_index}");

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
            Console.WriteLine($"Grid poly {bound_approx.Size}");

        // Find closest points on approximated largest contour
        var grid_pts = bound_pts.Select(p => GetClosestPoint(p, bound_approx)).ToList();

        var grid = source.Clone();
        CvInvoke.DrawContours(grid, contours, max_area_index, new MCvScalar(0, 0, 255), 3);
        CvInvoke.Rectangle(grid, bound, new MCvScalar(255, 0, 0), 3);
        grid_pts.ForEach(p => CvInvoke.Circle(grid, p, 10, new MCvScalar(0, 255, 0), -1));
        if (show_debug)
            CvInvoke.Imshow("Grid", grid.Resize(0.4, CvEnum.Inter.Cubic));

        // Get perspective transform
        var margin = 0;
        var size = source.Width;
        var src = new PointF[] { grid_pts[0], grid_pts[1], grid_pts[2], grid_pts[3] };
        var dst = new PointF[] { new(margin, margin), new(size - margin, margin), new(size - margin, size - margin), new(margin, size - margin) };
        var perspective = CvInvoke.GetPerspectiveTransform(src, dst);

        // Warp perspective
        var source_perspective_corrected = new Image<Rgb, byte>(size, size);
        CvInvoke.WarpPerspective(source, source_perspective_corrected, perspective, source_perspective_corrected.Size, CvEnum.Inter.Cubic, CvEnum.Warp.Default, CvEnum.BorderType.Default);
        if (show_debug)
        {
            CvInvoke.Imshow("Perspective", source_perspective_corrected.Resize(0.4, CvEnum.Inter.Cubic));
            CvInvoke.WaitKey();
        }
        
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
