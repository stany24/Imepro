using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ClassLibrary6.Command;
using TeacherSoftware.Logic.MessageManager;

namespace ClassLibrary6.MessageManager;

public class StudentMessageManager
{
    private byte[] _receivedBytes = new byte[10000];
    private bool _running;
    private Socket _teacherSocket;
    public event EventHandler<MessageReceivedEventArgs> MessageReceived;
    public StudentMessageManager(Socket socket)
    {
        _teacherSocket = socket;
        Task.Run(Receive);
    }

    ~StudentMessageManager()
    {
        _running = false;
    }

    private void Receive()
    {
        while (_running)
        {
            int messageSize= _teacherSocket.Receive(_receivedBytes);
            byte[] messageBytes = new byte[messageSize];
            Array.Copy(_receivedBytes,messageBytes,messageSize);
            Message? message = JsonSerializer.Deserialize<Message>(Encoding.Default.GetString(messageBytes));
            if (message == null)
            {
                MessageReceived.Invoke(this,new MessageReceivedEventArgs(null,CommandType.Error,""));
                continue;
            }
            MessageReceived.Invoke(this,new MessageReceivedEventArgs(null,message.Type,message.Content));
        }
    }

    public void SendMessage(Message message)
    {
        message.TargetSocket.Send(Encoding.Default.GetBytes(JsonSerializer.Serialize(message)));
    }
}