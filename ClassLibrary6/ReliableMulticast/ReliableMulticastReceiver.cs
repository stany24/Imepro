using System.Net.Sockets;
using System.Text;

namespace ClassLibrary6.ReliableMulticast;

/// <summary>
/// Class used to receive UDP multicast messages.
/// </summary>
public class ReliableMulticastMessageReceiver
{
    #region Variables

    public event EventHandler<NewImageEventArgs> NewImageEvent;
    private readonly List<ReliableImage> _images = new();
    readonly Socket _socketToReceive;
    public bool Receiving { get; set; }

    #endregion

    #region Constructor

    public ReliableMulticastMessageReceiver(Socket socket)
    {
        _socketToReceive = socket;
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
            int size = _socketToReceive.Receive(message);
            Array.Resize(ref message, size);
            ReliableMulticastMessage reliable = new(Encoding.Default.GetString(message));
            AddMessageToImage(reliable);
        }
    }

    private void AddMessageToImage(ReliableMulticastMessage message)
    {
        foreach (ReliableImage image in _images.Where(image => image.ImageNumber == message.ImageNumber))
        {
            image.AddData(message.Data, message.PartNumber); return;
        }
        ReliableImage newImage = new(message);
        newImage.ImageCompletedEvent += DisplayImage;
        _images.Add(newImage);
    }

    private void DisplayImage(object sender, ImageCompletedEventArgs e)
    {
        NewImageEvent.Invoke(sender, new NewImageEventArgs(e.CompletedImage));
        for (int i = 0; i < _images.Count; i++)
        {
            if (_images[i].ImageNumber <= e.ImageId) { _images.Remove(_images[i]); }
        }
    }

    #endregion
}