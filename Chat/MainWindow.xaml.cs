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
            client.OnMessageReceived += AddChat;
        }

        private void AddChat(object source, System.Windows.HorizontalAlignment Allignment, string Message, string Sender, DateTime Date)
        {
            stackPanelMessages.Children.Add(new ChatMessage()
            {
                HorizontalAlignment = Allignment,
                Message = Message,
                From = Sender,
                Date = Date
            });
        }
        
        private void buttonToggleConnect_Click(object sender, RoutedEventArgs args)
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
                    MessageBox.Show("Login Failed");
                    buttonToggleConnect.IsChecked = false;
                }
            }
            else
            {
                buttonToggleConnect.Content = "Connect";
                client.Disconnect("Disconnected");
                stackPanelMessages.Children.Clear();
            }
        }
    
        private void buttonStartServer_Click(object sender, RoutedEventArgs args)
        {
                Task t = new Task(Server.startServer);
                t.Start();
        }
        
        private void buttonSend_Click(object sender, RoutedEventArgs args)
        {
            client.SendMessage(textBoxMessage.Text);
            textBoxMessage.Text = "";
        }
        
        private void Window_Closed(object sender, EventArgs args)
        {
            client.Disconnect("Disconnected");
            Server.Stop();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs args)
        {
            settings.Port = Convert.ToInt32(textBoxServerPort.Text);
            settings.IpAddress = textBoxServerIP.Text;
            settings.Save();

            textBoxServerIP.Text = settings.IpAddress;
            textBoxServerPort.Text = settings.Port.ToString();
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs args)
        {
            if(buttonToggleConnect.IsChecked != true)
            {
                try
                {
                    if(client.Register(textBoxRegUsername.Text, textBoxRegPassword.Text))
                    {
                        MessageBox.Show("Registered successfully");
                    }
                }
                catch(Exception e)
                {
                    Log.WriteLine("[MainWindow][{0}] {1}", DateTime.Now, e.InnerException);
                    MessageBox.Show("Registration failed");
                }
            }
            else
            {
                MessageBox.Show("Log out before register");
            }
        }

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs args)
        {
            if(args.Key == Key.Enter)
            {
                client.SendMessage(textBoxMessage.Text);
                textBoxMessage.Text = "";
            }
        }
    }
}
