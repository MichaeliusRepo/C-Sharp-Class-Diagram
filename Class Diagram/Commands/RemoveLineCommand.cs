using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Diagram.Model;
using System.Collections.ObjectModel;

namespace Class_Diagram.Commands
{
    public class RemoveLineCommand : IUndoRedoCommand
    {
        private ObservableCollection<Line> lines;
        private Line line;

        public RemoveLineCommand(ObservableCollection<Line> lines, Line line)
        {
            this.lines = lines;
            this.line = line;
        }

        public void Execute()
        {
            lines.Remove(line);
        }

        public void UnExecute()
        {
            lines.Add(line);
            line.OnLineMoveEnd();
        }

    }
}
