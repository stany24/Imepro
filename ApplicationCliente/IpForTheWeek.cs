using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace ApplicationCliente
{
    /// <summary>
    /// Classe qui contient toutes les adresses ip pour une semaine
    /// </summary>
    [Serializable]
    public static class IpForTheWeek
    {
        /// <summary>
        /// Fonction qui enregistre l'ip donnée au bonne endroit, qui dépand du jour et de l'heure de l'action
        /// </summary>
        /// <param name="ip"></param>
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
        /// Fonction qui retourne la bonne ip en fonction du jour et de l'heure de l'appel
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetIp()
        {
            Dictionary<string, string[]> Days = JsonSerializer.Deserialize<Dictionary<string, string[]>>(Properties.Settings.Default.IpForTheWeek);
            int BeforeAfterNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { BeforeAfterNoon = 1; }
            return IPAddress.Parse(Days[DateTime.Now.DayOfWeek.ToString()][BeforeAfterNoon]);
        }
    }
}
