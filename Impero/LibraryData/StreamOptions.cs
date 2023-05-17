using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace LibraryData
{
    /// <summary>
    /// Classe qui contient les options du stream
    /// </summary>
    [Serializable]
    public class StreamOptions
    {
        [JsonInclude]
        public Priority priority;
        [JsonInclude]
        public Focus focus;
        [JsonInclude]
        public List<string> AutorisedOpenedProcess;
        public StreamOptions(Priority priority, Focus focus, List<string> autorisedOpenedProcess)
        {
            this.priority = priority;
            this.focus = focus;
            AutorisedOpenedProcess = autorisedOpenedProcess;
        }
    }

    public enum Priority
    {
        Widowed,
        Fullscreen,
        Topmost,
        Blocking
    }

    public enum Focus
    {
        Everything,
        OneNote,
        VisualStudio,
        VSCode,
        Word,
    }
}
