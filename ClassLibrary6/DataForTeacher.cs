using System.Net.Sockets;

namespace ClassLibrary6;

/// <summary>
/// Class that represent a student in the teacher application.
/// </summary>
public class DataForTeacher : Data
{
    #region Variables

    public Socket SocketToStudent { get; set; }
    public Socket SocketControl { get; set; }
    public int ID { get; set; }
    public int NumberOfFailure { get; set; }

    #endregion

    #region Constructor

    public DataForTeacher(Socket socket, int id)
    {
        SocketToStudent = socket;
        ID = id;
    }

    public DataForTeacher(Data data)
    {
        UserName = data.UserName;
        ComputerName = data.ComputerName;
        Urls = data.Urls;
        Processes = data.Processes;
    }

    #endregion
}