using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassLibrary6.Command
{
    public class Command
    {
        [JsonInclude]
        public CommandType Type { get; set; }
        [JsonInclude]
        public List<string> Args { get; set; }

        [JsonConstructor]
        public Command(CommandType type, List<string>? args)
        {
            Type = type;
            Args = args ?? new List<string>();
        }
        public Command(CommandType type) { Type = type; Args = new List<string>(); }

        public IEnumerable<byte> ToByteArray()
        {
            return Encoding.Default.GetBytes(JsonSerializer.Serialize(this));
        }
        public override string ToString()
        {
            if (Args.Count == 0) { return Type.ToString(); }
            string str = Type + "  Args:";
            Args.ForEach(arg => str += " " + arg);
            return str;
        }
    }
}
