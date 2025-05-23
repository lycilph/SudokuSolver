using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Local;

namespace PaddleOCRTest;

// https://github.com/sdcb/PaddleSharp/blob/master/docs/ocr.md

internal class Program
{
    static void Main()
    {
        var filename = "IMG_20250330_101246.jpg";

        using (var t = new ResourcesTracker())
        {
            var input = t.T(new Mat(filename));

            var gray = t.NewMat();
            Cv2.CvtColor(input, gray, ColorConversionCodes.RGB2GRAY);

            var threshold = t.NewMat();
            Cv2.AdaptiveThreshold(gray, threshold, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 55, 5);

            // Find the largest contour in the image
            Point[][] contours;
            HierarchyIndex[] hierarchies;
            Cv2.FindContours(threshold, out contours, out hierarchies, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
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

            Console.WriteLine($"Found {contours.Length} contours in the image");
            Console.WriteLine($"Largest area is {max_area} ({contour_area_percentage_of_image_size:f2}% of image) for contour {max_area_index}");

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
            var bound_approx = Cv2.ApproxPolyDP(contours[max_area_index], 500, true);
            Console.WriteLine($"Approximating grid with a {bound_approx.Length}-polygon");

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
            var output = t.NewMat();
            Cv2.WarpPerspective(gray, output, perspective, new Size(size, size), InterpolationFlags.Cubic);

            OCR(output);

            //t.T(new Window("output", output));
            //Cv2.WaitKey();
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
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

    private static void OCR(Mat img)
    {
        FullOcrModel model = LocalFullModels.EnglishV4;

        using (PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
        {
            AllowRotateDetection = false,
            Enable180Classification = false,
        })
        {
            using (Mat src = img.Clone())
            {
                PaddleOcrResult result = all.Run(src);
                Console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");

                    var width = (int)region.Rect.Size.Width;
                    var height = (int)region.Rect.Size.Height;
                    var cx = (int)region.Rect.Center.X;
                    var cy = (int)region.Rect.Center.Y;
                    var x = cx - width / 2;
                    var y = cy - height / 2;
                    var rect = new Rect(x, y, width, height);
                    var scalar = new Scalar(255, 0, 0);

                    Cv2.Rectangle(src, rect, scalar, 3);
                    Cv2.PutText(src, region.Text, new Point(cx, cy), HersheyFonts.HersheyPlain, 10, scalar, 3);
                }

                new Window("output", src);
                Cv2.WaitKey();
            }
        }

        /*private static void OCRTest()
        {
            FullOcrModel model = LocalFullModels.EnglishV4;

            using (PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
            {
                AllowRotateDetection = false,
                Enable180Classification = false,
            })
            {
                using (Mat src = Cv2.ImRead("image1005_cropped.jpg"))
                {
                    PaddleOcrResult result = all.Run(src);
                    Console.WriteLine("Detected all texts: \n" + result.Text);
                    foreach (PaddleOcrResultRegion region in result.Regions)
                    {
                        Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");

                        //var rect = new Rect2f(region.Rect.Center, region.Rect.Size);
                        var width = (int)region.Rect.Size.Width;
                        var height = (int)region.Rect.Size.Height;
                        var cx = (int)region.Rect.Center.X;
                        var cy = (int)region.Rect.Center.Y;
                        var x = cx - width / 2;
                        var y = cy - height / 2;
                        var rect = new Rect(x, y, width, height);
                        var scalar = new Scalar(255, 0, 0);

                        Cv2.Rectangle(src, rect, scalar, 3);
                        Cv2.PutText(src, region.Text, new Point(cx, cy), HersheyFonts.HersheyPlain, 10, scalar, 3);
                    }

                    Cv2.ImWrite("image1005_cropped_output.jpg", src);
                }
            }*/

        // Setup of nuget etc
        // https://github.com/sdcb/PaddleSharp/blob/master/docs/ocr.md

        // Basic usage of opencv
        // https://github.com/shimat/opencvsharp
        // https://shimat.github.io/opencvsharp_docs/html/531c29f5-962b-0700-999a-077acede649e.htm
        // Drawing rectangles
        // https://stackoverflow.com/questions/67603572/drawing-rectangles-using-opencvsharp
    }
}
