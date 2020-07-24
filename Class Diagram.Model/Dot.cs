using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Diagram.Model
{
    public class Dot : NotifyPropertyBase, IShape
    {
        #region Fields

        [XmlIgnore]
        public Class attachedClass { get; set; }

        [XmlIgnore]
        public List<Line> connectedLines;

        private double x;
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => CanvasCenterX);
            }
        }
        private double y;
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => CanvasCenterY);
            }
        }

        public double Size { get { return 12; } }

        public double CanvasCenterX
        {
            get { return X + Size / 2; }
            set
            {
                X = value - Size / 2;
                NotifyPropertyChanged(() => X);
            }
        }

        public double CanvasCenterY
        {
            get { return Y + Size / 2; }
            set
            {
                Y = value - Size / 2;
                NotifyPropertyChanged(() => Y);
            }
        }
        #endregion

        #region Constructor

        public Dot () { connectedLines = new List<Line>(); }

        #endregion

        #region Methods

        public void OnMove()
        {
            foreach (Line line in connectedLines)
                line.OnLineMove();
        }

        public void OnMoveEnd()
        {
            List<Line> copy = new List<Line>(connectedLines);
            foreach (Line line in copy)
                line.OnLineMoveEnd();
        }

        public override string ToString() => "Dot";

        #endregion
    }
}
