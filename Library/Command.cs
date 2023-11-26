using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Library
{
    public class Command
    {
        [JsonInclude]
        public CommandType Type { get; set; }
        [JsonInclude]
        public List<string> Args { get; set; }

        [JsonConstructor]
        public Command(CommandType type, List<string> args)
        {
            Type = type;
            if (args != null) { Args = args; }
            else { Args = new List<string>(); }
        }
        public Command(CommandType type) { Type = type; Args = new List<string>(); }

        public IEnumerable<byte> ToByteArray()
        {
            return Encoding.Default.GetBytes(JsonSerializer.Serialize(this));
        }
        public override string ToString()
        {
            if (Args.Count == 0) { return Type.ToString(); }
            string str = Type.ToString() + "  Args:";
            Args.ForEach(arg => str += " " + arg);
            return str;
        }
    }
    public enum CommandType
    {
        DemandData,
        DemandImage,
        KillProcess,
        ReceiveMulticast,
        ApplyMulticastSettings,
        StopReceiveMulticast,
        ReceiveMessage,
        ReceiveAutorisedUrls,
        GiveControl,
        StopControl,
        DisconnectOfTeacher,
        StopApplication
    }
}
