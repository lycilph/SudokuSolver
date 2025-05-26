using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Local;

namespace PaddleOCRTest;

// https://github.com/sdcb/PaddleSharp/blob/master/docs/ocr.md

internal class Program
{
    /*static void Main()
    {
        var filename = "image1005.jpg";

        using (var t = new ResourcesTracker())
        {
            var input = t.T(new Mat(filename));
            var puzzle = ImageImporter.Import(input, ImportConfiguration.Default());
            Console.WriteLine(puzzle);
        }


        var txt_filename = Path.ChangeExtension(filename, "txt");
        var test_puzzle = File.ReadAllText(txt_filename);
        Console.WriteLine(test_puzzle);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        // https://github.com/sdcb/PaddleSharp/blob/master/docs/detection.md

        // This link https://www.nuget.org/packages/Sdcb.PaddleOCR
        // Points to this demo: https://paddlesharp-ocr.starworks.cc:88/ which finds all the numbers in the ...46 image
    }*/


    static void Main()
    {
        FullOcrModel model = LocalFullModels.EnglishV4;

        using (PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
        {
            AllowRotateDetection = false,
            Enable180Classification = false,
        })
        {
            using (Mat src = new Mat("IMG_20250330_101246_cropped.jpg"))
            {
                PaddleOcrResult result = all.Run(src);
                Console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                }
                Mat dest = PaddleOcrDetector.Visualize(src, result.Regions.Select(x => x.Rect).ToArray(), Scalar.Red, thickness: 2);
                Mat dest_resized = new Mat();
                Cv2.Resize(dest, dest_resized, new Size(800, 800));

                new Window("output", dest_resized);
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
