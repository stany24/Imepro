namespace TeacherApp.Parameter.Type;

public class DualParameterComponent:ParameterComponent
{
    public int LowestMin{get;}
    public int HighestMin{get;}
    private double _min;
    public double Min
    {
        get => _min;
        set
        {
            if (value >= Max) { value = Max-1;}
            if (value > HighestMin) { _min = HighestMin;}
            if (value < LowestMin) { _min = LowestMin;}
            if (value<= HighestMin && value >= LowestMin) { _min = value;}
        }
    }
    
    public int HighestMax{get;}
    public int LowestMax{get;}
    private double _max;
    public double Max
    {
        get => _max;
        set
        {
            if (value <= Min) { value = Min+1;}
            if (value > HighestMax) { _max = HighestMax;}
            if (value < LowestMax) { _max = LowestMax;}
            if (value<= HighestMax && value >= LowestMax) { _max = value;}
        }
    }

    public DualParameterComponent(int lowestMax,double max,int highestMax,int lowestMin,double min,int highestMin)
    {
        LowestMin = lowestMin;
        HighestMin = highestMin;
        LowestMax = lowestMax;
        HighestMax = highestMax;
        _min = min;
        Max = max;
        Min = min;
    }
}