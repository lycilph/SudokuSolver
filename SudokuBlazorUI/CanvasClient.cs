using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OpenCvSharp;
using SpawnDev.BlazorJS;
using System.Runtime.InteropServices;

namespace SudokuBlazorUI;

public class CanvasClient
{
    private readonly BlazorJSRuntime jsRuntime;
    private readonly ElementReference canvasElement;

    public CanvasClient(
        BlazorJSRuntime jsRuntime,
        ElementReference canvasElement)
    {
        this.jsRuntime = jsRuntime;
        this.canvasElement = canvasElement;
    }

    public void DrawPixels(byte[] pixels)
    {
        jsRuntime.CallVoid("drawPixels", canvasElement, pixels);
    }

    public void DrawMat(Mat mat)
    {
        Mat? rgba = null;
        try
        {
            var type = mat.Type();
            if (type == MatType.CV_8UC1)
                rgba = mat.CvtColor(ColorConversionCodes.GRAY2RGBA);
            else if (type == MatType.CV_8UC3)
                rgba = mat.CvtColor(ColorConversionCodes.BGR2RGBA);
            else if (type == MatType.CV_8UC4)
            {
                rgba = mat.Clone(); // No conversion needed, just clone
                Console.WriteLine("Mat is already in RGBA format, no conversion needed.");
            }
            else
                throw new ArgumentException($"Invalid mat type ({mat.Type()})");

            if (rgba == null || !rgba.IsContinuous())
                throw new InvalidOperationException("RGBA Mat should be continuous.");

            using (var temp = new Mat())
            {
                Cv2.Resize(rgba, temp, new Size(256, 256), interpolation: InterpolationFlags.Cubic);

                var length = (int)(temp.DataEnd.ToInt64() - temp.DataStart.ToInt64());
                var pixelBytes = new byte[length];
                Marshal.Copy(temp.DataStart, pixelBytes, 0, length);

                DrawPixels(pixelBytes);
            }
        }
        finally
        {
            rgba?.Dispose();
        }
    }
}
