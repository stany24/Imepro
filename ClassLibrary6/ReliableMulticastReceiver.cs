using System.Net.Sockets;
using System.Text;

namespace ClassLibrary6;

/// <summary>
/// Class used to receive UDP multicast messages.
/// </summary>
public class ReliableMulticastMessageReceiver
{
    #region Variables

    public event EventHandler<NewImageEventArgs> NewImageEvent;
    readonly private List<ReliableImage> Images = new();
    readonly Socket SocketToReceive;
    public bool Receiving { get; set; }

    #endregion

    #region Constructor

    public ReliableMulticastMessageReceiver(Socket socket)
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
        NewImageEvent.Invoke(sender, new NewImageEventArgs(e.CompletedImage));
        for (int i = 0; i < Images.Count; i++)
        {
            if (Images[i].ImageNumber <= e.ImageId) { Images.Remove(Images[i]); }
        }
    }

    #endregion
}