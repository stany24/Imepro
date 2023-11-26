﻿using System.Text.Json.Serialization;

namespace Library
{
    /// <summary>
    /// Class containing the options for the stream
    /// </summary>
    [Serializable]
    public class StreamOptions
    {
        [JsonInclude]
        public readonly Priority Priority;
        [JsonInclude]
        public readonly List<string> Focus;
        public Priority GetPriority() { return Priority; }
        public List<string> GetFocus() { return Focus; }
        public StreamOptions(Priority priority, List<string> focus)
        {
            Priority = priority;
            Focus = focus;
        }
    }

    /// <summary>
    /// Class representing the priority of a stream on the student computer
    /// </summary>
    public enum Priority
    {
        Widowed,
        Fullscreen,
        Topmost,
        Blocking
    }
}
