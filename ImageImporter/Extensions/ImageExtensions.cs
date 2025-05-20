using Emgu.CV;

namespace ImageImporter.Extensions;

public static class ImageExtensions
{
    public static int Area<TColor, TDepth>(this Image<TColor, TDepth> img) where TColor : struct, IColor where TDepth : new()
    {
        return img.Width * img.Height;
    }
}
