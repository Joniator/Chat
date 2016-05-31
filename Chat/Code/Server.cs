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
        public static List<Connection> connectedUser;
        static bool running;
        public static UserDatabase DataBase = new UserDatabase();
        public static List<Message> chatHistory;

        /// <summary>
        /// Startet den Listener der neue Verbindungen von Clients annimmt.
        /// </summary>
        public static void startServer()
        {
            running = true;
            connectedUser = new List<Connection>();
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 1337); tcpListener.Start();
            Log.WriteLine("[Server][{0}]: Listener started at port {1}", DateTime.Now, 1337);

            chatHistory = new List<Message>();

            // Wartet auf Anfragen von Clients solange der Server gestartet ist.
            while (running)
            {
                try
                {
                    AcceptClient(tcpListener.AcceptTcpClient());
                }
                catch (Exception e)
                {
                    Log.WriteLine("[Server][{0}]{1}", DateTime.Now, e.InnerException);
                }
            }
            tcpListener.Stop();
        }

        /// <summary>
        /// Stoppt den Server.
        /// </summary>
        public static void Stop()
        {
            running = false;
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
    class Connection
    {
        TcpClient tcpClient;
        StreamRW streamRW;
        string username;

        public Connection(TcpClient TcpClient)
        {
            tcpClient = TcpClient;
            streamRW = new StreamRW(tcpClient.GetStream());
        }

        /// <summary>
        /// Beginnt mit der Überwachung des Streams und läuft solange der TcpClient connected ist.
        /// </summary>
        public void Start()
        {
            // Solange der Client mit dem Server verbunden ist, wird jede ankommende Nachricht verarbeitet.
            while (tcpClient.Connected)
            {
                string Message = streamRW.ReadLine();
                if (Message != null && Message != "")
                    ProcessMessage(Message);
            }
        }

        /// <summary>
        /// Verarbeitet Nachrichten, die an den Server von der Verbindung gesendet werden.
        /// </summary>
        /// <param name="Message">Aufbau: Befehle:Parameter</param>
        /// <returns></returns>
        void ProcessMessage(string Content)
        {
            // Deserialisiert die Message.
            Message receivedMessage = Serializer.Deserialize<Message>(Content);
            Content content = receivedMessage.content;

            // Verarbeitet die Nachricht.
            switch (content.type)
            {
                // Registriert einen Benutzer in der UserDatabase und meldet das Ergebnis.
                case ContentType.Register:
                    if (Server.DataBase.Register(content.parameter[0], content.parameter[1]))
                    {
                        this.streamRW.WriteLine("Register succesfull");
                        Log.WriteLine("[Server][{0}] {1} registered.", DateTime.Now, content.parameter[0]);
                    }
                    else
                    {
                        Log.WriteLine("[Server][{0}] {1} failed to register.", DateTime.Now, content.parameter[0]);
                    }
                    break;

                // Überprüft die Anmeldedaten und gibt das Ergebnis zurück.
                case ContentType.Login:
                    StreamRW streamRW = new StreamRW(tcpClient.GetStream());
                    if (Server.DataBase.Login(content.parameter[0], content.parameter[1]))
                    {
                        streamRW.WriteLine("Login successfull");
                        username = content.parameter[0];
                        Log.WriteLine("[Server][{0}] {1} logged in", DateTime.Now, username);
                    }
                    else
                    {
                        streamRW.WriteLine("Login failed");
                        Log.WriteLine("[Server][{0}] {1} failed to log in", DateTime.Now, username);
                    }
                    break;

                case ContentType.Message:
                    Server.chatHistory.Add(receivedMessage);
                    foreach (Connection connection in Server.connectedUser)
                    {
                        connection.streamRW.WriteLine(new Message()
                        {
                            content = receivedMessage.content,
                            sender = receivedMessage.sender,
                            sendTime = receivedMessage.sendTime

                        }.ToString());
                    }
                    Log.WriteLine("[Server][{0}] {1}: {2}", receivedMessage.sendTime, receivedMessage.sender, content.parameter[0]);
                    break;

                case ContentType.Disconnect:
                    Server.connectedUser.Remove(this);
                    Log.WriteLine("[Server][{0}] {1}: Disconnected: {2}", receivedMessage.sendTime, receivedMessage.sender, content.parameter[0]);
                    break;

                case ContentType.RequestChat:
                    this.streamRW.WriteLine(Serializer.Serialize<List<Message>>(Server.chatHistory, false).Replace(Environment.NewLine, ""));
                    break;
            }
        }
    }
}
