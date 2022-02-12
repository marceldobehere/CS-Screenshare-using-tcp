using System;
using System.Buffers.Binary;
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
    public partial class ScreenReceive : Window
    {
        private IPAddress address;

        public ScreenReceive(IPAddress address)
        {
            Title = "Bruh";
            this.address = address;
            InitializeComponent();


            var t = new Thread(() => MainLoop());
            t.Start();
        }

        private void MainLoop()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(address, 1234));

                //MessageBox.Show($"Getting Data...");

                int width = 0, height;
                {
                    byte[] aaa = new byte[4];
                    int counter = 0;
                    while (counter < 4)
                        counter += socket.Receive(aaa, counter, 4, SocketFlags.None);
                    width = BinaryPrimitives.ReadInt32BigEndian(aaa);
                }
                {
                    byte[] aaa = new byte[4];
                    int counter = 0;
                    while (counter < 4)
                        counter += socket.Receive(aaa, counter, 4, SocketFlags.None);
                    height = BinaryPrimitives.ReadInt32BigEndian(aaa);
                }

                int size = width * height * 3;

                //MessageBox.Show($"Width:{width}\nHeight:{height}\nSize:{size}\n");

                //int totalsize = BinaryPrimitives.ReadInt32BigEndian(aaa);

                System.Threading.Thread.Sleep(500);

                Bitmap tempimg = new Bitmap(width, height);

                while (true)
                {
                    byte[] image_arr = new byte[size];
                    {
                        //MessageBox.Show($"Getting Image Data...");
                        int counter = 0;
                        while (counter < size)
                            counter += socket.Receive(image_arr, counter, size, SocketFlags.None);
                    }

                    {
                        //MessageBox.Show($"Setting Image...");
                        int index = 0;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                byte[] rgb = new byte[3];
                                rgb[0] = image_arr[index]; index++;
                                rgb[1] = image_arr[index]; index++;
                                rgb[2] = image_arr[index]; index++;

                                tempimg.SetPixel(x, y, Color.FromArgb(0, rgb[0], rgb[1], rgb[2]));
                            }
                        }
                    }



                    //captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                    //MessageBox.Show($"Drawing");
                    image.Dispatcher.Invoke(() => image.Source = BitmapToImageSource(tempimg));
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
