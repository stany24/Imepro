using System;
using System.Net;

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
            try { IPAddress.Parse(ip); }
            catch { return; }
            int BeforeAfterNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { BeforeAfterNoon = 1; }
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday: Properties.Settings.Default.monday[BeforeAfterNoon] = ip.ToString();break;
                case DayOfWeek.Tuesday: Properties.Settings.Default.tuesday[BeforeAfterNoon] = ip.ToString(); break;
                case DayOfWeek.Wednesday: Properties.Settings.Default.wednesday[BeforeAfterNoon] = ip.ToString(); break;
                case DayOfWeek.Thursday: Properties.Settings.Default.thursday[BeforeAfterNoon] = ip.ToString();break;
                case DayOfWeek.Friday: Properties.Settings.Default.friday[BeforeAfterNoon] = ip.ToString();break;
                case DayOfWeek.Saturday: Properties.Settings.Default.saturday[BeforeAfterNoon] = ip.ToString();break;
                case DayOfWeek.Sunday: Properties.Settings.Default.sunday[BeforeAfterNoon] = ip.ToString();break;
            }
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        /// <summary>
        /// Fonction qui retourne la bonne ip en fonction du jour et de l'heure de l'appel
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetIp()
        {
            DayOfWeek day = DateTime.Now.DayOfWeek;
            int BeforeAfterNoon = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { BeforeAfterNoon = 1; }
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday: return IPAddress.Parse(Properties.Settings.Default.monday[BeforeAfterNoon]);
                case DayOfWeek.Tuesday: return IPAddress.Parse(Properties.Settings.Default.tuesday[BeforeAfterNoon]);
                case DayOfWeek.Wednesday: return IPAddress.Parse(Properties.Settings.Default.wednesday[BeforeAfterNoon]);
                case DayOfWeek.Thursday: return IPAddress.Parse(Properties.Settings.Default.thursday[BeforeAfterNoon]);
                case DayOfWeek.Friday: return IPAddress.Parse(Properties.Settings.Default.friday[BeforeAfterNoon]);
                case DayOfWeek.Saturday: return IPAddress.Parse(Properties.Settings.Default.saturday[BeforeAfterNoon]);
                case DayOfWeek.Sunday: return IPAddress.Parse(Properties.Settings.Default.sunday[BeforeAfterNoon]);
            }
            return IPAddress.Parse("1.1.1.1");
        }
    }
}
