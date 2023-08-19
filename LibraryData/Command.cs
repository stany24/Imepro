using System.Collections.Generic;

namespace LibraryData
{
    public class Command
    {
        private CommandType type { get; set; }
        List<string> args;

        public void AddArgument(string arg) { args.Add(arg); }
        public CommandType GetCommandType() { return type; }
        public List<string> GetArgs() { return args; }
        public Command(CommandType type, List<string> args) { this.type= type; this.args = args; }
        public Command(CommandType type) { this.type= type;}

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
