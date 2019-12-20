using System;
using System.Drawing;
//using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading;

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
        ApexMap[] Maps = new ApexMap[2];
        int index = 0;

        static readonly int width = 800;
        static readonly int height = 800;

        static readonly string OutputName = "Merged";
        static readonly string localdir = AppDomain.CurrentDomain.BaseDirectory;
        static readonly string outputFile = localdir + "\\";
        static readonly string jsonloc = "Coordinates.json";
        //static readonly string jsonloc = AppDomain.CurrentDomain.BaseDirectory + "Coordinates.json";
        public MainWindow()
        {
            Maps[0] = KingsC;
            Maps[1] = WorldsE;

            Current = Maps[0];
            InitializeComponent();
        }
        public void UndoWin()
        {
            if (File.Exists(localdir + Current.MapName + jsonloc))
            {
                List<Coords> points;
                points = DrawMap.Read(Current.MapName);
                DrawMap.SaveCoords(DrawMap.UndoCoords(points), Current.MapName);
                GenerateHeatMap();
            }
        }
        public void AddWin(int x, int y)
        {
            List<Coords> points;

            if (!File.Exists(localdir + Current.MapName + jsonloc))
            {
                //textboxett.Text = "Missing wins";
                points = new List<Coords>();
            }
            else
            {
                points = DrawMap.Read(Current.MapName);
            }
            /*for (int i = 0; i < 50; i++)
                DrawMap.AddCoords(points, i+50, i+50);*/
            DrawMap.SaveCoords(DrawMap.AddCoords(points, x, y), Current.MapName);
            GenerateHeatMap();
        }
        public void GenerateHeatMap()
        {
            if (!File.Exists(localdir + Current.MapName + jsonloc))
            {
                return;
            }
            Bitmap bmp = new Bitmap(width, height);

            string Name = "heatmap1";
            System.Drawing.Color temp;

            if (index == 0)
                temp = System.Drawing.Color.Red;
            else
                temp = System.Drawing.Color.Green;

            DrawMap.DrawCoords(bmp, DrawMap.Read(Current.MapName), temp);

            bmp.Save(localdir + "\\" + Name + ".bmp"); // save "heat"

            Uri uri = new Uri(localdir + "/Resources/" + Current.MapDir + ".png", UriKind.Relative); // load current map

            string stringUri;
            stringUri = uri.ToString();

            Bitmap bump = ConvertToBitmap((stringUri));  // Convert loaded image
            Bitmap merged = MergedBitmaps(bmp, bump); // Merge images

            Load_Image(merged, OutputName + Current.MapName); // using streams to avoid read/write issues

            //textboxett.Text = "GenerateHeatMap Done";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (sender == Change_Button)
                {
                    Current = Maps[(++index) % 2];
                    textboxett.Text = Current.MapName;

                    Uri uri = new Uri(localdir + "/Resources/" + Current.MapDir + ".png", UriKind.Relative); // load current mapstring stringUri;
                    string stringUri;
                    stringUri = uri.ToString();
                    Bitmap image = ConvertToBitmap((stringUri));

                    Load_Image(image, Current.MapName);
                }
                if (sender == Add_Button)
                {
                    Oh_NO.Visibility = Visibility.Visible;
                    textboxett.Text = "Waiting for click";
                }
                if (sender == Draw_Button)
                {
                    GenerateHeatMap();
                }
                if(sender == Oh_NO)
                {
                    Oh_NO.Visibility = Visibility.Hidden;

                    System.Windows.Point p = Mouse.GetPosition(ImageView);

                    double X = p.X;
                    double Y = p.Y;
                    int x = (int)Math.Floor(X);
                    int y = (int)Math.Floor(Y);

                    AddWin(x, y);
                }
                if (sender == Undo_Button)
                {
                    UndoWin();
                }
            }
            catch (FileNotFoundException)
            {
                textboxett.Text = "Exception in Button_Click";
            }
        }
        private void Load_Image(Bitmap source, string saveloc)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(outputFile + saveloc + ".bmp", FileMode.Create, FileAccess.ReadWrite))
                {
                    source.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = fs;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImageView.Source = bitmap;
                }
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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
    }
}