using ImageMagick;

namespace ClassLibrary6;

/// <summary>
/// Event used to signal a new image has been received
/// </summary>
public class NewImageEventArgs : EventArgs
{
    public MagickImage Image { get; }
    public NewImageEventArgs(MagickImage newImage) { Image = newImage; }
}