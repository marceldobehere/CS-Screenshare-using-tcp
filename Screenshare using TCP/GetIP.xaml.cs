using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Screenshare_using_TCP
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class GetIP : Window
    {
        public bool send;
        public GetIP()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IPAddress.TryParse(iptext.Text, out IPAddress address))
            {
                if (send)
                {
                    ScreenSend window = new ScreenSend(address);
                    window.Show();
                }
                else
                {
                    ScreenReceive window = new ScreenReceive(address);
                    window.Show();
                }
                Close();

            }
            else
            {
                MessageBox.Show("Invalid IP", "bruh");
            }
        }

    }
}
