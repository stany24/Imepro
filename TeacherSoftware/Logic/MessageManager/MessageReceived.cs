using System;

namespace TeacherSoftware.Logic.MessageManager;

public class MessageReceived:EventArgs
{
    public string Message;
    public int StudentId;

    public MessageReceived(int studentId,string message)
    {
        StudentId = studentId;
        Message = message;
    }
}