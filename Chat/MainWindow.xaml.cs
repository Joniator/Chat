using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        SettingsDatabase settings;
        public MainWindow()
        {
            InitializeComponent();
            client = new Client();
            client.userUI = this;
            settings = SettingsDatabase.Load();
            textBoxServerPort.Text = settings.Port.ToString();
            textBoxServerIP.Text = settings.IpAddress;        
        }

        public void AddChat(ChatMessage chatMessage)
        {
            stackPanelMessages.Children.Add(chatMessage);
            Log.WriteLine("ADDED");
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
                    client.LoadChat();
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
        }

        // Beendet den Server wenn das Fenster geschlossen wird.
        private void Window_Closed(object sender, EventArgs e)
        {
            Server.Stop();            
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            settings.Port = Convert.ToInt32(textBoxServerPort.Text);
            settings.IpAddress = textBoxServerIP.Text;
            settings.Save();

            textBoxServerIP.Text = settings.IpAddress;
            textBoxServerPort.Text = settings.Port.ToString();
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Register(textBoxRegUsername.Text, textBoxRegPassword.Text);                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
