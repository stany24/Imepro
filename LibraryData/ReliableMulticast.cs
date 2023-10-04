using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryData
{
    /// <summary>
    /// Class that handles sending and receiving a message 
    /// </summary>
    public class ReliableMulticastMessage
    {
        public byte[] Data { get; }
        readonly public int ImageNumber;
        readonly public int PartNumber;
        readonly public int TotalPartNumber;
        private const string separator = "[";

        public ReliableMulticastMessage(string customstring)
        {
            string[] split = customstring.Split(separator[0]);
            int length = customstring.Length;
            ImageNumber = Convert.ToInt32(split[0]);
            PartNumber = Convert.ToInt32(split[1]);
            TotalPartNumber = Convert.ToInt32(split[2]);
            int HeaderLength = 3 + split[0].Length + split[1].Length + split[2].Length;
            Data = new byte[customstring.Length-HeaderLength];
            Array.Copy(Encoding.Default.GetBytes(customstring), HeaderLength, Data, 0, customstring.Length-HeaderLength);
        }

        public ReliableMulticastMessage(byte[] data, int imagenumber, int partnumber,int totalpartnumber)
        {
            Data = data;
            ImageNumber = imagenumber;
            PartNumber = partnumber;
            TotalPartNumber = totalpartnumber;
        }

        public string ToCustomString()
        {
            return ImageNumber +separator+PartNumber+separator+TotalPartNumber+separator+Encoding.Default.GetString(Data);
        }
    }

    public class ReliableMulticastSender
    {
        private const int DATA_SIZE = 64000;
        private int ScreenToShareId { get; set; }
        private int ImageNumber = 0;
        readonly private Socket SocketToSend;
        public bool Sending { get; set; }

        public ReliableMulticastSender(Socket socket,int screentoshareid)
        {
            ScreenToShareId = screentoshareid;
            SocketToSend = socket;
            Sending = true;
            Task.Run(SendImages);
        }

        private void SendImages()
        {
            while (Sending)
            {
                byte[] ImageBytes = TakeScreenshot();
                Bitmap bmp;
                using (var ms = new MemoryStream(ImageBytes))
                {
                    bmp = new Bitmap(ms);
                }

                int TotalImagePart = ImageBytes.Count() / DATA_SIZE + 1;
                for(int i = 0; i* DATA_SIZE < ImageBytes.Count(); i++)
                {
                    byte[] part = new byte[DATA_SIZE];
                    try { Array.Copy(ImageBytes, i * DATA_SIZE, part, 0, DATA_SIZE); }
                    catch {
                        Array.Copy(ImageBytes, i * DATA_SIZE, part, 0, ImageBytes.Length - i*DATA_SIZE);
                        Array.Resize(ref part, ImageBytes.Length - i*DATA_SIZE);
                    }
                    ReliableMulticastMessage message = new(part,ImageNumber,i,TotalImagePart);
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
            Bitmap bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format16bppRgb565);
            Rectangle ScreenSize = screen.Bounds;
            Graphics.FromImage(bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
            ImageConverter converter = new();
            return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
        }
    }

    public class ReliableMulticastReceiver
    {
        readonly private List<ReliableImage> Images = new();
        readonly private PictureBox pbxImage;
        readonly Socket SocketToReceive;
        public bool Receiving { get; set; }

        public ReliableMulticastReceiver(Socket socket, PictureBox pbxImage)
        {
            SocketToReceive = socket;
            this.pbxImage = pbxImage;
            Receiving = true;
            Task.Run(Receive);
        }

        public void Receive()
        {
            while (Receiving)
            {
                byte[] message = new byte[65000];
                int size = SocketToReceive.Receive(message);
                Array.Resize(ref message,size);
                ReliableMulticastMessage reliable = new(Encoding.Default.GetString(message));
                AddMessageToImage(reliable);
            }
        }

        private void AddMessageToImage(ReliableMulticastMessage message)
        {
            foreach(ReliableImage image in Images)
            {
                if (image.ImageNumber == message.ImageNumber) { image.AddData(message.Data,message.PartNumber);return; }
            }
            ReliableImage NewImage = new(message);
            NewImage.ImageCompletedEvent += DisplayImage;
            Images.Add(NewImage);
        }

        private void DisplayImage(object sender,ImageCompletedEventArgs e)
        {
            pbxImage.Image = e.CompletedImage;
            for(int i = 0;i<Images.Count;i++)
            {
                if (Images[i].ImageNumber <= e.ImageId) { Images.Remove(Images[i]); }
            }
        }
    }

    public class ReliableImage
    {
        readonly private byte[][] ImageBytes;
        readonly public int ImageNumber;
        public event EventHandler<ImageCompletedEventArgs> ImageCompletedEvent;

        public ReliableImage(ReliableMulticastMessage message)
        {
            ImageBytes = new byte[message.TotalPartNumber][];
            AddData(message.Data,message.PartNumber);
            ImageNumber = message.ImageNumber;
        }

        public void AddData(byte[] messagedata,int partnumber)
        {
            ImageBytes[partnumber] = messagedata;
            foreach(byte[] b in ImageBytes)
            {
                if(b == null) { return; }
            }
            ImageCompleted();
        }

        public void ImageCompleted()
        {
            byte[] imageData = ImageBytes.SelectMany(a => a).ToArray();
            Bitmap bmp;
            using (var ms = new MemoryStream(imageData))
            {
                bmp = new Bitmap(ms);
            }
            ImageCompletedEvent?.Invoke(this, new ImageCompletedEventArgs(bmp,ImageNumber));
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
