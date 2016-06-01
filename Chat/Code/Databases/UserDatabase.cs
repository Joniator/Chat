using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace Chat
{
    public class UserDatabase
    {
        static string saveFile = @"User.xmldb";
        Dictionary<string, string> userDatabase = new Dictionary<string, string>();
        public List<string> keys;
        public List<string> values;

        public bool Register(string Username, string Password)
        {
            try
            {
                if(!userDatabase.Keys.ToArray<string>().Contains<string>(Username))
                {
                    userDatabase.Add(Username, Password);
                    Save();
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                Log.WriteLine("[UserDatabase][{0}] {1}", DateTime.Now, e.InnerException);
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

        /// Problem: Dictionaries 
        /// XML kann keine Dictioniries serialisieren.

        /// Workaround: Das Dictionary wird in eine Key- und eine Value-List zerlegt, welche dann serialisiert werden.
        /// Dadurch muss das Dictionary selbst private sein, was soweit auch kein Problem ist, immerhin kann man mit Register und Login alle
        /// Zugriffe darauf abarbeiten.
        /// Da zu serialisierende Felder aber public sein müssen, sind die Lists welche Die Keys und Values enthalten public.

        public void Save()
        {
            // Füllt die Arrays mit den Keys und Values des Dictionariess.
            keys = userDatabase.Keys.ToList<string>();
            values = userDatabase.Values.ToList<string>();

            StreamWriter streamWriter = new StreamWriter(saveFile, false);
            streamWriter.WriteLine(Serializer.Serialize<UserDatabase>(this, true));
            streamWriter.Close();
        }

        public static UserDatabase Load()
        {
            if(!File.Exists(saveFile))
            {
                new UserDatabase()
                {
                }.Save();
            }
            StreamReader streamReader = new StreamReader(saveFile);
            UserDatabase toReturn = Serializer.Deserialize<UserDatabase>(streamReader.ReadToEnd());

            // Erstellt aus den Arrays Keys und Values ein neues Dictionary.
            for(int i = 0; i < toReturn.keys.Count; i++)
            {
                toReturn.userDatabase.Add(toReturn.keys[i], toReturn.values[i]);
            }
            return toReturn;
        }
    }

}