using ImageMagick;

namespace ClassLibrary6;

/// <summary>
/// Class representing the received image from UDP multicast.
/// </summary>
public class ReliableImage
{
    #region Variables

    private readonly byte[][] _imageBytes;
    public readonly int ImageNumber;
    public event EventHandler<ImageCompletedEventArgs> ImageCompletedEvent;

    #endregion

    #region Constructor

    public ReliableImage(ReliableMulticastMessage message)
    {
        _imageBytes = new byte[message.TotalPartNumber][];
        AddData(message.Data, message.PartNumber);
        ImageNumber = message.ImageNumber;
    }

    public void AddData(byte[] messageData, int partnumber)
    {
        _imageBytes[partnumber] = messageData;
        foreach (byte[] b in _imageBytes)
        {
            if (b == null) { return; }
        }
        ImageCompleted();
    }

    private void ImageCompleted()
    {
        byte[] imageData = _imageBytes.SelectMany(a => a).ToArray();
        MagickImage bmp = new(imageData);
        ImageCompletedEvent.Invoke(this, new ImageCompletedEventArgs(bmp, ImageNumber));
    }

    #endregion
}