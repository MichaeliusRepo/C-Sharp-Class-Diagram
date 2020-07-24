using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Diagram.Model;
using System.Collections.ObjectModel;

namespace Class_Diagram.Commands
{
    public class AddLineCommand : IUndoRedoCommand
    {
        private ObservableCollection<Line> lines;
        private Line line;

        public AddLineCommand(ObservableCollection<Line> lines, Line line)
        {
            this.lines = lines;
            this.line = line;
        }

        public void Execute()
        {
            lines.Add(line);
            line.OnLineMoveEnd();
        }

        public void UnExecute()
        {
            lines.Remove(line);
        }

    }
}
