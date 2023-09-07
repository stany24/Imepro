using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace ApplicationCliente
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
            Dictionary<string, string[]> Days;
            try
            {
                Days = JsonSerializer.Deserialize<Dictionary<string, string[]>>(Properties.Settings.Default.IpForTheWeek);
            }
            catch (Exception) { Days = new(); }
            int IsBeforeNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { IsBeforeNoon = 1; }
            if (!Days.ContainsKey(DateTime.Now.DayOfWeek.ToString())) { Days.Add(DateTime.Now.DayOfWeek.ToString(), new string[2]); }
            Days[DateTime.Now.DayOfWeek.ToString()][IsBeforeNoon] = ip;
            Properties.Settings.Default.IpForTheWeek = JsonSerializer.Serialize(Days);
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        /// <summary>
        /// Function that returns the ip for the day and hour.
        /// </summary>
        /// <returns>The ip addresse for the teacher.</returns>
        public static IPAddress GetIp()
        {
            Dictionary<string, string[]> Days = JsonSerializer.Deserialize<Dictionary<string, string[]>>(Properties.Settings.Default.IpForTheWeek);
            int IsBeforeNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { IsBeforeNoon = 1; }
            return IPAddress.Parse(Days[DateTime.Now.DayOfWeek.ToString()][IsBeforeNoon]);
        }
    }
}
