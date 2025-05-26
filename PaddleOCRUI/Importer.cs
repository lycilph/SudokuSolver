using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Local;
using System.Diagnostics;

namespace PaddleOCRUI;

public class Importer
{
    private readonly PaddleOcrAll ocr;

    public Importer()
    {
        FullOcrModel model = LocalFullModels.EnglishV4;
        ocr = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
        {
            AllowRotateDetection = false,
            Enable180Classification = false,
        };
    }

    public void Recognize(Mat input)
    {
        PaddleOcrResult result = ocr.Run(input);

        var w = input.Size().Width / 9;
        var h = input.Size().Height / 9;
        var ow = w / 2;
        var oh = h / 2;

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                Cv2.Circle(input, new Point(x * w + ow, y * h + oh), 10, new Scalar(0, 255, 0), -1);
            }
        }

        Debug.WriteLine("Detected all texts: \n" + result.Text);
        foreach (PaddleOcrResultRegion region in result.Regions)
        {
            Debug.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");

            var width = (int)region.Rect.Size.Width;
            var height = (int)region.Rect.Size.Height;
            var cx = (int)region.Rect.Center.X;
            var cy = (int)region.Rect.Center.Y;
            var x = cx - width / 2;
            var y = cy - height / 2;
            var rect = new Rect(x, y, width, height);
            var scalar = new Scalar(255, 0, 0);

            if (!string.IsNullOrEmpty(region.Text))
            {
                Cv2.Rectangle(input, rect, scalar, 12);
                Cv2.PutText(input, region.Text, new Point(cx - 50, cy + 50), HersheyFonts.HersheyPlain, 10, scalar, 12);
            }
            else
                Cv2.Circle(input, new Point(cx, cy), 90, scalar, 10);
        }
    }

    public Mat Import(Mat input)
    {
        Mat output = new Mat();

        using (var t = new ResourcesTracker())
        {
            var gray = t.NewMat();
            Cv2.CvtColor(input, gray, ColorConversionCodes.RGB2GRAY);

            var threshold = t.NewMat();
            Cv2.AdaptiveThreshold(gray, threshold, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 55, 5);

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
            var contour_area_percentage_of_image_size = max_area / input_image_area * 100;

            Debug.WriteLine($"Found {contours.Length} contours in the image");
            Debug.WriteLine($"Largest area is {max_area} ({contour_area_percentage_of_image_size:f2}% of image) for contour {max_area_index}");

            // Find the bounding rectangle around the largest contour
            var bound = Cv2.BoundingRect(contours[max_area_index]);
            //Cv2.Rectangle(input, bound, new Scalar(255, 0, 0), 3);
            var bound_pts = new List<Point>
            {
                new(bound.X, bound.Y),
                new(bound.X + bound.Width, bound.Y),
                new(bound.X + bound.Width, bound.Y + bound.Height),
                new(bound.X, bound.Y + bound.Height)
            };

            // Approximate contour
            var bound_approx = Cv2.ApproxPolyDP(contours[max_area_index], 10, true);
            Debug.WriteLine($"Approximating grid with a {bound_approx.Length}-polygon");
            //foreach (var pt in bound_approx)
            //    Cv2.Circle(input, pt, 10, new Scalar(0, 255, 0), -1);

            // Find closest points on approximated largest contour
            var grid_pts = bound_pts.Select(p => GetClosestPoint(p, bound_approx)).ToList();
            //foreach (var pt in grid_pts)
            //    Cv2.Circle(input, pt, 10, new Scalar(0, 0, 255), -1);

            // Get perspective transform
            var size = 3000;
            var margin = 0;
            var src = new Point2f[] { grid_pts[0], grid_pts[1], grid_pts[2], grid_pts[3] };
            var dst = new Point2f[] { new(margin, margin), new(size - margin, margin), new(size - margin, size - margin), new(margin, size - margin) };
            var perspective = Cv2.GetPerspectiveTransform(src, dst);

            // Warp perspective
            Cv2.WarpPerspective(input, output, perspective, new Size(size, size), InterpolationFlags.Cubic);

            //Cv2.ImWrite("output.jpg", output);
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
}
