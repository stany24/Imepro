using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClassLibrary6.Command;

namespace TeacherSoftware.Logic.MessageManager;

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