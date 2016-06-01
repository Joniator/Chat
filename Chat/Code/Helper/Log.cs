using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Chat
{
    /// <summary>
    /// Log-Klasse um eine geordnete Ausgabe von Protokollierung bereitzustellen.
    /// </summary
    public static class Log
    {
        public static void WriteLine(string Message, params object[] param)
        {
            try
            {
                string message = string.Format(Message, param);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Log.WriteLine("[Log][{0}] {1}]", DateTime.Now, e.InnerException);
            }
        }
    }

}