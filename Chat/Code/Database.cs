using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    class Database
    {
        Dictionary<string, string> UserDatabase = new Dictionary<string, string>();
        
        public string registration(string Username, string Password)
        {
            try
            {
                UserDatabase.Add(Username, Password);

                return "Die Registrierung war erfolgreich!";
            }
            catch (Exception)
            {

                throw;
            }            
        }
        
        public void login(string Username, string Password)
        {
            if(UserDatabase.ContainsKey(Username))
            {
                //if(UserDatabase.)
            }
        } 
    }
}
