using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace Screenshare_using_TCP
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class ScreenSend : Window
    {
        public ScreenSend()
        { 
            InitializeComponent();


            var t = new Thread(() => MainLoop());
            t.Start();
        }

        private void MainLoop()
        {
            try

            {

                Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
                Bitmap captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height, PixelFormat.Format32bppArgb);
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);



                //captureBitmap.Save(@"Capture.jpg", ImageFormat.Jpeg);


                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

                image.Dispatcher.Invoke(() => image.Source = BitmapToImageSource(captureBitmap));
                //image.Source = BitmapToImageSource(captureBitmap);


                System.Threading.Thread.Sleep(1000);

                while (true)
                {
                    captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                    image.Dispatcher.Invoke(() => image.Source = BitmapToImageSource(captureBitmap));
                   //System.Threading.Thread.Sleep(0);
                }

            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);

            }
        }


        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
