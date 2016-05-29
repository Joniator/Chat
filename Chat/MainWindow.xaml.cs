using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // client.SendMessage(textBoxMessage.Text);

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
    }
}
