using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibraryData
{
    public class Command
    {
        [JsonInclude]
        public CommandType type { get; set; }
        [JsonInclude]
        public List<string> args;

        public void AddArgument(string arg) { args.Add(arg); }
        [JsonConstructor]
        public Command(CommandType type, List<string> args) {
            this.type= type;
            if(args != null) { this.args = args; }
            else { this.args = new List<string>(); }
        }
        public Command(CommandType type) { this.type= type; this.args = new List<string>(); }

        public byte[] toByteArray()
        {
             return Encoding.Default.GetBytes(JsonSerializer.Serialize(this));
        }
        public override string ToString()
        {
            if(args.Count == 0) { return type.ToString(); }
            string str  = type.ToString() + "  Args:";
            args.ForEach(arg => str += " "+arg);
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
