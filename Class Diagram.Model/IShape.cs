using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Diagram.Model
{
    public interface IShape
    {
        double X { get; set; }
        double Y { get; set; }
        double CanvasCenterX { get; set; }
        double CanvasCenterY { get; set; }
        void OnMove();
        void OnMoveEnd();

        //double Height { get; set; }
        //double Width { get; set; }
        //List<string> Data { get; set; }
        //double Height { get; set; }
        //int Number { get; }
        //EShape Type { get; }
        //double Width { get; }


        //string ToString();
    }
}
