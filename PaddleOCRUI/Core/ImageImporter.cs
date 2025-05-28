using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;

namespace PaddleOCRUI.Core;

public static class ImageImporter
{
    public static string Import(Mat input, ImportConfiguration config)
    {
        string puzzle = string.Empty;

        // This should dispose of the grid after using it
        using (var grid_image = ExtractGrid(input, config))
        {
            var regions = RecognizeNumbers(grid_image, config);
            puzzle = MapNumbersToCells(regions, grid_image.Size());
        }

        return puzzle;
    }

    private static Mat ExtractGrid(Mat input, ImportConfiguration config)
    {
        Mat output = new();

        using (var t = new ResourcesTracker())
        {
            var gray = t.NewMat();
            Cv2.CvtColor(input, gray, ColorConversionCodes.RGB2GRAY);

            var threshold = t.NewMat();
            Cv2.AdaptiveThreshold(gray, threshold, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, config.BlockSize, config.Threshold);

            // Find the largest contour in the image
            Cv2.FindContours(threshold, out Point[][] contours, out HierarchyIndex[] hierarchies, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            var max_area = 0.0;
            var max_area_index = -1;
            var input_image_area = input.Size().Width * input.Size().Height;
            for (int i = 0; i < contours.Length; i++)
            {
                var area = Cv2.ContourArea(contours[i]);
                if (area > max_area)
                {
                    max_area = area;
                    max_area_index = i;
                }
            }

            // Find the bounding rectangle around the largest contour
            var bound = Cv2.BoundingRect(contours[max_area_index]);
            var bound_pts = new List<Point>
            {
                new(bound.X, bound.Y),
                new(bound.X + bound.Width, bound.Y),
                new(bound.X + bound.Width, bound.Y + bound.Height),
                new(bound.X, bound.Y + bound.Height)
            };

            // Approximate contour
            var bound_approx = Cv2.ApproxPolyDP(contours[max_area_index], 10, true);
            // Find closest points on approximated largest contour
            var grid_pts = bound_pts.Select(p => GetClosestPoint(p, bound_approx)).ToList();

            // Get perspective transform
            var size = config.GridOutputSize;
            var src = new Point2f[] { grid_pts[0], grid_pts[1], grid_pts[2], grid_pts[3] };
            var dst = new Point2f[] { new(0, 0), new(size, 0), new(size, size), new(0, size) };
            var perspective = Cv2.GetPerspectiveTransform(src, dst);

            // Warp perspective
            Cv2.WarpPerspective(gray, output, perspective, new Size(size, size), InterpolationFlags.Cubic);
        }

        return output;
    }

    private static Point GetClosestPoint(Point p, Point[] points)
    {
        var min_dist = double.MaxValue;
        Point min_pt = p;
        for (var i = 0; i < points.Length; i++)
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

    private static PaddleOcrResultRegion[] RecognizeNumbers(Mat input, ImportConfiguration config)
    {
        using (var ocr = new PaddleOcrAll(LocalFullModels.EnglishV4, PaddleDevice.Mkldnn())
        {
            AllowRotateDetection = false,
            Enable180Classification = false,
        })
        {
            PaddleOcrResult result = ocr.Run(input);

            //if (config.Debug)
            //{
            //    using (var t = new ResourcesTracker())
            //    {
            //        Mat dest = t.T(PaddleOcrDetector.Visualize(input, result.Regions.Select(x => x.Rect).ToArray(), Scalar.Red, thickness: 2));
            //        Mat resized = t.NewMat();
            //        Cv2.Resize(dest, resized, new Size(800, 800));
            //        t.T(new Window("output", resized));
            //        Cv2.WaitKey(0);
            //    }
            //}

            return result.Regions;
        }
    }

    private static string MapNumbersToCells(PaddleOcrResultRegion[] regions, Size grid_size)
    {
        var width = grid_size.Width / 9f;
        var height = grid_size.Height / 9f;
        var width_offset = width / 2f;
        var height_offset = height / 2f;

        // Create the cells
        List<Cell> cells = [];
        for (int i = 0; i < 81; i++)
        {
            int y = i / 9;
            int x = i % 9;

            cells.Add(new Cell() { Id = i, Center = new Point2f(x * width + width_offset, y * height + height_offset) });
        }

        foreach (var region in regions)
        {
            if (!string.IsNullOrEmpty(region.Text))
            {
                // Find the closest cell
                var cell = cells.MinBy(c => region.Rect.Center.DistanceTo(c.Center));
                if (cell != null)
                    cell.Text = region.Text;
            }
        }

        return string.Join("", cells.Select(c => string.IsNullOrEmpty(c.Text) ? "." : c.Text));
    }
}
