using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Chat
{
    public class Settings
    {
        // dumm

        static string saveFile = @"Settings.xml";

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
                    Log.WriteLine("[Settings][{0}] {1} set as IPAddress.", DateTime.Now, value);
                }
                catch
                {
                    Log.WriteLine("[Settings][{0}] {1} is not a valid IP Address.", DateTime.Now, value);
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
                if (value > 1024 && value < 65536)
                {
                        port = value;
                        Log.WriteLine("[Settings][{0}] {1} set as port for server.", DateTime.Now, value);
                }
                else
                {
                    Log.WriteLine("[Settings][{0}] {1} is not a valid port.", DateTime.Now, value);
                }
            }
        }        

        public void SaveSettings()
        {
            StreamWriter streamWriter = new StreamWriter(saveFile, false);
            streamWriter.WriteLine(Serializer.Serialize<Settings>(this, true));
            streamWriter.Close();
        }

        public static Settings LoadSettings()
        {
            if (!File.Exists(saveFile))
            {
                new Settings()
                {
                    IpAddress = "127.0.0.1",
                    Port = 1337
                }.SaveSettings();
            }

            StreamReader streamReader = new StreamReader(saveFile);
            return Serializer.Deserialize<Settings>(streamReader.ReadToEnd());
        }
    }
}
