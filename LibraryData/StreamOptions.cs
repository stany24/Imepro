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
        private Priority Priority { get; set; }
        [JsonInclude]
        private List<string> Focus { get; set; }
        public StreamOptions(Priority priority, List<string> focus)
        {
            this.Priority = priority;
            this.Focus = focus;
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
