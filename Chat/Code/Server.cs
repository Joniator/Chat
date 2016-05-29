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
        static Dictionary<string, Thread> connectedUser;
        static bool started;
        /// <summary>
        /// Startet den Listener der neue Verbindungen von Clients annimmt.
        /// </summary>
        public static void startServer()
        {
            started = true;
            connectedUser = new Dictionary<string, Thread>();
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 1337); tcpListener.Start();
            Console.WriteLine("[Server][{0}]: Listener started", DateTime.Now);

            // Wartet auf Anfragen von Clients solange der Server gestartet ist.
            while (started)
            {
                try
                {
                    AcceptClient(tcpListener.AcceptTcpClient());
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Server]" + e.InnerException);
                }
            }
            tcpListener.Stop();
        }

        /// <summary>
        /// Stoppt den Server.
        /// </summary>
        public static void Stop()
        {
            started = false;
        }

        /// <summary>
        /// Überprüft die Anmeldedaten, weißt dem Client eine Connection zu und startet diese in einem neuen Thread.
        /// </summary>
        /// <param name="tcpClient">Der Client, der die Verbindung angefragt hat.</param>
        static void AcceptClient(TcpClient tcpClient)
        {
                Connection c = new Connection(tcpClient);
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
            while (tcpClient.Connected)
            {
                string Message = streamRW.ReadLine();
                if (Message != "")
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
            Message receivedMessage = MessageSerializer.Deserialize(Content);
            Command command = (Command)receivedMessage.content;
            switch (command.type)
            {
                case CommandType.Login:
                    StreamRW streamRW = new StreamRW(tcpClient.GetStream());
                    if (command.parameter[0] == command.parameter[1])
                    {
                        streamRW.WriteLine("Login successfull");
                        username = command.parameter[0];
                        Console.WriteLine("[Server][{0}]{1} logged in",DateTime.Now, username);
                    }
                    else
                    {
                        streamRW.WriteLine("Login failed");
                        Console.WriteLine("[Server][{0}{1}] failed to log in", DateTime.Now, username);
                    }
                        break;
                case CommandType.Message:
                    Console.WriteLine("[Server][{0}]{1}: {2}", receivedMessage.sendTime, receivedMessage.sender, command.parameter[0]);
                    break;
                case CommandType.Disconnect:
                    Console.WriteLine("[Server][{0}]{1}: Disconnected: {2}", receivedMessage.sendTime, receivedMessage.sender, command.parameter[0]);
                    break;
            }

        }
    }
}
