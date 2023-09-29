using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryData
{
    /// <summary>
    /// Class that handles sending and receiving a message 
    /// </summary>
    public class ReliableMulticastMessage
    {
        #region Variables

        readonly byte[] Data;
        readonly int ImageNumber;
        readonly int PartNumber;
        readonly int TotalPartNumber;

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

        private int ImageNumber = 0;
        private Socket SocketToSend;
        public bool sending { get; set; }

        #endregion

        public ReliableMulticastSender(Socket socket)
        {
            SocketToSend = socket;
            Task.Run(SendImages);
        }

        private void SendImages()
        {
            while (sending)
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
            return new byte[64000];
        }
    }

    public class ReliableMulticastReceiver
    {
        #region Variables

        readonly List<byte> remainder = new();
        readonly List<ReliableMulticastMessage> messages = new();
        readonly Socket SocketToReceive;

        public bool receiving { get; set; }

        #endregion

        #region Constructor

        public ReliableMulticastReceiver(Socket socket)
        {
            SocketToReceive = socket;
        }

        #endregion

        public void Receive()
        {
            while (receiving)
            {
                byte[] message = new byte[65000];
                int size = SocketToReceive.Receive(message);
                Array.Resize(ref message,size);
                ReliableMulticastMessage reliable = JsonSerializer.Deserialize<ReliableMulticastMessage>(message);

            }
        }
    }

    public class ReliableImage
    {
        private List<byte[]> ImageBytes = new();
        private int ImageNumber;
        private int TotalImagePart;
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
