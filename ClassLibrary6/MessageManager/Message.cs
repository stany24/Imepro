using System.Net.Sockets;
using System.Text.Json.Serialization;
using ClassLibrary6.Command;
using TeacherSoftware.Logic.MessageManager;

namespace ClassLibrary6.MessageManager;

public class Message
{
    [JsonInclude]
    public string Content;
    [JsonInclude]
    public CommandType Type;
    public Socket TargetSocket;
    public int StudentId;
    public int NumberOfFailure = 0;
    public MessagePriority MessagePriority;
    public DateTime CreatedTime;
    
    public Message(string content,CommandType type,Socket studentSocket,int studentId)
    {
        Content = content;
        Type = type;
        TargetSocket = studentSocket;
        StudentId = studentId;
    }
}