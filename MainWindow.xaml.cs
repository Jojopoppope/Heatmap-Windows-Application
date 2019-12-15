using System;
using System.Drawing;
//using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace HeatMap_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ApexMap
    {
        public string MapDir { get; set; }
        public string MapName { get; set; }
        public ImageSource Source;
        public ApexMap(string Dir, string Name)
        {
            MapDir = Dir;
            MapName = Name;
            Source = new BitmapImage(new Uri("/Resources/" + Dir + ".png", UriKind.Relative));
        }
    }
    public partial class MainWindow : Window
    {
        public ApexMap KingsC = new ApexMap("KingsC", "Kings Canyon");
        public ApexMap WorldsE = new ApexMap("WorldE", "World's Edge");
        public ApexMap Current;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\.ApexMap\\";

        PixelFormat pf = PixelFormats.Bgr32;
        static readonly int width = 800;
        static readonly int height = 800;

        static readonly string OutputName = "Merged";
        static readonly string outputFileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + OutputName + ".bmp";
        BitmapImage bitmapImage = new BitmapImage();
        public MainWindow()
        {
            Current = KingsC;
            InitializeComponent();
        }
        public void GenerateHeatMap()
        {
            Bitmap bmp = new Bitmap(width, height);

            string Name = "heatmap1";
            System.Drawing.Color temp;

            if (Current.MapName == "Kings Canyon")
                temp = System.Drawing.Color.Blue;
            else
                temp = System.Drawing.Color.Green;

            for (int i = -100; i < 101; i++) // draw a square
            {
                for (int j = -100; j < 101; j++)
                {
                    bmp.SetPixel(500 + i, 500 + j, temp);
                }
            }

            bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "\\" + Name + ".bmp"); // save "heat"

            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/" + Current.MapDir + ".png", UriKind.Relative); // load current map

            string stringUri;
            stringUri = uri.ToString();

            Bitmap bump = ConvertToBitmap((stringUri));  // Convert loaded image
            Bitmap merged = MergedBitmaps(bmp, bump); // Merge images

            // using streams to avoid read/write issues
            using (MemoryStream memory = new MemoryStream())
            {                    
                    using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        merged.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                  
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = fs;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        ImageOverlay.Source = bitmap;
                }
            }

            textboxett.Text = "GenerateHeatMap Done";
        }
        public Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }
        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            Bitmap result = new Bitmap(Math.Max(bmp1.Width, bmp2.Width),
                                       Math.Max(bmp1.Height, bmp2.Height));
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp2, System.Drawing.Point.Empty);
                g.DrawImage(bmp1, System.Drawing.Point.Empty);
            }
            return result;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                textboxett.Text = "Starting...";

                switch (Current.MapDir)
                {
                    case "KingsC":
                        Current = WorldsE;
                        break;
                    case "WorldE":
                        Current = KingsC;
                        break;
                    default:
                        break;

                }
                GenerateHeatMap();

                //ImageView.Source = Current.Source;
                textboxett.Text = Current.MapName;
            }
            catch (FileNotFoundException)
            {
                textboxett.Text = "Exception in Button_Click";
            }
        }
    }
}
//HeatMap_App.Properties.Resources.WorldE
//image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image.png", UriKind.Absolute));
//ImageOverlay.Source = new BitmapImage(new Uri(path + "myBitmap.png"));
//ImageOverlay.Source = new BitmapImage(new Uri("C:\\Users\\Joppm\\source\\repos\\HeatMap App\\bin\\Debug\\myBitmap.png"));

//string target = "/Resources/" + Current.MapDir + ".png";
//ImageView.Source = new BitmapImage(new Uri(target, UriKind.Relative));
//image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image.png", UriKind.Absolute));
