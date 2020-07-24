using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Diagram.Model;
using System.Collections.ObjectModel;

namespace Class_Diagram.Commands
{
    public class RemoveClassCommand :IUndoRedoCommand
    {
        private ObservableCollection<Class> classes;
        private Class _class;
        private ObservableCollection<Dot> dots;
        private ObservableCollection<Line> lines;

        public RemoveClassCommand(ObservableCollection<Class> classes, Class _class, ObservableCollection<Dot> dots, ObservableCollection<Line> lines)
        {
            this.classes = classes;
            this._class = _class;
            this.dots = dots;
            this.lines = lines;
        }

        public void Execute()
        {
            classes.Remove(_class);
            foreach (Dot dot in _class.Dots)
            {
                this.dots.Remove(dot);
                foreach (Line line in dot.connectedLines)
                    lines.Remove(line);
            }
        }

        public void UnExecute()
        {
            classes.Add(_class);
            foreach (Dot dot in _class.Dots)
            {
                this.dots.Add(dot);
                foreach (Line line in dot.connectedLines)
                    lines.Add(line);
            }
            _class.OnMoveEnd();
        }

    }
}
