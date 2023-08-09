using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibraryData
{
    /// <summary>
    /// Class containing the options for the stream
    /// </summary>
    [Serializable]
    public class StreamOptions
    {
        [JsonInclude]
        public Priority priority;
        [JsonInclude]
        public List<string> focus;
        public StreamOptions(Priority priority, List<string> focus)
        {
            this.priority = priority;
            this.focus = focus;
        }
    }

    public enum Priority
    {
        Widowed,
        Fullscreen,
        Topmost,
        Blocking
    }
}
