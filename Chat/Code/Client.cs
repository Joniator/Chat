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
        public MainWindow userUI;

        DispatcherTimer dTimer;
        /// <summary>
        /// Wird jede Sekunde ausgeführt und überprüft ob neue Nachrichten angekommen sind.
        /// </summary>
        void DTimer_Tick(object sender, EventArgs e)
        {
            Message message = Serializer.Deserialize<Message>(streamRW.ReadLine());

            System.Windows.HorizontalAlignment horizontallignment;
            if (message.sender == username) horizontallignment = System.Windows.HorizontalAlignment.Right;
            else horizontallignment = System.Windows.HorizontalAlignment.Left;

            userUI.AddChat(new ChatMessage()
            {
                HorizontalAlignment = horizontallignment,
                Message = message.content.parameter[0],
                From = message.sender,
                Date = message.sendTime
            });
        }

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
            tcpClient = new TcpClient(SettingsDatabase.Load().IpAddress, SettingsDatabase.Load().Port);

            streamRW = new StreamRW(tcpClient.GetStream());
            Message message = new Message()
            {
                content = new Content(ContentType.Login, Username, Password),
                sender = Username,
                sendTime = DateTime.Now
            };
            Send(message);
            username = Username;

            if (streamRW.ReadLine() == "Login successfull")
            {
                return true;
            }
            else
            {
                streamRW.Close();
                tcpClient.Close();
                return false;
            }
        }

        /// <summary>
        /// Versucht, sich beim Server zu registrieren.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Register(string username, string password)
        {
            tcpClient = new TcpClient(SettingsDatabase.Load().IpAddress, SettingsDatabase.Load().Port);
            streamRW = new StreamRW(tcpClient.GetStream());

            Message message = new Message()
            {
                content = new Content(ContentType.Register, username, password),
                sender = null,
                sendTime = DateTime.Now
            };

            Send(message);

            if (streamRW.ReadLine() == "Register succesfull")
            {
                Log.WriteLine("[Client][{0}] Registered.", DateTime.Now);
            }
            Disconnect();
        }

        /// <summary>
        /// Lädt die ChatHistory vom Server und zeigt sie auf der Form an.
        /// </summary>
        public void LoadChat()
        {
            Message sendMessage = new Message()
            {
                content = new Content(ContentType.RequestChat)
            };
            streamRW.WriteLine(sendMessage.ToString());

            List<Message> chatHistory = Serializer.Deserialize<List<Message>>(streamRW.ReadLine());
            if (chatHistory != null)
                foreach (Message message in chatHistory)
                {
                    System.Windows.HorizontalAlignment horizontallignment;
                    if (message.sender == username) horizontallignment = System.Windows.HorizontalAlignment.Right;
                    else horizontallignment = System.Windows.HorizontalAlignment.Left;

                    userUI.AddChat(new ChatMessage()
                    {
                        HorizontalAlignment = horizontallignment,
                        Message = message.content.parameter[0],
                        From = message.sender,
                        Date = message.sendTime
                    });
                }

            dTimer = new DispatcherTimer();
            dTimer.Interval = new TimeSpan(0, 0, 10);
            dTimer.Tick += DTimer_Tick;
            dTimer.Start();
        }

        /// <summary>
        /// Sendet eine Nachricht, fügt Typ, Sender und Date automatisch hinzu.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendMessage(string message)
        {
            Message mes = new Message()
            {
                content = new Content(ContentType.Message, message),
                sender = username,
                sendTime = DateTime.Now
            };
            return Send(mes);
        }

        /// <summary>
        /// Sendet eine Nachricht.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
                content = new Content(ContentType.Disconnect, "Quit"),
                sender = username,
                sendTime = DateTime.Now
            };
            Send(message);
            tcpClient.Close();
        }

    }
}