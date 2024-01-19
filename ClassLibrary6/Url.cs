using System.Text.Json.Serialization;

namespace ClassLibrary6;

/// <summary>
/// Class representing an url.
/// </summary>
[Serializable]
public class Url
{
    [JsonInclude]
    readonly public DateTime ScreenShotTime;
    [JsonInclude]
    readonly public string Name;

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