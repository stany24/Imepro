using System;

namespace TeacherSoftware.Logic.MessageManager;

public class MessageReceivedEventArgs:EventArgs
{
    public string Message;
    public int StudentId;

    public MessageReceivedEventArgs(int studentId,string message)
    {
        StudentId = studentId;
        Message = message;
    }
}