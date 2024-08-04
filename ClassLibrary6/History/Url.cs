using System.Text.Json.Serialization;

namespace ClassLibrary6.History;

/// <summary>
/// Class representing an url.
/// </summary>
[Serializable]
public class Url
{
    [JsonInclude]
    public readonly DateTime ScreenShotTime;

    [JsonInclude]
    public string Name { get; set; }

    public Url(DateTime screenShotTime, string name)
    {
        ScreenShotTime = screenShotTime;
        Name = name;
    }

    public override string ToString()
    {
        return ScreenShotTime.ToString("HH:mm:ss") + " " + Name;
    }
}