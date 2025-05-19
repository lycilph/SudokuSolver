using System.Drawing;
using System.Text;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using CvEnum = Emgu.CV.CvEnum;

namespace ImageImporter;

public class Importer
{
    private readonly Tesseract tesseract;

    private StringBuilder sb = new();
    private string image_filename = string.Empty;

    // Input/Output variables
    private Image<Rgb, byte> input_image = null!;
    private Image<Rgb, byte> grid_image = null!;
    private Image<Rgb, byte> cells_image = null!;
    private List<Cell> cells = [];
    private List<Digit> digits = [];
    private CellsExtractionParameters current_cells_extraction_parameters;

    // Parameters for the algorithm(s)
    public int GridMargin { get; set; } = 0;
    public List<CellsExtractionParameters> CellsExtractionParameters { get; set; } = [new(5, 1), new(5, 3), new(2, 3)];
    public List<DigitExtractionParameters> DigitExtractionParameters { get; set; } = [new(5, 1, 1, 0), new(5, 1, 1, 1), new(5, 2, 1, 1), new(2, 3, 1, 1), new(3, 1, 1, 1), new(1, 5, 1, 1), new(5, 5, 1, 2)];

    public string Log => sb.ToString();
    public Image<Rgb, byte> InputImage => input_image;
    public Image<Rgb, byte> GridImage => grid_image;
    public Image<Rgb, byte> CellsImage => cells_image;
    public List<Digit> Digits => digits;

    public Importer()
    {
        tesseract = new Tesseract("tessdata", "eng", OcrEngineMode.LstmOnly, "123456789") { PageSegMode = PageSegMode.SingleChar };
    }

    public string GetPuzzle()
    {
        var chars = digits.Select(d => string.IsNullOrWhiteSpace(d.Text) ? "." : d.Text).ToArray();
        var str = string.Join("", chars);
        return str;
    }

    public void Import(string filename)
    {
        sb = new StringBuilder();

        ReadInputImage(filename);

        ExtractGrid(GridMargin);

        ExtractCells();
        if (cells.Count != 81)
            return;

        ExtractDigits();

        CleanupDigits();

        //CvInvoke.Imshow("Input", input_image.Resize(0.5, CvEnum.Inter.Cubic));
        //CvInvoke.Imshow("Grid", grid_image.Resize(0.8, CvEnum.Inter.Cubic));
        //CvInvoke.Imshow("Cells", cells_image.Resize(0.8, CvEnum.Inter.Cubic));
        //CvInvoke.WaitKey();
    }

    private void ReadInputImage(string filename)
    {
        image_filename = filename;
        if (!File.Exists(image_filename))
            throw new InvalidDataException($"The file {image_filename} doesn't exist");

        sb.AppendLine($"* Loading image from file {image_filename} *");
        input_image = new Image<Rgb, byte>(image_filename);
    }

    public void ExtractGrid(int margin)
    {
        sb.AppendLine($"* Extracting grid (margin={margin}) *");

        // Basic preprocessing of the image
        var img = input_image.Convert<Gray, byte>();
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
        var input_image_area = input_image.Width * input_image.Height;
        var contour_area_percentage_of_image_size = max_area / input_image_area * 100;
        sb.AppendLine($"Found {contours.Size} contours in the image");
        sb.AppendLine($"Largest area is {max_area} ({contour_area_percentage_of_image_size:f2}% of image) for contour {max_area_index}");

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
        sb.AppendLine($"Approximating grid with a {bound_approx.Size}-polygon");

        // Find closest points on approximated largest contour
        var grid_pts = bound_pts.Select(p => GetClosestPoint(p, bound_approx)).ToList();

        var grid = input_image.Clone();
        CvInvoke.DrawContours(grid, contours, max_area_index, new MCvScalar(0, 0, 255), 3);
        CvInvoke.Rectangle(grid, bound, new MCvScalar(255, 0, 0), 3);
        grid_pts.ForEach(p => CvInvoke.Circle(grid, p, 10, new MCvScalar(0, 255, 0), -1));

        // Get perspective transform
        var size = input_image.Width;
        var src = new PointF[] { grid_pts[0], grid_pts[1], grid_pts[2], grid_pts[3] };
        var dst = new PointF[] { new(margin, margin), new(size - margin, margin), new(size - margin, size - margin), new(margin, size - margin) };
        var perspective = CvInvoke.GetPerspectiveTransform(src, dst);

        // Warp perspective
        grid_image = new Image<Rgb, byte>(size, size);
        CvInvoke.WarpPerspective(input_image, grid_image, perspective, grid_image.Size, CvEnum.Inter.Cubic, CvEnum.Warp.Default, CvEnum.BorderType.Default);
    }

    public void ExtractCells()
    {
        sb.AppendLine($"* Extracting cells from image (trying all parameters) *");

        foreach (var parameters in CellsExtractionParameters)
        {
            ExtractCells(parameters);
            if (cells.Count == 81)
            {
                current_cells_extraction_parameters = parameters;
                sb.AppendLine($"Found {cells.Count} cells in the image ({current_cells_extraction_parameters})");
                break;
            }
        }
        if (cells.Count != 81)
            sb.AppendLine($"!!!Couldn't find 81 cells in the image!!!");
    }

    public void ExtractCells(CellsExtractionParameters parameters)
    {
        sb.AppendLine($"* Extracting cells from image ({parameters}) *");

        // Basic preprocessing of the image
        var img = grid_image.Convert<Gray, byte>();
        img._GammaCorrect(0.8);
        img = img.SmoothGaussian(7);
        img = img.ThresholdAdaptive(new Gray(255), CvEnum.AdaptiveThresholdType.GaussianC, CvEnum.ThresholdType.BinaryInv, 55, new Gray(parameters.Threshold));

        var kernel = CvInvoke.GetStructuringElement(CvEnum.ElementShape.Rectangle, new Size(9, 9), new Point(-1, -1));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), 1, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

        // Filter out small elements
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(img, contours, null, CvEnum.RetrType.Tree, CvEnum.ChainApproxMethod.ChainApproxSimple);
        var grid_image_area = grid_image.Size.Height * grid_image.Size.Width;
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
        sb.AppendLine($"Elements smaller than {area_threshold} is discarded (found {small_element_count} of these)");

        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), parameters.Iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
        CvInvoke.MorphologyEx(img, img, CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), parameters.Iterations, CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

        // Draw a fake border (in case not all of it is present)
        var border = new Rectangle(0, 0, grid_image.Width, grid_image.Height);
        CvInvoke.Rectangle(img, border, new MCvScalar(255, 255, 255), 3);

        // Find cells
        cells_image = grid_image.Clone();
        contours = new();
        var buffer = 0.5;
        var approx_cell_size_min = grid_image.Width * grid_image.Height / 81.0 * (1 - buffer);
        var approx_cell_size_max = grid_image.Width * grid_image.Height / 81.0 * (1 + buffer);
        var approx_image_size = grid_image.Width * grid_image.Height * (1.0 - buffer);
        var cells_count = 0;

        cells.Clear();
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

                cells.Add(new Cell { Image = grid_image.Copy(rect), Center = pt });
            }
            else if (area < approx_cell_size_min)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(0, 255, 0), 3);
            else if (area > approx_image_size)
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 0, 0), 3);
            else
                CvInvoke.DrawContours(cells_image, contours, i, new MCvScalar(255, 255, 0), 3);
        }
        sb.AppendLine($"Found {contours.Size} contours and {cells_count} cells (approx cell size is estimated to be between {approx_cell_size_min:f2} and {approx_cell_size_max:f2})");

        // Sort cells here (first top to bottom, then left to right)
        cells = cells
            .OrderBy(c => c.Center.Y)
            .Chunk(9)
            .SelectMany(c => c.OrderBy(c => c.Center.X))
            .ToList();

        for (int i = 0; i < cells.Count; i++)
            cells[i].Id = i;
    }

    public void ExtractDigits()
    {
        sb.AppendLine($"* Extracting digits from cells *");

        digits = cells
            .Select(c => ExtractDigit(c, DigitExtractionParameters.First()))
            .ToList();

        var digits_found = digits.Count(d => !string.IsNullOrEmpty(d.Text));
        var failures = digits.Count(d => d.RecognitionFailure);

        sb.AppendLine($"Found {digits_found} digits ({failures} failures)");
    }

    public Digit ExtractDigit(Cell cell, DigitExtractionParameters parameters)
    {
        sb.AppendLine($"* Extracting digit from cell {cell.Id} *");

        var digit = new Digit(cell);

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
        sb.AppendLine($"Cell {filled_percent:f2}% filled");

        if (filled_percent > 2.0) // To try do OCR if fill is over 2 percent
        {
            // Apparently a border makes the OCR work better (https://stackoverflow.com/questions/52823336/why-do-i-get-such-poor-results-from-tesseract-for-simple-single-character-recogn)
            CvInvoke.Rectangle(img, new Rectangle(0, 0, img.Width, img.Height), new MCvScalar(255, 255, 255), 5);

            // Resize and invert to improve recognition
            img = img.Resize(5, CvEnum.Inter.Cubic);
            img = img.Not();

            tesseract.SetImage(img);
            tesseract.Recognize();

            digit.Text = tesseract.GetUTF8Text().TrimEnd();
            digit.RecognitionFailure = string.IsNullOrWhiteSpace(digit.Text);

            if (!string.IsNullOrWhiteSpace(digit.Text))
                digit.Confidence = tesseract.GetWords().First().Confident;

            if (digit.Text.Length > 1)
                digit.RecognitionFailure = true;

            sb.AppendLine($"Recognized word for cell {cell.Id}: {digit.Text} (confidence={digit.Confidence}, failure={digit.RecognitionFailure})");
        }

        digit.ImageProcessed = img;

        return digit;
    }

    private void CleanupDigits()
    {
        sb.AppendLine($"* Cleaning up digits *");

        var cells_to_cleanup = digits.Where(d => d.RecognitionFailure || (!string.IsNullOrWhiteSpace(d.Text) && d.Confidence < 50.0)).ToList();

        foreach (var cell in cells_to_cleanup)
        {
            var alternatives = DigitExtractionParameters
                .Select(p => ExtractDigit(cell.Cell, p))
                .ToList();

            var best = alternatives
                .Where(d => !string.IsNullOrWhiteSpace(d.Text))
                .Where(d => !d.RecognitionFailure)
                .OrderByDescending(d => d.Confidence)
                .FirstOrDefault();

            if (best != null)
                digits[best.Cell.Id] = best;
        }
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
