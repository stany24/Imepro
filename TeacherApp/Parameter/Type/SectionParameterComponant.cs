using System.Collections.Generic;

namespace TeacherApp.Parameter.Type;

public class SectionParameterComponent:ParameterComponent
{
    public SectionParameterComponent(Dictionary<string, ParameterComponent> components)
    {
        Components = components;
    }
}