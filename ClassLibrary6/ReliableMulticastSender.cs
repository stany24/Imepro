using System.Net.Sockets;
using System.Text;

namespace ClassLibrary6;

/// <summary>
/// Class used to send UDP multicast messages.
/// </summary>
public class ReliableMulticastMessageSender
{
    #region Variables

    private const int DATA_SIZE = 64000;
    private int ScreenToShareId { get; set; }
    private int ImageNumber = 0;
    readonly private Socket SocketToSend;
    private readonly ScreenShotTaker _screenShotTaker = new();
    public bool Sending { get; set; }

    #endregion

    #region Constructor

    public ReliableMulticastMessageSender(Socket socket, int screentoshareid)
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
            byte[] ImageBytes = _screenShotTaker.TakeScreenShot(ScreenToShareId).ToByteArray();
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

    #endregion
}