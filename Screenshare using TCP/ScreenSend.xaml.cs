using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using SevenZip.Compression.LZMA;

namespace Screenshare_using_TCP
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class ScreenSend : Window
    {
        private IPAddress address;

        public ScreenSend(IPAddress address)
        {
            this.address = address;
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


                Socket ogsocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                ogsocket.Bind(new IPEndPoint(address, 1234));
                ogsocket.Listen(100);
                Socket socket = ogsocket.Accept();


                {
                    int number = captureBitmap.Width;
                    byte[] bytes = BitConverter.GetBytes(number);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(bytes);
                    socket.Send(bytes);
                }

                {
                    int number = captureBitmap.Height;
                    byte[] bytes = BitConverter.GetBytes(number);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(bytes);
                    socket.Send(bytes);
                }




                System.Threading.Thread.Sleep(1000);

                while (true)
                {
                    captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                    image.Dispatcher.Invoke(() => image.Source = BitmapToImageSource(captureBitmap));

                    byte[] data = new byte[3 * captureBitmap.Height * captureBitmap.Width];
                    int index = 0;
                    int wi = captureBitmap.Width, he = captureBitmap.Height;
                    for (int y = 0; y < he; y++)
                    {
                        for (int x = 0; x < wi; x++)
                        {
                            Color temp = captureBitmap.GetPixel(x, y);
                            data[index] = temp.R; index++;
                            data[index] = temp.G; index++;
                            data[index] = temp.B; index++;
                        }
                    }

                    // Compress it
                    data = SevenZip.Compression.LZMA.SevenZipHelper.Compress(data);

                    {
                        int number = data.Length;
                        byte[] bytes = BitConverter.GetBytes(number);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        socket.Send(bytes);
                    }

                    socket.Send(data);

                    System.Threading.Thread.Sleep(50);
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
