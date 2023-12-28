using System.Collections.Generic;

namespace TeacherApp.Parameter.Type;

public class ParameterComponent
{
    public Dictionary<string, ParameterComponent> Components { get; protected init; } = new();
}