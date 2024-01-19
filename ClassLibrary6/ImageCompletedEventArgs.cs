using ImageMagick;

namespace ClassLibrary6;

/// <summary>
/// Event used to signal the completion of an image
/// </summary>
public class ImageCompletedEventArgs : EventArgs
{
    public ImageCompletedEventArgs(MagickImage completedImage, int imageId)
    {
        CompletedImage = completedImage;
        ImageId = imageId;
    }

    public int ImageId { get; set; }
    public MagickImage CompletedImage { get; set; }
}