using System.Collections.Generic;
using ClassLibrary6.StreamOptions;

namespace TeacherSoftware.Logic;

public class Properties
{
    public int TimeBetweenDemand { get; set; } = 15;
    public int DefaultTimeout { get; set; } = 1000;
    public int ScreenToShareId { get; set; } = 1;
    public List<string> AutorisedWebsites { get; set; } = new();
    public StreamOptions Options { get; set; } = new(Priority.Widowed,new List<string>());
}