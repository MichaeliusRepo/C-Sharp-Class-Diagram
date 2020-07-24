using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Diagram.Model;
using System.Collections.ObjectModel;

namespace Class_Diagram.Commands
{
    public class RemoveDotCommand : IUndoRedoCommand
    {
        private ObservableCollection<Dot> dots;
        private Dot dot;
        private ObservableCollection<Line> lines;

        public RemoveDotCommand(ObservableCollection<Dot> dots, Dot dot, ObservableCollection<Line> lines)
        {
            this.dots = dots;
            this.dot = dot;
            this.lines = lines;
        }

        public void Execute()
        {
            dots.Remove(dot);
            foreach (Line line in dot.connectedLines)
                lines.Remove(line);
        }

        public void UnExecute()
        {
            dots.Add(dot);
            foreach (Line line in dot.connectedLines)
                lines.Add(line);
        }
    }
}
