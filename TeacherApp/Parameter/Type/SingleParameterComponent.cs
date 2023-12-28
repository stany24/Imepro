namespace TeacherApp.Parameter.Type;

public class SingleParameterComponent:ParameterComponent
{
    public int Min { get; }
    public int Max { get; }
    private double _value;
    public double Value
    {
        get => _value;
        set
        {
            if (value > Max) { _value = Max;}
            if (value < Min) { _value = Min;}
            if (value<= Max && value >= Min) { _value = value;}
        }
    }

    public SingleParameterComponent(int min,double value,int max)
    {
        Min = min;
        Max = max;
        Value = value;
    }
}