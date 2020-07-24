using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Diagram.Model
{
    [Serializable]
    public class Diagram
    {
        public List<Class> Classes { get; set; }
        public List<Dot> Dots { get; set; }
        public List<Line> Lines { get; set; }

        public List<int> classReferences;
        public List<int> lineReferences;

        public Diagram()
        {
            classReferences = new List<int>();
            lineReferences = new List<int>();
        }

        public void OnSerialization()
        { // Runtime of this algorithm is about O(n^2)
            foreach (Class _class in Classes)
                classReferences.Add(Dots.FindIndex(a => a == _class.Dots.ElementAt(0)));

            foreach (Line line in Lines)
            {
                lineReferences.Add(Dots.FindIndex(a => a == line.From));
                lineReferences.Add(Dots.FindIndex(a => a == line.To));
            }

        }

        public void OnDeserialization()
        { // This algorithm also likely has an runtime of O(n^2)

            foreach (Line line in Lines)
            {
                line.From = Dots.ElementAt(lineReferences.ElementAt(0));
                line.From.connectedLines.Add(line);
                lineReferences.RemoveAt(0);

                line.To = Dots.ElementAt(lineReferences.ElementAt(0));
                line.To.connectedLines.Add(line);
                lineReferences.RemoveAt(0);
            }

            foreach (Class _class in Classes)
            {
                _class.Dots = new List<Dot>();

                int i = classReferences.ElementAt(0); // Reference first index of this class' 4 dots.
                for (int a = 0; a < 4; a++) // Loop through all 4 dots
                    _class.Dots.Add(this.Dots.ElementAt(i + a));
                foreach (Dot dot in _class.Dots)
                    dot.attachedClass = _class;
                classReferences.RemoveAt(0);
                _class.OnMoveEnd();
            }

        }
    }
}
