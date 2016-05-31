using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Chat
{
    public class SettingsDatabase
    {
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
                }
                catch
                {

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
            if (!File.Exists(saveFile))
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

    class MessageDatabase
    {
        static string saveFile = @"User.xml";
        public List<Message> messageHistory;
        public void Save()
        {
            StreamWriter streamWriter = new StreamWriter(saveFile, false);
            streamWriter.WriteLine(Serializer.Serialize<MessageDatabase>(this, true));
            streamWriter.Close();
        }

        public static MessageDatabase Load()
        {
            return null;
        }
    }

    class UserDatabase
    {
        static string saveFile = @"User.xml";
        Dictionary<string, string> userDatabase = new Dictionary<string, string>();
        
        public bool Register(string Username, string Password)
        {
            try
            {
                userDatabase.Add(Username, Password);

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
        
        public bool Login(string Username, string Password)
        {
            if(userDatabase.ContainsKey(Username))
            {
                if(userDatabase[Username] == Password)
                {
                    Log.WriteLine("login erfolgreich!");
                    return true;
                }
                else
                {
                    Log.WriteLine("Password falsch!");
                    return false;
                }                
            }

            Log.WriteLine("Username existiert nicht!");
            return false;
        } 

        public void Save()
        {
            StreamWriter streamWriter = new StreamWriter(saveFile, false);
            streamWriter.WriteLine(Serializer.Serialize<UserDatabase>(this, true));
            streamWriter.Close();
        }

        public static UserDatabase Load()
        {
            return null;
        }
    }
}
