using System.Collections.Generic;
using System.Linq;

namespace TeacherApp.Parameter.Type;

public class ListParameterComponent<T>:ParameterComponent
{
    public List<T> SelectedValues { get; set; }//must stay public
    public List<T> AllValues { get; }//must stay public

    public ListParameterComponent(List<T> all, IEnumerable<T> selected)
    {
        AllValues = all;
        SelectedValues = new List<T>();
        foreach (T item in selected.Where(item => AllValues.Contains(item)))
        {
            SelectedValues.Add(item);
        }
    }
    
    public void SetSelectedValues(List<object> objects) // do not delete it is being used
    {
        SelectedValues.Clear();
        foreach (object obj in objects)
        {
            if (obj is T t)
            {
                SelectedValues.Add(t);
            }
        }
    }
}