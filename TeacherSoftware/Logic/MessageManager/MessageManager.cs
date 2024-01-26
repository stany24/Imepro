using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TeacherSoftware.Logic.MessageManager;

public class MessageManager
{
    private byte[] receivedBytes = new byte[10000];
    private bool _running;
    private List<Message> _messages = new();
    public EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public EventHandler<EventArgs>? MessageFailed;

    public MessageManager()
    {
        _running = true;
        Task.Run(Sender);
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
                int messageSize = message.TargetSocket.Receive(receivedBytes);
                byte[] messageBytes = new byte[messageSize];
                Array.Copy(receivedBytes,messageBytes,messageSize);
                Console.WriteLine(Encoding.Default.GetString(messageBytes));
                MessageReceived?.Invoke(null,new MessageReceivedEventArgs(message.StudentId,message.Type,Encoding.Default.GetString(messageBytes)));
                _messages.Remove(message);
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