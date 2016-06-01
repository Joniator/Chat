using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Chat
{
    class Server
    {
        static TcpListener tcpListener;
        public static List<Connection> connectedUser;
        public static bool Running = false;
        public static UserDatabase userDatabase;
        public static MessageDatabase messageDatabase;
        public static List<Message> chatHistory;

        static SettingsDatabase settings = SettingsDatabase.Load();

        /// <summary>
        /// Startet den Listener der neue Verbindungen von Clients annimmt.
        /// </summary>
        public static void startServer()
        {
            if(!Running)
            {
                userDatabase = UserDatabase.Load();
                messageDatabase = MessageDatabase.Load();
                chatHistory = MessageDatabase.Load().messageHistory;
                Running = true;

                connectedUser = new List<Connection>();
                tcpListener = new TcpListener(IPAddress.Any, settings.Port);
                tcpListener.Start();

                Log.WriteLine("[Server][{0}]: Listener started at port {1}", DateTime.Now, settings.Port);
                // Wartet auf Anfragen von Clients solange der Server gestartet ist.
                while(Running)
                {
                    try
                    {
                        AcceptClient(tcpListener.AcceptTcpClient());
                    }
                    catch(Exception e)
                    {
                        Log.WriteLine("[Server][{0}]{1}", DateTime.Now, e.InnerException);
                    }
                }
                tcpListener.Stop();
            }
        }

        /// <summary>
        /// Stoppt den Server.
        /// </summary>
        public static void Stop()
        {
            if (messageDatabase != null) messageDatabase.Save();
            Running = false;
            if(tcpListener != null) tcpListener.Stop();
        }

        /// <summary>
        /// Überprüft die Anmeldedaten, weißt dem Client eine Connection zu und startet diese in einem neuen Thread.
        /// </summary>
        /// <param name="tcpClient">Der Client, der die Verbindung angefragt hat.</param>
        static void AcceptClient(TcpClient tcpClient)
        {
            // Erstellt für den neuen Client eine Verbindung und fügt diese zu den aktiven Nutzern hinzu.
            Connection c = new Connection(tcpClient);
            connectedUser.Add(c);

            // Startet den Thread, der die Verbindung überwacht und ihre Nachrichten überwacht.
            Thread t = new Thread(new ThreadStart(c.Start));
            t.Start();
        }
    }
}
