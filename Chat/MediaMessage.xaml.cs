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
    /// Interaktionslogik für MediaMessage.xaml
    /// </summary>
    public partial class MediaMessage : UserControl
    {
        public MediaMessage()
        {
            InitializeComponent();
        }

        public string Source
        {
            get
            {
                return mediaElement.Source.ToString();
            }
            set
            {
                mediaElement.Source = new UriBuilder(value).Uri;
            }
        }
    }
}
