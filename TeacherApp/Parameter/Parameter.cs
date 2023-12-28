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
        Sections.Add(
            "Jobs",
            new SectionParameterComponent(
                new Dictionary<string, ParameterComponent>
                {
                    {"Salary",new DualParameterComponent(100, 500, 5000, 5, 20, 200)}
                }
            )
        );
        Sections.Add(
            "Vehicles",
            new SectionParameterComponent(
                new Dictionary<string, ParameterComponent>
                {
                    {"PriceV",new DualParameterComponent(3000,15000,100000,2000,4000,10000)},
                    {"MaxAttemptV",new SingleParameterComponent(1,3,10)},
                    {"MinPriceNormalV",new SingleParameterComponent(4000,12000,120000)},
                    {"MinPriceExpensiveV",new SingleParameterComponent(10000,20000,1000000)}
                }
            )
        );
        Sections.Add(
            "Simulation",
            new SectionParameterComponent(
                new Dictionary<string, ParameterComponent>
                {
                    {"MaxDays",new SingleParameterComponent(1,1000,1000000)},
                    {"NoBuyDays",new SingleParameterComponent(0,5,1000000)},
                    {"HoursPerDay",new SingleParameterComponent(1,8,24)},
                    {"StartTargetMoney",new DualParameterComponent(100000,1000000,100000000,0,1000,100000)},
                    {"EnablePlayerDeath",new BooleanParameterComponent(true)}
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