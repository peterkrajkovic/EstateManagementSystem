using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class GPS : IComparable
    {
        public char WidthChar { get; set; }
        public char HeightChar { get; set; }

        public double Width {  get; set; }
        public double Height { get; set; }
        public GPS()
        {

        }

        public GPS(GPS gps)
        {
            WidthChar = gps.WidthChar;
            HeightChar = gps.HeightChar;
            Width = gps.Width;
            Height = gps.Height;
        }
        public GPS(double width, double height)
        {
            Width = width;
            Height = height;
            WidthChar = width >= 0 ? 'E' : 'W';
            HeightChar = height >= 0 ? 'N' : 'S';
        }

        public int CompareTo(object? obj)
        {
            if (obj is GPS && ((GPS)obj).Width == this.Width && ((GPS)obj).Height == this.Height)
            {
                return 0;
            }
            return -1;
        }
    }
}
