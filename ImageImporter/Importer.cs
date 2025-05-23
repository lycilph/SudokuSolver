using System.Drawing;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageImporter.Extensions;
using ImageImporter.Models;
using ImageImporter.Parameters;
using CvEnum = Emgu.CV.CvEnum;

namespace ImageImporter;

public class Importer
{
    private readonly Tesseract tesseract;

    public Importer()
    {
        tesseract = new Tesseract("tessdata", "eng", OcrEngineMode.LstmOnly, "123456789") { PageSegMode = PageSegMode.SingleChar };
    }

    public Puzzle Import(string filename, ImportParameters parameters)
    {
        var puzzle = new Puzzle(filename);

        ReadInputImage(puzzle);

        ExtractGrid(puzzle, parameters.GridParameters);

        ExtractCells(puzzle, parameters.CellsParameters);
        if (puzzle.Cells.Count == 81)
        {
            RecognizeNumbers(puzzle, parameters.NumberParameters.First());

            CleanupNumberss(puzzle, parameters.NumberParameters);
        }

        return puzzle;
    }

    private void ReadInputImage(Puzzle puzzle)
    {
        if (!File.Exists(puzzle.Filename))
            throw new InvalidDataException($"The file {puzzle.Filename} doesn't exist");

        puzzle.AppendDebugLog($"* Loading image from file {puzzle.Filename} *");
        puzzle.InputImage = new Image<Rgb, byte>(puzzle.Filename);

        puzzle.AppendDebugLog("");
    }

    public void ExtractGrid(Puzzle puzzle, GridExtractionParameters parameters)
    {
        puzzle.AppendDebugLog($"* Extracting grid ({parameters}) *");
        puzzle.Grid.ParametersUsed = parameters;

        // Basic preprocessing of the image
        var img = puzzle.InputImage.Convert<Gray, byte>();
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
            if (area > max_area)
            {
                max_area = area;
                max_area_index = i;
            }
        }
        var input_image_area = puzzle.InputImage.Area();
        var contour_area_percentage_of_image_size = max_area / input_image_area * 100;
        // Log debug information
        puzzle.AppendDebugLog($" * Found {contours.Size} contours in the image");
        puzzle.AppendDebugLog($" * Largest area is {max_area} ({contour_area_percentage_of_image_size:f2}% of image) for contour {max_area_index}");

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
        puzzle.AppendDebugLog($" * Approximating grid with a {bound_approx.Size}-polygon");

        // Find closest points on approximated largest contour
        var grid_pts = bound_pts.Select(p => GetClosestPoint(p, bound_approx)).ToList();

        var debug = puzzle.InputImage.Clone();
        CvInvoke.DrawContours(debug, contours, max_area_index, new MCvScalar(0, 0, 255), 3);
        CvInvoke.Rectangle(debug, bound, new MCvScalar(255, 0, 0), 3);
        grid_pts.ForEach(p => CvInvoke.Circle(debug, p, 10, new MCvScalar(0, 255, 0), -1));
        puzzle.Grid.DebugImage = debug;

        // Get perspective transform
        var size = parameters.OutputSize;
        var margin = parameters.Margin;
        var src = new PointF[] { grid_pts[0], grid_pts[1], grid_pts[2], grid_pts[3] };
        var dst = new PointF[] { new(margin, margin), new(size - margin, margin), new(size - margin, size - margin), new(margin, size - margin) };
        var perspective = CvInvoke.GetPerspectiveTransform(src, dst);

        // Warp perspective
        var output = new Image<Rgb, byte>(size, size);
        CvInvoke.WarpPerspective(puzzle.InputImage, output, perspective, output.Size, CvEnum.Inter.Cubic, CvEnum.Warp.Default, CvEnum.BorderType.Default);
        puzzle.Grid.Image = output;

        puzzle.AppendResultLog("Grid found");
        puzzle.AppendDebugLog("");
    }

    public void ExtractCells(Puzzle puzzle, List<CellsExtractionParameters> parameters_list)
    {
        puzzle.AppendDebugLog($"* Extracting cells from image (using {parameters_list.Count} parameter sets) *");

        foreach (var parameters in parameters_list)
        {
            ExtractCells(puzzle, parameters);
            if (puzzle.Cells.Count == 81)
            {
                puzzle.AppendDebugLog($" * Found {puzzle.Cells.Count} cells in the image (continuing)");
                break;
            }
        }

        if (puzzle.Cells.Count != 81)
            puzzle.AppendDebugLog($"   !!!Couldn't find 81 cells in the image!!!");

        puzzle.AppendResultLog($"{puzzle.Cells.Count} cells found");
        puzzle.AppendDebugLog("");
    }

    public void ExtractCells(Puzzle puzzle, CellsExtractionParameters parameters)
    {
        puzzle.AppendDebugLog($"* Extracting cells from image ({parameters}) *");
        puzzle.CellsExtraction.ParametersUsed = parameters;

        // Basic preprocessing of the image
        var img = puzzle.Grid.Image.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(parameters.Threshold));

        var kernel = CvInvoke.GetStructuringElement(CvEnum.ElementShape.Rectangle, new Size(9, 9), new Point(-1, -1));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

        // Filter out small elements
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        var grid_image_area = puzzle.Grid.Image.Area();
        var area_threshold = grid_image_area * 0.00006;
        var small_element_count = 0;
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);
            if (area < area_threshold)
            {
                CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 0), -1);
                small_element_count++;
            }
        }
        puzzle.AppendDebugLog($" * Elements smaller than {area_threshold} is discarded (found {small_element_count} of these)");

        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), parameters.Iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), parameters.Iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

        // Draw a fake border (in case not all of it is present)
        var width = puzzle.Grid.Image.Width;
        var height = puzzle.Grid.Image.Height;
        var border = new Rectangle(0, 0, width, height);
        CvInvoke.Rectangle(img, border, new MCvScalar(255, 255, 255), 3);

        // Find cells
        var cells_image = puzzle.Grid.Image.Clone();
        contours = new();
        var buffer = 0.5;
        var approx_cell_size_min = width * height / 81.0 * (1 - buffer);
        var approx_cell_size_max = width * height / 81.0 * (1 + buffer);
        var approx_image_size = width * height * (1.0 - buffer);
        var cells_count = 0;

        puzzle.Cells.Clear();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);

            var bound = CvInvoke.BoundingRectangle(contours[i]);
            var aspect_ratio = bound.Width / (double)bound.Height;

            if (area > approx_cell_size_min && area < approx_cell_size_max &&
                aspect_ratio > 0.6 && aspect_ratio < 1.4)
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

                // Skip this cell if the bounding rect becomes pathological
                if (rect.Width > 0 && rect.Height > 0)
                    puzzle.Cells.Add(new Cell { Image = puzzle.Grid.Image.Copy(rect), Center = pt, Bounds = rect });
            }
            else if (area < approx_cell_size_min)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(0, 255, 0), 3);
            else if (area > approx_image_size)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 0, 0), 3);
            else
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 255, 0), 3);
        }
        puzzle.CellsExtraction.Image = cells_image;
        puzzle.AppendDebugLog($" * Found {contours.Size} contours and {cells_count} cells (approx cell size is estimated to be between {approx_cell_size_min:f2} and {approx_cell_size_max:f2})");

        // Sanity check that no 2 cells are on top of each other (ie. that their center of gravity are too close)
        var invalid_cells = new List<Cell>();
        foreach (var cell in puzzle.Cells)
        {
            foreach (var other in puzzle.Cells)
            {
                if (cell != other && 
                    Dist(cell.Center, other.Center) <= 10 &&
                    !invalid_cells.Contains(cell) &&
                    !invalid_cells.Contains(other))
                    invalid_cells.Add(other);
            }
        }

        foreach (var cell in invalid_cells)
            puzzle.Cells.Remove(cell);

        // Sort cells here (first top to bottom, then left to right)
        puzzle.Cells = puzzle.Cells
            .OrderBy(c => c.Center.Y)
            .Chunk(9)
            .SelectMany(c => c.OrderBy(c => c.Center.X))
            .ToList();

        for (int i = 0; i < puzzle.Cells.Count; i++)
            puzzle.Cells[i].Id = i;

        // Debug image
        var debug = puzzle.Grid.Image.Clone();
        for (int i = 0; i < puzzle.Cells.Count; i++)
        {
            var str = puzzle.Cells[i].Id.ToString();
            var bound = puzzle.Cells[i].Bounds;
            var pt = puzzle.Cells[i].Center;

            pt.X -= 75;
            pt.Y += 50;

            CvInvoke.Rectangle(debug, bound, new MCvScalar(255, 0, 0), 3);
            CvInvoke.PutText(debug, str, pt, CvEnum.FontFace.HersheyPlain, 10, new MCvScalar(0, 0, 255), 10);
        }
        puzzle.CellsExtraction.DebugImage = debug;

        puzzle.AppendDebugLog("");
    }

    public void RecognizeNumbers(Puzzle puzzle, NumberRecognitionParameters parameters)
    {
        puzzle.AppendDebugLog($"* Recognizing numbers from cells *");

        puzzle.Numbers = puzzle
            .Cells
            .Select(c => RecognizeNumber(puzzle, c, parameters))
            .ToList();

        var numbers_found = puzzle.Numbers.Count(d => !string.IsNullOrEmpty(d.Text));
        var recognition_failures = puzzle.Numbers.Count(d => d.RecognitionFailure);

        puzzle.AppendResultLog($" * Found {numbers_found} numbers ({recognition_failures} failures)");
        puzzle.AppendDebugLog("");
    }

    public Number RecognizeNumber(Puzzle puzzle, Cell cell, NumberRecognitionParameters parameters)
    {
        puzzle.AppendDebugLog($"* Recognizing number from cell {cell.Id} *");
        var number = new Number(cell) { ParametersUsed = parameters };

        // Basic preprocessing of the image
        var img = cell.Image.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(parameters.Threshold));

        // Filter out small elements
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contours.Size; i++)
        {
            var area = CvInvoke.ContourArea(contours[i]);
            if (area < 200)
                CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 0), -1);
        }

        // Do extra processing based on the operations parameter
        if (parameters.Operation > 0)
        {
            var kernel = CvInvoke.GetStructuringElement(CvEnum.ElementShape.Ellipse, new Size(parameters.KernelSize, parameters.KernelSize), new Point(-1, -1));

            switch (parameters.Operation)
            {
                case 1:
                    CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), parameters.Iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
                    break;
                case 2:
                    CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Close, kernel, new Point(-1, -1), parameters.Iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
                    break;
            }
        }

        var filled_percent = img.CountNonzero()[0] / (double)(cell.Image.Width * cell.Image.Height) * 100.0;

        if (filled_percent > 2.0) // To try do OCR if fill is over 2 percent
        {
            // Apparently a border makes the OCR work better (https://stackoverflow.com/questions/52823336/why-do-i-get-such-poor-results-from-tesseract-for-simple-single-character-recogn)
            CvInvoke.Rectangle(img, new Rectangle(0, 0, img.Width, img.Height), new MCvScalar(255, 255, 255), 5);

            // Resize and invert to improve recognition
            img = img.Resize(5, CvEnum.Inter.Cubic);
            img = img.Not();

            tesseract.SetImage(img);
            tesseract.Recognize();

            number.Text = tesseract.GetUTF8Text().TrimEnd();
            number.RecognitionFailure = string.IsNullOrWhiteSpace(number.Text);
            number.ContainsNumber = true;

            if (!string.IsNullOrWhiteSpace(number.Text))
                number.Confidence = tesseract.GetWords().First().Confident;

            if (number.Text.Length > 1)
                number.RecognitionFailure = true;

            puzzle.AppendDebugLog($" * Recognized word for cell {cell.Id}: {number.Text} (confidence={number.Confidence}, failure={number.RecognitionFailure}, {filled_percent:f2}% filled)");
        }
        else
            puzzle.AppendDebugLog($" * Cell {filled_percent:f2}% filled (assuming no number in cell");
        
        number.ImageProcessed = img;
        return number;
    }

    private void CleanupNumberss(Puzzle puzzle, List<NumberRecognitionParameters> parameters_list)
    {
        puzzle.AppendDebugLog($"* Cleaning up digits *");

        var numbers_to_cleanup = 
            puzzle.Numbers
            .Where(d => d.RecognitionFailure || 
                        (!string.IsNullOrWhiteSpace(d.Text) && d.Confidence < 50.0))
            .ToList();

        foreach (var number in numbers_to_cleanup)
        {
            var alternatives = parameters_list
                .Select(p => RecognizeNumber(puzzle, number.Cell, p))
                .ToList();

            var best = alternatives
                .Where(d => !string.IsNullOrWhiteSpace(d.Text))
                .Where(d => !d.RecognitionFailure)
                .OrderByDescending(d => d.Confidence)
                .FirstOrDefault();

            if (best != null)
                puzzle.Numbers[best.Cell.Id] = best;
        }

        puzzle.AppendDebugLog("");
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
