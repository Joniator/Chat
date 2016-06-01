using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace Chat
{
    public class SettingsDatabase
    {
        static string saveFile = @"Settings.xmldb";
        IPAddress ipAddress;
        public string IpAddress
        {
            get
            {
                return ipAddress.ToString();
            }
            set
            {
                try
                {
                    ipAddress = IPAddress.Parse(value);
                }
                catch (Exception e)
                {
                    Log.WriteLine("[SetingsDatabase][{0}] {1}", DateTime.Now, e.InnerException);
                }
            }
        }

        int port;
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                if(value > 1024 && value < 65536)
                {
                    port = value;
                }
            }
        }

        /// <summary>
        /// Speichert die aktuellen Settings.
        /// </summary>
        public void Save()
        {
            StreamWriter streamWriter = new StreamWriter(saveFile, false);
            streamWriter.WriteLine(Serializer.Serialize<SettingsDatabase>(this, true));
            streamWriter.Close();
        }

        /// <summary>
        /// Lädt die gespeicherten Settings oder erstellt Default-Settings wenn keine gefunden werden.
        /// </summary>
        /// <returns></returns>
        public static SettingsDatabase Load()
        {
            if(!File.Exists(saveFile))
            {
                new SettingsDatabase()
                {
                    IpAddress = "127.0.0.1",
                    Port = 1337
                }.Save();
            }

            StreamReader streamReader = new StreamReader(saveFile);
            return Serializer.Deserialize<SettingsDatabase>(streamReader.ReadToEnd());
        }
    }

}
