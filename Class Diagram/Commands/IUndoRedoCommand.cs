using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Diagram.Commands
{
    public interface IUndoRedoCommand
    {
        void Execute();
        void UnExecute();
    }
}
