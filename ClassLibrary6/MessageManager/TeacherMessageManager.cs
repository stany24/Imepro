using System.Text;
using System.Text.Json;
using ClassLibrary6.Command;
using TeacherSoftware.Logic.MessageManager;

namespace ClassLibrary6.MessageManager;

public class TeacherMessageManager
{
    private readonly byte[] _receivedBytes = new byte[10000];
    private bool _running;
    private readonly List<Message> _messages = new();
    public EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public EventHandler<EventArgs>? MessageFailed;

    public TeacherMessageManager()
    {
        _running = true;
        Task.Run(Sender);
    }

    ~TeacherMessageManager()
    {
        _running = false;
    }

    public void NewMessage(Message message)
    {
        message.CreatedTime = DateTime.Now;
        _messages.Add(message);
    }

    private void Sender()
    {
        while (_running)
        {
            if (_messages.Count > 0)
            {
                Message? message = GetMessageToSend();
                if(message == null){continue;}

                string serialized = JsonSerializer.Serialize(message);
                message.TargetSocket.Send(Encoding.Default.GetBytes(serialized));
                try
                {
                    int messageSize = message.TargetSocket.Receive(_receivedBytes);
                    byte[] messageBytes = new byte[messageSize];
                    Array.Copy(_receivedBytes,messageBytes,messageSize);
                    Console.WriteLine(Encoding.Default.GetString(messageBytes));
                    MessageReceived?.Invoke(null,new MessageReceivedEventArgs(message.StudentId,message.Type,Encoding.Default.GetString(messageBytes)));
                    _messages.Remove(message);
                }
                catch
                {
                    MessageReceived?.Invoke(null,new MessageReceivedEventArgs(message.StudentId,CommandType.Error,""));
                }
                
            }
            else
            {
                Thread.Sleep(10);
            }
        }
    }

    private Message? GetMessageToSend()
    {
       Message? message = _messages.FindAll(message => message.MessagePriority == MessagePriority.High).MinBy(message => message.CreatedTime);
       if (message != null) { return message; }
       message = _messages.FindAll(message => message.MessagePriority == MessagePriority.Normal).MinBy(message => message.CreatedTime);
       if (message != null) { return message; }
       message = _messages.FindAll(message => message.MessagePriority == MessagePriority.Low).MinBy(message => message.CreatedTime);
       return message;
    }
}