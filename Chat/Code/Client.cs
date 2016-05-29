using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Threading;

namespace Chat
{
    class Client
    {
        TcpClient tcpClient;
        StreamRW streamRW;
        string username;
        /// <summary>
        /// Gibt zurück ob der Client mit dem Server verbunden ist.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return tcpClient.Connected;
            }
        }

        /// <summary>
        /// Verbindet mit dem Server und meldet sich mit den Nutzerdaten an.
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns>Gibt an ob die Verbindung und der Login erfolgreich waren.</returns>
        public bool Connect(string Username, string Password)
        {
            tcpClient = new TcpClient("87.172.204.102", 1337);

            streamRW = new StreamRW(tcpClient.GetStream());
            Message message = new Message()
            {
                content = new Command(CommandType.Login, Username, Password),
                sender = Username,
                sendTime = DateTime.Now
            };
            Send(message);
            username = Username;
            return (streamRW.ReadLine() == "Login successfull") ? true : false;
        }

        public bool SendMessage(string message)
        {
            Message mes = new Message()
            {
                content = new Command(CommandType.Message, message),
                sender = username,
                sendTime = DateTime.Now
            };
            return Send(mes);
        }

        public bool Send(Message message)
        {
            streamRW.WriteLine(message.ToString());

            return true;
        }

        /// <summary>
        /// Beendet die Verbindung mit dem Server.
        /// </summary>
        public void Disconnect()
        {
            Message message = new Message()
            {
                content = new Command(CommandType.Disconnect, "Quit"),
                sender = username,
                sendTime = DateTime.Now
            };
            Send(message);
            tcpClient.Close();
        }
    }
}