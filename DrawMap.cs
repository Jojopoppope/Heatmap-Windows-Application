using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HeatMap_App
{
    [DataContract]
    struct Coords
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    class DrawMap
    {
        static List<Coords> _data = new List<Coords>(); // data containing all the marks in the map
        static readonly string localdir = AppDomain.CurrentDomain.BaseDirectory;
        public static void DrawCoords(Bitmap bmp, List<Coords> data, System.Drawing.Color temp)
        {
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.InterpolationMode = InterpolationMode.High;
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.CompositingQuality = CompositingQuality.HighQuality;

                using (Pen thick_pen = new Pen(Color.Black, 1)) //useless as of now since it doesn't use a pen
                {
                    thick_pen.DashStyle = DashStyle.Solid;
                    SolidBrush redBrush = new SolidBrush(Color.FromArgb(150, 200, 0, 50));
                    thick_pen.Width = 1;
                    foreach (var points in data)
                    {
                        //bmp.SetPixel(points.X, points.Y, Color.FromArgb(150, 200, 0, 50));
                        gr.DrawEllipse(thick_pen, new Rectangle(points.X-5, points.Y-5, 10, 10));
                        gr.FillEllipse(redBrush, new Rectangle(points.X-5, points.Y-5, 10, 10));
                    }

                    GraphicsPath p = new GraphicsPath();
                    p.AddString(
                        "Wins: " + (data.Count).ToString(), // text to draw
                        FontFamily.GenericSansSerif,        // or any other font family
                        (int)FontStyle.Regular,             // font style (bold, italic, etc.)
                        gr.DpiY * 16 / 72,                  // em size
                        new Point(650, 20),                 // location where to draw text
                        new StringFormat());
                    gr.DrawPath(Pens.Black, p);
                    gr.FillPath(new SolidBrush(Color.White), p);
                    //gr.DrawString("Wins: " + (data.Count).ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 650f,20f); //old
                }
            }
        }
        /*      Functions for reading and writing to a json file        */
        public static List<Coords> Read(string currentmap)
        {
            _data.Clear();
            
            using (StreamReader file = File.OpenText(localdir + currentmap + "Coordinates.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                _data = (List<Coords>)serializer.Deserialize(file, typeof(List<Coords>));
            }

            return _data;
        }
        public static List<Coords> AddCoords(List<Coords> input, int x, int y)
        {
            Coords temp = new Coords(x, y);
            input.Add(temp);
            return input;
        }
        public static List<Coords> UndoCoords(List<Coords> input)
        {
            input.RemoveAt(input.Count - 1);
            return input;
        }
        public static void SaveCoords(List<Coords> input, string currentmap)
        {
            string json = JsonConvert.SerializeObject(input.ToArray());
            System.IO.File.WriteAllText(localdir + currentmap + "Coordinates.json", json); // write string to file
        }
    }
}

//generates coords as of now

/*for (int i = -100; i < 101; i+=3) // draw a square
{
    for (int j = -100; j < 101; j+=3)
    {
        Coords temp = new Coords(300+i, 300+j);
        _data.Add(temp);
    }
}*/
/*      Function for creating a test drawing        *//*
public static void DrawSquare(Bitmap bmp, System.Drawing.Color temp)
{
    for (int i = -100; i < 101; i++) // draw a square
    {
        for (int j = -100; j < 101; j++)
        {
            bmp.SetPixel(500 + i, 500 + j, temp);
        }
    }
}*/