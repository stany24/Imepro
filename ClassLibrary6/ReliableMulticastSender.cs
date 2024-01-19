using System.Net.Sockets;
using System.Text;

namespace ClassLibrary6;

/// <summary>
/// Class used to send UDP multicast messages.
/// </summary>
public class ReliableMulticastMessageSender
{
    #region Variables

    private const int DataSize = 64000;
    private int ScreenToShareId { get; set; }
    private int _imageNumber = 0;
    private readonly Socket _socketToSend;
    private readonly ScreenShotTaker _screenShotTaker = new();
    public bool Sending { get; set; }

    #endregion

    #region Constructor

    public ReliableMulticastMessageSender(Socket socket, int screenToShareId)
    {
        ScreenToShareId = screenToShareId;
        _socketToSend = socket;
        Sending = true;
        Task.Run(SendImages);
    }

    #endregion

    #region Image management

    private void SendImages()
    {
        while (Sending)
        {
            byte[] imageBytes = _screenShotTaker.TakeScreenShot(ScreenToShareId).ToByteArray();
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
                ReliableMulticastMessage message = new(part, _imageNumber, i, totalImagePart);
                string custom = message.ToCustomString();
                byte[] bytes = Encoding.Default.GetBytes(custom);
                _socketToSend.Send(bytes);
            }

            _imageNumber++;
        }
    }

    #endregion
}