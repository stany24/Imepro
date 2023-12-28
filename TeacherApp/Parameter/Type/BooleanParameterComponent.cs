namespace TeacherApp.Parameter.Type;

public class BooleanParameterComponent:ParameterComponent
{
    public bool Value { get; set; }

    public BooleanParameterComponent(bool isEnabled)
    {
        Value = isEnabled;
    }
}