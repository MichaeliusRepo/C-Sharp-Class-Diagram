using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Diagram.Model;
using System.Collections.ObjectModel;

namespace Class_Diagram.Commands
{
    public class AddDotCommand : IUndoRedoCommand
    {
        private ObservableCollection<Dot> dots;
        private Dot dot;

        public AddDotCommand(ObservableCollection<Dot> dots, Dot dot)
        {
            this.dots = dots;
            this.dot = dot;
        }

        public void Execute()
        {
            dots.Add(dot);
        }

        public void UnExecute()
        {
            dots.Remove(dot);
        }
    }
}
