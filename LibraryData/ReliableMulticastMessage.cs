using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryData
{
    /// <summary>
    /// Class that handles sending and receiving a message 
    /// </summary>
    public class ReliableMulticastMessage
    {
        #region Variables

        readonly public byte[] Data;
        readonly public int ImageNumber;
        readonly private int PartNumber;
        readonly public int TotalPartNumber;

        #endregion

        #region Constructor

        public ReliableMulticastMessage(byte[] data, int imagenumber, int partnumber,int totalpartnumber)
        {
            Data = data;
            ImageNumber = imagenumber;
            PartNumber = partnumber;
            TotalPartNumber = totalpartnumber;
        }

        #endregion
    }

    public class ReliableMulticastSender
    {
        #region Variables
        private int ScreenToShareId { get; set; }
        private int ImageNumber = 0;
        readonly private Socket SocketToSend;
        public bool Sending { get; set; }

        #endregion

        public ReliableMulticastSender(Socket socket,int screentoshareid)
        {
            ScreenToShareId = screentoshareid;
            SocketToSend = socket;
            Task.Run(SendImages);
        }

        private void SendImages()
        {
            while (Sending)
            {
                byte[] ImageBytes = TakeScreenshot();
                int TotalImagePart = ImageBytes.Count() / 64000 + 1;
                for(int i = 0; i*64000 < ImageBytes.Count(); i++)
                {
                    byte[] part = new byte[64000];
                    Array.Copy(ImageBytes, i * 64000, part, 0, 64000);
                    ReliableMulticastMessage message = new(TakeScreenshot(),ImageNumber,i,TotalImagePart);
                    string JsonMessage = JsonSerializer.Serialize(message);
                    SocketToSend.Send(Encoding.ASCII.GetBytes(JsonMessage));
                }
                ImageNumber++;
            }
        }

        private byte[] TakeScreenshot()
        {
            Screen screen = Screen.AllScreens[ScreenToShareId];
            Bitmap bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format16bppRgb565);
            Rectangle ScreenSize = screen.Bounds;
            Graphics.FromImage(bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
            ImageConverter converter = new();
            return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
        }
    }

    public class ReliableMulticastReceiver
    {
        #region Variables
        private List<ReliableImage> Images = new();
        readonly Socket SocketToReceive;

        public bool Receiving { get; set; }

        #endregion

        #region Constructor

        public ReliableMulticastReceiver(Socket socket)
        {
            SocketToReceive = socket;
        }

        #endregion

        public void Receive()
        {
            while (Receiving)
            {
                byte[] message = new byte[65000];
                int size = SocketToReceive.Receive(message);
                Array.Resize(ref message,size);
                ReliableMulticastMessage reliable = JsonSerializer.Deserialize<ReliableMulticastMessage>(message);
                AddMessageToImage(reliable);
            }
        }

        private void AddMessageToImage(ReliableMulticastMessage message)
        {
            foreach(ReliableImage image in Images)
            {
                if (image.ImageNumber == message.ImageNumber) { image.AddData(message.Data);return; }
            }
            Images.Add(new ReliableImage(message.Data, message.ImageNumber, message.TotalPartNumber));
        }
    }

    public class ReliableImage
    {
        readonly private List<byte[]> ImageBytes = new();
        readonly public int ImageNumber;
        readonly private int TotalImagePart;
        public event EventHandler<ImageCompletedEventArgs> ImgaeCompletedEvent;

        public ReliableImage(byte[] imageBytes, int imageNumber, int totalImagePart)
        {
            AddData(imageBytes);
            ImageNumber = imageNumber;
            TotalImagePart = totalImagePart;
        }

        public void AddData(byte[] messagedata)
        {
            ImageBytes.Add(messagedata);
            if(ImageBytes.Count == TotalImagePart) { ImageCompleted(); }
        }

        public void ImageCompleted()
        {
            byte[] imageData = ImageBytes.SelectMany(a => a).ToArray();
            Bitmap bmp;
            using (var ms = new MemoryStream(imageData))
            {
                bmp = new Bitmap(ms);
            }
            ImgaeCompletedEvent.Invoke(this, new ImageCompletedEventArgs(bmp,ImageNumber));
        }
    }

    public class ImageCompletedEventArgs : EventArgs
    {
        public ImageCompletedEventArgs(Bitmap competedimage, int imageId)
        {
            CompletedImage = competedimage;
            ImageId = imageId;
        }

        public int ImageId { get; set; }
        public Bitmap CompletedImage { get; set; }
    }
}
