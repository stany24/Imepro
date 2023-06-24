using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibraryData
{
    /// <summary>
    /// Classe qui contient toutes les adresses ip pour une semaine
    /// </summary>
    [Serializable]
    public class IpForTheWeek
    {
        string[] days = { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
        [JsonInclude]
        public Dictionary<string, string[]> Days = new();


        /// <summary>
        /// Used to make a copy of another instance
        /// </summary>
        /// <param name="copy"></param>
        public IpForTheWeek(IpForTheWeek copy)
        {
            Days = copy.Days;
        }

        /// <summary>
        /// Used to make a new instance with all ip to 1.1.1.1
        /// </summary>
        public IpForTheWeek()
        {
            instanciate(IPAddress.Parse("1.1.1.1"));
        }

        /// <summary>
        /// Used to make a new instance with all ip to the given value
        /// </summary>
        /// <param name="ip">The value all ip will take</param>
        public IpForTheWeek(string ip)
        {
            instanciate(IPAddress.Parse(ip));
        }

        /// <summary>
        /// Fonction qui enregistre l'ip donnée au bonne endroit, qui dépand du jour et de l'heure de l'action
        /// </summary>
        /// <param name="ip"></param>
        public void SetIp(string ip)
        {
            try { IPAddress.Parse(ip); }
            catch { return; }

            DayOfWeek day = DateTime.Now.DayOfWeek;
            int MatinOuAprèsMidi = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { MatinOuAprèsMidi = 1; }
            Days[day.ToString().ToLower()][MatinOuAprèsMidi] = ip;
        }

        /// <summary>
        /// Fonction qui retourne la bonne ip en fonction du jour et de l'heure de l'appel
        /// </summary>
        /// <returns></returns>
        public string GetIp()
        {
            DayOfWeek day = DateTime.Now.DayOfWeek;
            int MatinOuAprèsMidi = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { MatinOuAprèsMidi = 1; }
            return Days[day.ToString().ToLower()][MatinOuAprèsMidi];
        }


        private void instanciate(IPAddress ip)
        {
            for (int i = 0; i < days.Length; i++)
            {
                Days.Add(days[i], new string[2]);
                Days[days[i]][0] = ip.ToString();
                Days[days[i]][1] = ip.ToString();
            }
        }
    }
}
