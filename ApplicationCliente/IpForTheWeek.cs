using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace ApplicationCliente
{
    /// <summary>
    /// Classe containing all ip for a week
    /// </summary>
    [Serializable]
    public static class IpForTheWeek
    {
        /// <summary>
        /// Function that save the given ip at the right place given the day and hour
        /// </summary>
        /// <param name="ip">the ip you want to save</param>
        public static void SetIp(string ip)
        {
            Dictionary<string, string[]> Days;
            try
            {
                Days = JsonSerializer.Deserialize<Dictionary<string, string[]>>(Properties.Settings.Default.IpForTheWeek);
            }catch(Exception) { Days = new(); }
            try { IPAddress.Parse(ip); }
            catch { return; }
            int BeforeAfterNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { BeforeAfterNoon = 1; }
            try{
                Days.Add(DateTime.Now.DayOfWeek.ToString(), new string[2]);
            }catch {  }
            Days[DateTime.Now.DayOfWeek.ToString()][BeforeAfterNoon] = ip;
            Properties.Settings.Default.IpForTheWeek = JsonSerializer.Serialize(Days);
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        /// <summary>
        /// Function that returns the ip for the day and hour
        /// </summary>
        /// <returns>the ip addresse for the teacher</returns>
        public static IPAddress GetIp()
        {
            Dictionary<string, string[]> Days = JsonSerializer.Deserialize<Dictionary<string, string[]>>(Properties.Settings.Default.IpForTheWeek);
            int BeforeAfterNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { BeforeAfterNoon = 1; }
            return IPAddress.Parse(Days[DateTime.Now.DayOfWeek.ToString()][BeforeAfterNoon]);
        }
    }
}
