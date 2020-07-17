using System;

namespace HealthMonitor.Config
{
    public class Settings
    {
        public readonly static string DBPATH = $"{AppDomain.CurrentDomain.BaseDirectory}healthlogs.sqlite";
    }
}
