namespace LibraryData
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
    /// Class used to send UDP multicast messages.
    /// </summary>
    public class ReliableMulticastSender
    {
        #region Variables

        private const int DATA_SIZE = 64000;
        private int ScreenToShareId { get; set; }
        private int ImageNumber = 0;
        readonly private Socket SocketToSend;
        public bool Sending { get; set; }

        #endregion

        #region Constructor

        public ReliableMulticastSender(Socket socket, int screentoshareid)
        {
            ScreenToShareId = screentoshareid;
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
                byte[] ImageBytes = TakeScreenshot();
                int TotalImagePart = ImageBytes.Count() / DATA_SIZE + 1;
                for (int i = 0; i * DATA_SIZE < ImageBytes.Count(); i++)
                {
                    byte[] part = new byte[DATA_SIZE];
                    try { Array.Copy(ImageBytes, i * DATA_SIZE, part, 0, DATA_SIZE); }
                    catch
                    {
                        Array.Copy(ImageBytes, i * DATA_SIZE, part, 0, ImageBytes.Length - i * DATA_SIZE);
                        Array.Resize(ref part, ImageBytes.Length - i * DATA_SIZE);
                    }
                    ReliableMulticastMessage message = new(part, ImageNumber, i, TotalImagePart);
                    string custom = message.ToCustomString();
                    byte[] bytes = Encoding.Default.GetBytes(custom);
                    SocketToSend.Send(bytes);
                }

                ImageNumber++;
            }
        }

        public byte[] TakeScreenshot()
        {
            Screen screen = Screen.AllScreens[ScreenToShareId];
            Rectangle ScreenSize = screen.Bounds;
            Bitmap bitmap = new(ScreenSize.Width, ScreenSize.Height);
            Graphics.FromImage(bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
            ImageConverter converter = new();
            MemoryStream ms = new();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            AnyBitmap anybitmap = new(ms);
            return (byte[])converter.ConvertTo(anybitmap, typeof(byte[]));
        }

        #endregion
    }

    /// <summary>
    /// Class used to receive UDP multicast messages.
    /// </summary>
    public class ReliableMulticastReceiver
    {
        #region Variables

        public event EventHandler<NewImageEventArgs> NewImageEvent;
        readonly private List<ReliableImage> Images = new();
        readonly Socket SocketToReceive;
        public bool Receiving { get; set; }

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

        public void Receive()
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
            foreach (ReliableImage image in Images)
            {
                if (image.ImageNumber == message.ImageNumber) { image.AddData(message.Data, message.PartNumber); return; }
            }
            ReliableImage NewImage = new(message);
            NewImage.ImageCompletedEvent += DisplayImage;
            Images.Add(NewImage);
        }

        private void DisplayImage(object sender, ImageCompletedEventArgs e)
        {
            NewImageEvent.Invoke(sender,new NewImageEventArgs(e.CompletedImage));
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
            AnyBitmap bmp;
            using (var ms = new MemoryStream(imageData))
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
        public ImageCompletedEventArgs(AnyBitmap competedimage, int imageId)
        {
            CompletedImage = competedimage;
            ImageId = imageId;
        }

        public int ImageId { get; set; }
        public AnyBitmap CompletedImage { get; set; }
    }

    /// <summary>
    /// Event used to signal a new image has been received
    /// </summary>
    public class NewImageEventArgs : EventArgs
    {
        public AnyBitmap image { get; }
        public NewImageEventArgs(AnyBitmap newimage) { image = newimage; }
    }
}
