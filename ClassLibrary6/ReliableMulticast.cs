using System.Text;
using ImageMagick;

namespace ClassLibrary6
{
    /// <summary>
    /// Class representing a UDP multicast message.
    /// </summary>
    public class ReliableMulticastMessage
    {
        #region Variables

        public byte[] Data { get; }
        readonly public int ImageNumber;
        readonly public int PartNumber;
        readonly public int TotalPartNumber;
        private const string separator = "[";

        #endregion

        #region Constructor

        public ReliableMulticastMessage(string customstring)
        {
            string[] split = customstring.Split(separator[0]);
            ImageNumber = Convert.ToInt32(split[0]);
            PartNumber = Convert.ToInt32(split[1]);
            TotalPartNumber = Convert.ToInt32(split[2]);
            int HeaderLength = 3 + split[0].Length + split[1].Length + split[2].Length;
            Data = new byte[customstring.Length - HeaderLength];
            Array.Copy(Encoding.Default.GetBytes(customstring), HeaderLength, Data, 0, customstring.Length - HeaderLength);
        }

        public ReliableMulticastMessage(byte[] data, int imagenumber, int partnumber, int totalpartnumber)
        {
            Data = data;
            ImageNumber = imagenumber;
            PartNumber = partnumber;
            TotalPartNumber = totalpartnumber;
        }

        #endregion

        public string ToCustomString()
        {
            return ImageNumber + separator + PartNumber + separator + TotalPartNumber + separator + Encoding.Default.GetString(Data);
        }
    }

    /// <summary>
    /// Class representing the received image from UDP multicast.
    /// </summary>
    public class ReliableImage
    {
        #region Variables

        readonly private byte[][] ImageBytes;
        readonly public int ImageNumber;
        public event EventHandler<ImageCompletedEventArgs> ImageCompletedEvent;

        #endregion

        #region Constructor

        public ReliableImage(ReliableMulticastMessage message)
        {
            ImageBytes = new byte[message.TotalPartNumber][];
            AddData(message.Data, message.PartNumber);
            ImageNumber = message.ImageNumber;
        }

        public void AddData(byte[] messagedata, int partnumber)
        {
            ImageBytes[partnumber] = messagedata;
            foreach (byte[] b in ImageBytes)
            {
                if (b == null) { return; }
            }
            ImageCompleted();
        }

        public void ImageCompleted()
        {
            byte[] imageData = ImageBytes.SelectMany(a => a).ToArray();
            MagickImage bmp = new MagickImage(imageData);
            ImageCompletedEvent?.Invoke(this, new ImageCompletedEventArgs(bmp, ImageNumber));
        }

        #endregion
    }

    /// <summary>
    /// Event used to signal the completion of an image
    /// </summary>
    public class ImageCompletedEventArgs : EventArgs
    {
        public ImageCompletedEventArgs(MagickImage competedimage, int imageId)
        {
            CompletedImage = competedimage;
            ImageId = imageId;
        }

        public int ImageId { get; set; }
        public MagickImage CompletedImage { get; set; }
    }

    /// <summary>
    /// Event used to signal a new image has been received
    /// </summary>
    public class NewImageEventArgs : EventArgs
    {
        public MagickImage image { get; }
        public NewImageEventArgs(MagickImage newimage) { image = newimage; }
    }
}
