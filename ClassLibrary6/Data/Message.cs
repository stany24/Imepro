using ClassLibrary6.Command;

namespace ClassLibrary6.Data;

public class Message(CommandType type, string content)
{
    public string Content { get; init; } = content;
    public CommandType Type { get; init; } = type;
}