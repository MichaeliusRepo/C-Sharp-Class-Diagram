using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Diagram.Model
{
    public class Class : NotifyPropertyBase, IShape
    {
        #region Fields
        [XmlIgnore]
        public List<Dot> Dots;
        //public Dot Top, Bottom, Left, Right;
        private readonly double DotOffset = 2;

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

        private double width = 174;
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => CanvasCenterX);
            }
        }

        private double height = 56;
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => CanvasCenterY);
            }
        }

        public double CanvasCenterX
        {
            get { return X + Width / 2; }
            set
            {
                X = value - Width / 2;
                NotifyPropertyChanged(() => X);
            }
        }

        public double CanvasCenterY
        {
            get { return Y + Height / 2; }
            set
            {
                Y = value - Height / 2;
                NotifyPropertyChanged(() => Y);
            }
        }

        private EClass style = EClass.Class;
        public EClass Style
        {
            get { return style; }
            set
            {
                style = value;
                NotifyPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        private string content = "-email: String\n- address: String\n+register()\n+login()\n+updateProfile()";
        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                NotifyPropertyChanged();
            }
        }

        private bool isExpanded = false;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public Class()
        {
            Dots = new List<Dot>() {
                new Dot() { CanvasCenterX = this.CanvasCenterX, CanvasCenterY = this.Y - DotOffset, attachedClass = this},   // Top
                new Dot() { CanvasCenterX=  this.CanvasCenterX, CanvasCenterY =  this.Y + Height + DotOffset, attachedClass = this }, // Bottom
                new Dot() { CanvasCenterX =  this.X - DotOffset, CanvasCenterY= CanvasCenterY, attachedClass = this }, // Left
                new Dot() { CanvasCenterX = this.X + Width + DotOffset, CanvasCenterY = this.CanvasCenterY, attachedClass = this } // Right
             };
        }

        public Class(Class copy) : this()
        {
            this.Style = copy.Style;
            this.Name = copy.Name;
            this.Content = copy.Content;
            this.IsExpanded = copy.IsExpanded;
            OnSizeChanged(copy.Width, copy.Height);
        }

        #endregion

        #region Methods

        public void OnSizeChanged(double width, double height)
        {
            this.Width = width;
            this.Height = height;
            OnMove();

            foreach (Dot dot in Dots)
                dot.OnMoveEnd();
        }

        public void OnMove()
        {
            SetDotPositions();
            foreach (Dot dot in Dots)
                dot.OnMove();
        }

        public void OnMoveEnd()
        {
            SetDotPositions();
            foreach (Dot dot in Dots)
                dot.OnMoveEnd();
        }

        private void SetDotPositions()
        {            // Top, Bottom, Left, Right
            Dots.ElementAt(0).CanvasCenterX = Dots.ElementAt(1).CanvasCenterX = CanvasCenterX;
            Dots.ElementAt(0).CanvasCenterY = Y - DotOffset;
            //Dots.ElementAt(1).CanvasCenterX = CanvasCenterX;
            Dots.ElementAt(1).CanvasCenterY = Y + Height + DotOffset;
            Dots.ElementAt(2).CanvasCenterX = X - DotOffset;
            Dots.ElementAt(2).CanvasCenterY = Dots.ElementAt(3).CanvasCenterY = CanvasCenterY;
            Dots.ElementAt(3).CanvasCenterX = X + Width + DotOffset;
            //Dots.ElementAt(3).CanvasCenterY = CanvasCenterY;
        }

        public override string ToString() => Style + " '" + Name + "'";

        #endregion

    }
}
