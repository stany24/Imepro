using System.Net.Sockets;
using System.Text;
using IronSoftware.Drawing;
using ScreenCapture.NET;

namespace Library
{
    /// <summary>
    /// Class representing a UDP multicast message.
    /// </summary>
    public class ReliableMulticastMessage
    {
        #region Variables

        public byte[] Data { get; }
        public readonly int ImageNumber;
        public readonly int PartNumber;
        public readonly int TotalPartNumber;
        private const string Separator = "[";

        #endregion

        #region Constructor

        public ReliableMulticastMessage(string customString)
        {
            string[] split = customString.Split(Separator[0]);
            ImageNumber = Convert.ToInt32(split[0]);
            PartNumber = Convert.ToInt32(split[1]);
            TotalPartNumber = Convert.ToInt32(split[2]);
            int headerLength = 3 + split[0].Length + split[1].Length + split[2].Length;
            Data = new byte[customString.Length - headerLength];
            Array.Copy(Encoding.Default.GetBytes(customString), headerLength, Data, 0, customString.Length - headerLength);
        }

        public ReliableMulticastMessage(byte[] data, int imageNumber, int partnumber, int totalPartNumber)
        {
            Data = data;
            ImageNumber = imageNumber;
            PartNumber = partnumber;
            TotalPartNumber = totalPartNumber;
        }

        #endregion

        public string ToCustomString()
        {
            return ImageNumber + Separator + PartNumber + Separator + TotalPartNumber + Separator + Encoding.Default.GetString(Data);
        }
    }

    /// <summary>
    /// Class used to send UDP multicast messages.
    /// </summary>
    public class ReliableMulticastSender
    {
        #region Variables

        private const int DataSize = 64000;
        private int ScreenToShareId { get; set; }
        private int ImageNumber;
        private readonly Socket SocketToSend;
        private bool Sending { get; }

        #endregion

        #region Constructor

        public ReliableMulticastSender(Socket socket, int screenToShareId)
        {
            ScreenToShareId = screenToShareId;
            SocketToSend = socket;
            Sending = true;
            Task.Run(SendImages);
        }

        #endregion

        #region Image management

        private void SendImages()
        {
            while (Sending)
            {
                byte[] imageBytes = TakeScreenshot();
                int totalImagePart = imageBytes.Length / DataSize + 1;
                for (int i = 0; i * DataSize < imageBytes.Length; i++)
                {
                    byte[] part = new byte[DataSize];
                    try { Array.Copy(imageBytes, i * DataSize, part, 0, DataSize); }
                    catch
                    {
                        Array.Copy(imageBytes, i * DataSize, part, 0, imageBytes.Length - i * DataSize);
                        Array.Resize(ref part, imageBytes.Length - i * DataSize);
                    }
                    ReliableMulticastMessage message = new(part, ImageNumber, i, totalImagePart);
                    string custom = message.ToCustomString();
                    byte[] bytes = Encoding.Default.GetBytes(custom);
                    SocketToSend.Send(bytes);
                }

                ImageNumber++;
            }
        }

        private static byte[] TakeScreenshot()
        {
            // Create a screen-capture service
            IScreenCaptureService screenCaptureService = new X11ScreenCaptureService();

            // Get all available graphics cards
            IEnumerable<GraphicsCard> graphicsCards = screenCaptureService.GetGraphicsCards();

            // Get the displays from the graphics card(s) you are interested in
            IEnumerable<Display> displays = screenCaptureService.GetDisplays(graphicsCards.First());

            // Create a screen-capture for all screens you want to capture
            IScreenCapture screenCapture = screenCaptureService.GetScreenCapture(displays.First());
            
            // Register the regions you want to capture from the screen
            // Capture the whole screen
            ICaptureZone fullscreen = screenCapture.RegisterCaptureZone(0, 0, screenCapture.Display.Width, screenCapture.Display.Height);
            // Capture a 100x100 region at the top left and scale it down to 50x50
            screenCapture.CaptureScreen();
            using (fullscreen.Lock())
            {
                ReadOnlySpan<byte> rawData = fullscreen.RawBuffer;
                return rawData.ToArray();
            }
        }

        #endregion
    }

    /// <summary>
    /// Class used to receive UDP multicast messages.
    /// </summary>
    public class ReliableMulticastReceiver
    {
        #region Variables

        public event EventHandler<NewImageEventArgs>? NewImageEvent;
        private readonly List<ReliableImage> Images = new();
        private readonly Socket SocketToReceive;
        private bool Receiving { get; set; }

        #endregion

        #region Constructor

        public ReliableMulticastReceiver(Socket socket)
        {
            SocketToReceive = socket;
            Receiving = true;
            Task.Run(Receive);
        }

        #endregion

        #region Image management

        private void Receive()
        {
            while (Receiving)
            {
                byte[] message = new byte[65000];
                int size = SocketToReceive.Receive(message);
                Array.Resize(ref message, size);
                ReliableMulticastMessage reliable = new(Encoding.Default.GetString(message));
                AddMessageToImage(reliable);
            }
        }

        private void AddMessageToImage(ReliableMulticastMessage message)
        {
            foreach (ReliableImage image in Images.Where(image => image.ImageNumber == message.ImageNumber))
            {
                image.AddData(message.Data, message.PartNumber); return;
            }
            ReliableImage newImage = new(message);
            newImage.ImageCompletedEvent += DisplayImage;
            Images.Add(newImage);
        }

        private void DisplayImage(object? sender, ImageCompletedEventArgs e)
        {
            NewImageEvent?.Invoke(sender, new NewImageEventArgs(e.CompletedImage));
            for (int i = 0; i < Images.Count; i++)
            {
                if (Images[i].ImageNumber <= e.ImageId) { Images.Remove(Images[i]); }
            }
        }

        #endregion
    }

    /// <summary>
    /// Class representing the received image from UDP multicast.
    /// </summary>
    public class ReliableImage
    {
        #region Variables

        private readonly byte[][] ImageBytes;
        public readonly int ImageNumber;
        public event EventHandler<ImageCompletedEventArgs>? ImageCompletedEvent;

        #endregion

        #region Constructor

        public ReliableImage(ReliableMulticastMessage message)
        {
            ImageBytes = new byte[message.TotalPartNumber][];
            AddData(message.Data, message.PartNumber);
            ImageNumber = message.ImageNumber;
        }

        public void AddData(byte[] messageData, int partnumber)
        {
            ImageBytes[partnumber] = messageData;
            foreach (byte[] b in ImageBytes)
            {
                if (b == null) { return; }
            }
            ImageCompleted();
        }

        private void ImageCompleted()
        {
            byte[] imageData = ImageBytes.SelectMany(a => a).ToArray();
            AnyBitmap bmp;
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                bmp = new AnyBitmap(ms);
            }
            ImageCompletedEvent?.Invoke(this, new ImageCompletedEventArgs(bmp, ImageNumber));
        }

        #endregion
    }

    /// <summary>
    /// Event used to signal the completion of an image
    /// </summary>
    public class ImageCompletedEventArgs : EventArgs
    {
        public ImageCompletedEventArgs(AnyBitmap competedImage, int imageId)
        {
            CompletedImage = competedImage;
            ImageId = imageId;
        }

        public int ImageId { get; }
        public AnyBitmap CompletedImage { get;}
    }

    /// <summary>
    /// Event used to signal a new image has been received
    /// </summary>
    public class NewImageEventArgs : EventArgs
    {
        public AnyBitmap Image { get; }
        public NewImageEventArgs(AnyBitmap newImage) { Image = newImage; }
    }
}
