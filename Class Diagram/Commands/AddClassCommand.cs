using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Diagram.Model;
using Class_Diagram.ViewModel;
using System.Collections.ObjectModel;

namespace Class_Diagram.Commands
{
    public class AddClassCommand : IUndoRedoCommand

    {
        private ObservableCollection<Class> classes;
        private Class _class;
        private ObservableCollection<Dot> dots;

        public AddClassCommand(ObservableCollection<Class> classes, Class _class, ObservableCollection<Dot> dots)
        {
            this.classes = classes;
            this._class = _class;
            this.dots = dots;
        }

        public void Execute()
        {
            classes.Add(_class);
            foreach (Dot dot in _class.Dots)
                this.dots.Add(dot);
            _class.OnMoveEnd();
        }

        public void UnExecute()
        {
            classes.Remove(_class);
            foreach (Dot dot in _class.Dots)
                this.dots.Remove(dot);
        }

    }
}
