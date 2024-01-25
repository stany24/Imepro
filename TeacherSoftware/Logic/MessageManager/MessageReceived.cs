using System;
using ClassLibrary6.Command;

namespace TeacherSoftware.Logic.MessageManager;

public class MessageReceived:EventArgs
{
    public string Message;
    public CommandType type;
    public int StudentId;

    public MessageReceived(int studentId,CommandType type,string message)
    {
        StudentId = studentId;
        Message = message;
    }
}