using ClassLibrary6.Command;

namespace TeacherSoftware.Logic.MessageManager;

public class MessageReceivedEventArgs:EventArgs
{
    public string Message;
    public CommandType type;
    public int? StudentId;


    public MessageReceivedEventArgs(int? studentId,CommandType type,string message)
    {
        StudentId = studentId;
        Message = message;
    }
}