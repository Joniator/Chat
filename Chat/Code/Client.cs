using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Chat
{
    class Client
    {
        TcpClient tcpClient;
        StreamRW streamRW;
        string username;
        public MainWindow userUI;
        SettingsDatabase settings = SettingsDatabase.Load();

        DispatcherTimer dTimer;

        /// Problem: Forms-Zugriffsberechtigung
        /// Der Timer ist ein Workaround für die Zugriffsberechtigung von Forms/WPF-Elementen.
        /// Der Timer fragt jede Sekunde nach, ob eine neue Message angekommen ist, um das OnMessageReceived-Event zu starten.
        /// streamRW.ReadLine() würde den gesamten Thread so lange blockieren, bis eine Nachricht angekommen ist oder der Befehl Time-outet.
        /// Solange ReadLine den Thread blockiert, kann das Userinterface nicht genutzt werden.
        /// Eine Sekunde nach dem Timeout wird das Userinterface wieder blockiert bis entweder eine Nachricht eintrifft, oder der Reader timeoutet.
        /// 
        /// Das ganze in einen neuen Thread auszulagern scheidet aus, da es zu Zugriffsproblemen auf die Elemente im MainWindow kommt wenn man keinen 
        /// Thread benutzt:
        /// 
        /// Thread t = new Thread(refreshMessages);
        /// t.Start();
        /// (Der aufrufende Thread kann nicht auf dieses Objekt zugreifen, da sich das Objekt im Besitz eines anderen Threads befindet.).

        /// Workaround: Timer und ReadLineFastTO
        /// Als Workaround haben wir den Read-Timeout vom StreamReader auf 0.2 Sekunden gesetzt, damit der Zeitraum in dem nach neuen Nachrichten gefragt wird möglichst unauffällig ist.
        /// Damit die Funktionsweise des Readers, wenn er nicht unter Zeitdruck steht nicht gefährdet wird, haben wir eine spezielle ReadLine-Methode dafür geschrieben, die besonders schnell Timeoutet.

        /// <summary>
        /// Wird jede Sekunde ausgeführt und überprüft ob neue Nachrichten angekommen sind.
        /// </summary>
        private void DTimer_Tick(object sender, EventArgs args)
        {
            refreshMessages();
        }

        /// <summary>
        /// Liest die aktuelle Zeile vom Stream, extrahiert die Message und löst ein MessageReceived-Event mit 
        /// der Message aus.
        /// </summary>
        private void refreshMessages()
        {
            Message message = Serializer.Deserialize<Message>(streamRW.ReadLineFastTO());
            if(message != default(Message))
            {
                if(message.content.type == ContentType.Kicked)
                {
                    MessageBox.Show("Server closed connection");
                }
                else
                {
                    System.Windows.HorizontalAlignment horizontallignment;
                    if(message.sender == username) horizontallignment = System.Windows.HorizontalAlignment.Right;
                    else horizontallignment = System.Windows.HorizontalAlignment.Left;

                    RaiseMessageReceivedEvent(horizontallignment, message.content.parameter[0], message.sender, message.sendTime);
                }
            }
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
            try
            {
                tcpClient = new TcpClient(settings.IpAddress, settings.Port);

                streamRW = new StreamRW(tcpClient.GetStream());
                Message message = new Message()
                {
                    content = new Content(ContentType.Login, Username, Password),
                    sender = Username,
                    sendTime = DateTime.Now
                };
                Send(message);
                username = Username;

                if(streamRW.ReadLine() == "Login successfull")
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
            catch(Exception e)
            {
                Log.WriteLine("[Server][{0}] {1}", DateTime.Now, e.InnerException);
                MessageBox.Show("Server nicht erreichtbar");
                return false;
            }
        }

        /// <summary>
        /// Beendet die Verbindung mit dem Server.
        /// </summary>
        public void Disconnect(string Reason)
        {
            Message message = new Message()
            {
                content = new Content(ContentType.Disconnect, Reason),
                sender = username,
                sendTime = DateTime.Now
            };
            Send(message);
            if(dTimer != null) dTimer.Stop();
            if(tcpClient != null) tcpClient.Close();
        }

        /// <summary>
        /// Versucht, sich beim Server zu registrieren.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public bool Register(string username, string password)
        {
            if(tcpClient == null || !IsConnected)
            {
                settings = SettingsDatabase.Load();
                tcpClient = new TcpClient(settings.IpAddress, settings.Port);
                streamRW = new StreamRW(tcpClient.GetStream());

                Message message = new Message()
                {
                    content = new Content(ContentType.Register, username, password),
                    sender = null,
                    sendTime = DateTime.Now
                };

                Send(message);
                string buffer = streamRW.ReadLine();
                if(buffer == "Register succesfull")
                {
                    Log.WriteLine("[Client][{0}] {1} registered.", DateTime.Now, username);
                }
                else
                {
                    MessageBox.Show(buffer);
                    return false;
                }
                Disconnect("Registration finished");
                return true;
            }
            return false;
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
            if(chatHistory != null)
                foreach(Message message in chatHistory)
                {
                    System.Windows.HorizontalAlignment horizontallignment;
                    if(message.sender == username) horizontallignment = System.Windows.HorizontalAlignment.Right;
                    else horizontallignment = System.Windows.HorizontalAlignment.Left;

                    RaiseMessageReceivedEvent(horizontallignment, message.content.parameter[0], message.sender, message.sendTime);
                }

            dTimer = new DispatcherTimer();
            dTimer.Interval = new TimeSpan(0, 0, 1);
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
            if(streamRW != null)
                streamRW.WriteLine(message.ToString());

            return true;
        }

        /// <summary>
        /// Löst das MessageReceived-Event aus.
        /// </summary>
        private void RaiseMessageReceivedEvent(System.Windows.HorizontalAlignment Allignment, string Message, string Sender, DateTime Date)
        {
            if(OnMessageReceived != null)
            {
                OnMessageReceived(this, Allignment, Message, Sender, Date);
            }
        }
        public delegate void MessageReceivedEventHandler(object source, System.Windows.HorizontalAlignment Allignment, string Message, string Sender, DateTime Date);
        public event MessageReceivedEventHandler OnMessageReceived;
    }
}