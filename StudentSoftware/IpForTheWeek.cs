using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace StudentSoftware
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
            Dictionary<string, string[]> Days = GetIpFromFile();
            int IsBeforeNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { IsBeforeNoon = 1; }
            if (!Days.ContainsKey(DateTime.Now.DayOfWeek.ToString())) { Days.Add(DateTime.Now.DayOfWeek.ToString(), new string[2]); }
            Days[DateTime.Now.DayOfWeek.ToString()][IsBeforeNoon] = ip;
            SaveIpToFile(Days);
        }

        /// <summary>
        /// Function that returns the ip for the day and hour.
        /// </summary>
        /// <returns>The ip addresse for the teacher.</returns>
        public static IPAddress GetIp()
        {
            Dictionary<string, string[]> Days = GetIpFromFile();
            int IsBeforeNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { IsBeforeNoon = 1; }
            try { return IPAddress.Parse(Days[DateTime.Now.DayOfWeek.ToString()][IsBeforeNoon]); }
            catch { return null; }
        }

        public static void Reset()
        {
            SaveIpToFile(new());
        }

        private static Dictionary<string, string[]> GetIpFromFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (!File.Exists(path)) { return new(); }
            string text = File.ReadAllText(path);
            if (text == string.Empty) { return new(); }
            Dictionary<string, string[]> dic = JsonSerializer.Deserialize<Dictionary<string, string[]>>(text);
            if(dic == null) { return new(); }
            return dic;
        }

        private static void SaveIpToFile(Dictionary<string, string[]> data)
        {
            if(data == null) { return; }
            string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            JsonSerializer.Serialize(data);
            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }
    }
}
