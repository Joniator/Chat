using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client;
        public MainWindow()
        {
            InitializeComponent();
            client = new Client();

            #region SampleChat
            ChatMessage newMessage = new ChatMessage()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Message = "Yo",
                Date = DateTime.Now
            };
            stackPanelMessages.Children.Add(newMessage);

            newMessage = new ChatMessage()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Message = "Was geht, alles fit?",
                From = "Swaggerboy1337",
                Date = DateTime.Now
            };
            stackPanelMessages.Children.Add(newMessage);

            newMessage = new ChatMessage()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Message = "Kennt wer nen Witz?",
                From = "DerHomoLord",
                Date = DateTime.Now
            };
            stackPanelMessages.Children.Add(newMessage);

            newMessage = new ChatMessage()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Message = 
                @"In der Schule: Die Lehrerin fragt die Kinder, was deren Eltern beruflich machen. Alle erzählen was, dann ist Fritzchen dran.

„Mein Papa spielt Musik im Puff …“

Die Lehrerin, voll geschockt, geht am selben Abend zu seinen Eltern: „Wie können Sie das Kind in dieser Atmosphäre erziehen?!“

Der Vater: „Eigentlich bin ich Informatiker und spezialisiere mich auf TCP/IP Kommunikationsprotokolle in UNIX-Systemen …

Aber wie soll ich das einem 7jährigen Kind erklären?!“",
                From = "DerHomoLord",
                Date = DateTime.Now
            };
            stackPanelMessages.Children.Add(newMessage);
            #endregion  
        }

        // Überprüft ob eine Verbindung besteht, baut eine Verbindung auf und passt den Zustand des ToggelButtons entsprechend an.
        private void buttonToggleConnect_Click(object sender, RoutedEventArgs e)
        {
            if (buttonToggleConnect.IsChecked == true)
            {
                // Sollte der Verbindungsaufbau und die Anmeldung erfolgreich sein, wechselt der Status auf connected, ansonsten bleibt er Disconnected.
                if (client.Connect(textBoxUserName.Text, textBoxPassword.Text))
                {
                    buttonToggleConnect.Content = "Connected";
                }
                else
                {
                    buttonToggleConnect.IsChecked = false;
                }
            }
            else
            {
                buttonToggleConnect.Content = "Connect";
                client.Disconnect();
            }
        }

        // Startet einen Task für den Server.
        private void buttonStartServer_Click(object sender, RoutedEventArgs e)
        {
            Task t = new Task(Server.startServer);
            t.Start();
            Console.WriteLine(t.Id);
        }

        // Sended eine Chatnachricht an den Server.
        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage(textBoxMessage.Text);

            ChatMessage newMessage = new ChatMessage()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Message = textBoxMessage.Text,
                From = textBoxUserName.Text,
                Date = DateTime.Now
            };

            stackPanelMessages.Children.Add(newMessage);
        }

        // Beendet den Server wenn das Fenster geschlossen wird.
        private void Window_Closed(object sender, EventArgs e)
        {
            Server.Stop();
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            Database DatBase = new Database();

            try
            {
                Log.WriteLine(DatBase.registration(textBoxRegUsername.Text, textBoxRegPassword.Text));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
