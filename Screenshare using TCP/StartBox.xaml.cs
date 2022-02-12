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
using System.Windows.Shapes;

namespace Screenshare_using_TCP
{
    /// <summary>
    /// Interaktionslogik für StartBox.xaml
    /// </summary>
    public partial class StartBox : Window
    {
        public StartBox()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name.Equals("Send"))
            {
                ScreenSend window = new ScreenSend();
                window.Show();
                Close();
            }
            else if (((Button)sender).Name.Equals("Receive"))
            {
                ScreenReceive window = new ScreenReceive();
                window.Show();
                Close();
            }
        }
    }
}
