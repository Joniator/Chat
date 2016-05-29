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
    /// Interaktionslogik für ChatMessage.xaml
    /// </summary>
    public partial class ChatMessage : UserControl
    {
        public ChatMessage()
        {
            InitializeComponent();
        }
        public string Message
        {
            get
            {
                return textBlockMessage.Text;
            }
            set
            {
                textBlockMessage.Text = value;
            }
        }
        public string From
        {
            get
            {
                return labelFrom.Content.ToString();
            }
            set
            {
                labelFrom.Content = value;
            }
        }
        public DateTime Date
        {
            get
            {
                return (DateTime)labelDate.Content;
            }
            set
            {
                labelDate.Content = value;
            }
        }
        
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Date = DateTime.Now;
        }
    }
}
