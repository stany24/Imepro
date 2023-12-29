using System;
using System.Collections.Generic;
using TeacherApp.Parameter.Type;

namespace TeacherApp.Parameter;

public class Parameter
{
    public Dictionary<string, SectionParameterComponent> Sections { get; } = new();

    public Parameter()
    {
        Sections.Add(
            "Options"
            ,new SectionParameterComponent(
                new Dictionary<string, ParameterComponent>
                {
                    {"DefaultTimeout",new SingleParameterComponent(1,2,10)},
                    {"AuthorisedWebsite",new ListParameterComponent<string>(new List<string>{"github.com"},new List<string>{"github.com"})},
                }
            )
        );
    }

    public double GetMin(string section, string parameter)
    { 
        ParameterComponent? parameterComponent = GetParameter(section, parameter);

        if (parameterComponent is DualParameterComponent dual) return dual.Min;
        Console.WriteLine("Parameter: "+section+"/"+parameter+" doesn't exist.");
        return -double.NegativeInfinity;
    }
    
    public double GetMax(string section, string parameter)
    { 
        ParameterComponent? parameterComponent = GetParameter(section, parameter);

        if (parameterComponent is DualParameterComponent dual) return dual.Max;
        Console.WriteLine("Parameter: "+section+"/"+parameter+" doesn't exist.");
        return double.NegativeInfinity;

    }
    
    public double GetValue(string section, string parameter)
    { 
        ParameterComponent? parameterComponent = GetParameter(section, parameter);

        if (parameterComponent is SingleParameterComponent single) return single.Value;
        Console.WriteLine("Parameter: "+section+"/"+parameter+" doesn't exist.");
        return double.NegativeInfinity;
    }
    
    public bool GetBool(string section, string parameter)
    { 
        ParameterComponent? parameterComponent = GetParameter(section, parameter);

        if (parameterComponent is BooleanParameterComponent boolean) return boolean.Value;
        Console.WriteLine("Parameter: "+section+"/"+parameter+" doesn't exist.");
        return false;
    }

    public List<T> GetList<T>(string section, string parameter)
    {
        ParameterComponent? parameterComponent = GetParameter(section, parameter);
        if (parameterComponent == null || !parameterComponent.GetType().IsGenericType || parameterComponent.GetType().GetGenericTypeDefinition() != typeof(ListParameterComponent<>)) return new List<T>();
        dynamic list = parameterComponent;
        return list.SelectedValues;
    }

    private ParameterComponent? GetParameter(string section, string parameter)
    {
        return !Sections.TryGetValue(section, out SectionParameterComponent? sectionParameterComponent) ? null : sectionParameterComponent.Components.GetValueOrDefault(parameter);
    }
}