using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Diagram.Model
{
    public class Line : NotifyPropertyBase
    {

        #region Fields
        public ELine Style { get; set; }
        private readonly double arrowAngle = Math.PI / 8;
        private readonly double radius = 16;
        private readonly double rootOfHalf = Math.Sqrt(0.5);

        private Dot from;

        [XmlIgnore]
        public Dot From
        {
            get { return from; }
            set
            {
                from = value;
                from.connectedLines.Add(this);
                NotifyPropertyChanged();
            }
        }

        private Dot to;

        [XmlIgnore]
        public Dot To
        {
            get { return to; }
            set
            {
                to = value;
                to.connectedLines.Add(this);
                NotifyPropertyChanged();
            }
        }

        private double topX;
        public double TopX
        {
            get { return topX; }
            set
            {
                topX = value;
                NotifyPropertyChanged();
            }
        }

        private double topY;
        public double TopY
        {
            get { return topY; }
            set
            {
                topY = value;
                NotifyPropertyChanged();
            }
        }

        private double bottomX;
        public double BottomX
        {
            get { return bottomX; }
            set
            {
                bottomX = value;
                NotifyPropertyChanged();
            }
        }

        private double bottomY;
        public double BottomY
        {
            get { return bottomY; }
            set
            {
                bottomY = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Methods
        internal void OnLineMove()
        {
            if (Style == ELine.Arrow)
            {
                double angle = Math.Atan2((To.CanvasCenterY - From.CanvasCenterY), (To.CanvasCenterX - From.CanvasCenterX)) + Math.PI;
                TopX = To.CanvasCenterX + radius * Math.Cos(angle + arrowAngle);
                TopY = To.CanvasCenterY + radius * Math.Sin(angle + arrowAngle);
                BottomX = To.CanvasCenterX + radius * Math.Cos(angle - arrowAngle);
                BottomY = To.CanvasCenterY + radius * Math.Sin(angle - arrowAngle);
            }
        }

        public void OnLineMoveEnd()
        {
            IShape angleFrom = (From.attachedClass != null) ? From.attachedClass as IShape : From;
            IShape angleTo = (To.attachedClass != null) ? To.attachedClass as IShape : To;
            double angle = -Math.Atan2((angleTo.CanvasCenterY - angleFrom.CanvasCenterY), (angleTo.CanvasCenterX - angleFrom.CanvasCenterX));

            if (From.attachedClass != null)
            {            // Top, Bottom, Left, Right
                From.connectedLines.Remove(this);
                if (Math.Sin(angle) > rootOfHalf)
                    From = From.attachedClass.Dots.ElementAt(0);
                else if (Math.Sin(angle) < -rootOfHalf)
                    From = From.attachedClass.Dots.ElementAt(1);
                else if (Math.Cos(angle) < -rootOfHalf)
                    From = From.attachedClass.Dots.ElementAt(2);
                else if (Math.Cos(angle) > rootOfHalf)
                    From = From.attachedClass.Dots.ElementAt(3);
                else throw new Exception();
            }

            angle += Math.PI;

            if (To.attachedClass != null) // Code repeat: if this were relegated to a method, getters and setters won't trigger properly.
            {
                To.connectedLines.Remove(this);
                if (Math.Sin(angle) > rootOfHalf)
                    To = To.attachedClass.Dots.ElementAt(0);
                else if (Math.Sin(angle) < -rootOfHalf)
                    To = To.attachedClass.Dots.ElementAt(1);
                else if (Math.Cos(angle) < -rootOfHalf)
                    To = To.attachedClass.Dots.ElementAt(2);
                else if (Math.Cos(angle) > rootOfHalf)
                    To = To.attachedClass.Dots.ElementAt(3);
                else throw new Exception();
            }
            OnLineMove();
        }

        public override string ToString() => "Line";
        #endregion
    }
}
