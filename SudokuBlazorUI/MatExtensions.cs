using System.Runtime.InteropServices;
using OpenCvSharp;
using SpawnDev.BlazorJS.JSObjects;

namespace SudokuBlazorUI;

public static class MatExtensions
{
    public static async Task LoadImageURL(this Mat mat, string url, string? crossOrigin = "anonymous")
    {
        using var image = await HTMLImageElement.CreateFromImageAsync(url, crossOrigin);
        using var canvas = new HTMLCanvasElement();
        using var context = canvas.Get2DContext();
        canvas.Width = image.Width;
        canvas.Height = image.Height;
        context.DrawImage(image, 0, 0);
        using var imageData = context.GetImageData(0, 0, image.Width, image.Height);
        using var uint8ClampedArray = imageData.Data;
        var rgbaBytes = uint8ClampedArray.ReadBytes();
        mat.Create(new Size(image.Width, image.Height), MatType.CV_8UC4);
        Marshal.Copy(rgbaBytes, 0, mat.DataStart, rgbaBytes.Length);
    }
}
