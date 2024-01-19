using System.Text.Json.Serialization;
using ImageMagick;

namespace ClassLibrary6
{
    /// <summary>
    /// Class  that represent a basic student.
    /// </summary>
    [Serializable]
    public class Data
    {
        #region Variables

        [JsonInclude]
        public string UserName { get; set; }
        [JsonInclude]
        public string ComputerName { get; set; }
        [JsonInclude]
        public History Urls { get; set; }
        [JsonInclude]
        public Dictionary<int, string> Processes { get; set; }
        [JsonIgnore]
        public MagickImage ScreenShot { get; set; }

        #endregion

        #region Constructor

        public Data(string userName, string computerName, History urls, Dictionary<int, string> processes)
        {
            UserName = userName;
            ComputerName = computerName;
            Urls = urls;
            Processes = processes;
        }

        public Data()
        {
            Urls = new History();
            Processes = new Dictionary<int, string>();
        }

        #endregion
    }

    /// <summary>
    /// Event to signal a new message
    /// </summary>
    public class NewMessageEventArgs : EventArgs
    {
        public string Message { get; }
        public NewMessageEventArgs(string message) { Message = message; }
    }

    /// <summary>
    /// Event to signal a change of property is needed
    /// </summary>
    public class ChangePropertyEventArgs : EventArgs
    {
        public string ControlName { get; }
        public ControlType ControlType { get; }
        public string PropertyName { get; }
        public object PropertyValue { get; }

        public ChangePropertyEventArgs(string controlName, ControlType controlType, string propertyName, object propertyValue)
        {
            ControlName = controlName;
            ControlType = controlType;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }
    }

    public enum ControlType
    {
        Image,
        Window
    }
}
