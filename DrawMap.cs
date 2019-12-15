using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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


        public static void GenerateHeatMap(Bitmap bmp)
        {

        }
        public static void DrawCoords(Bitmap bmp, List<Coords> data, System.Drawing.Color temp)
        {
            foreach(var points in data)
            {
                bmp.SetPixel(points.X, points.Y, temp);
            }
        }

        /*      Functions for reading and writing to a json file        */
        public static List<Coords> Read()
        {
            _data.Clear();
            
            using (StreamReader file = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "Coordinates.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                _data = (List<Coords>)serializer.Deserialize(file, typeof(List<Coords>));
            }

            return _data;
        }
        public static void SaveCoords()
        {
            //generates coords as of now
            for (int i = -100; i < 101; i+=3) // draw a square
            {
                for (int j = -100; j < 101; j+=3)
                {
                    Coords temp = new Coords(300+i, 300+j);
                    _data.Add(temp);
                }
            }
            string json = JsonConvert.SerializeObject(_data.ToArray());

            //write string to file
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory+ "Coordinates.json", json);
        }

        /*      Function for creating a test drawing        */
        public static void DrawSquare(Bitmap bmp, System.Drawing.Color temp)
        {
            for (int i = -100; i < 101; i++) // draw a square
            {
                for (int j = -100; j < 101; j++)
                {
                    bmp.SetPixel(500 + i, 500 + j, temp);
                }
            }
        }
    }
}
