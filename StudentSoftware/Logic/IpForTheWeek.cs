using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace StudentSoftware.Logic
{
    /// <summary>
    /// Class containing all ip for a week.
    /// </summary>
    [Serializable]
    public static class IpForTheWeek
    {
        /// <summary>
        /// Function that save the given ip at the right place given the day and hour.
        /// </summary>
        /// <param name="ip">The ip you want to save.</param>
        public static void SetIp(string ip)
        {
            try { IPAddress.Parse(ip); }
            catch { return; }
            Dictionary<string, string[]> days = GetIpFromFile();
            int isBeforeNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { isBeforeNoon = 1; }
            if (!days.ContainsKey(DateTime.Now.DayOfWeek.ToString())) { days.Add(DateTime.Now.DayOfWeek.ToString(), new string[2]); }
            days[DateTime.Now.DayOfWeek.ToString()][isBeforeNoon] = ip;
            SaveIpToFile(days);
        }

        /// <summary>
        /// Function that returns the ip for the day and hour.
        /// </summary>
        /// <returns>The ip addresse for the teacher.</returns>
        public static IPAddress? GetIp()
        {
            Dictionary<string, string[]> days = GetIpFromFile();
            int isBeforeNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { isBeforeNoon = 1; }
            try { return IPAddress.Parse(days[DateTime.Now.DayOfWeek.ToString()][isBeforeNoon]); }
            catch { return null; }
        }

        public static void Reset()
        {
            SaveIpToFile(new Dictionary<string, string[]>());
        }

        private static Dictionary<string, string[]> GetIpFromFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (!File.Exists(path)) { return new Dictionary<string, string[]>(); }
            string text = File.ReadAllText(path);
            if (text == string.Empty) { return new Dictionary<string, string[]>(); }
            Dictionary<string, string[]> dic = JsonSerializer.Deserialize<Dictionary<string, string[]>>(text);
            return dic ?? new Dictionary<string, string[]>();
        }

        private static void SaveIpToFile(Dictionary<string, string[]> data)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            JsonSerializer.Serialize(data);
            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }
    }
}
