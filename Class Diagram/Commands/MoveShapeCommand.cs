using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Diagram.Model;

namespace Class_Diagram.Commands
{
    public class MoveShapeCommand : IUndoRedoCommand
    {
        private IShape shape;
        private double offsetX;
        private double offsetY;

        public MoveShapeCommand(IShape shape, double offsetX, double offsetY)
        {
            this.shape = shape;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public void Execute()
        {
            shape.CanvasCenterX += offsetX;
            shape.CanvasCenterY += offsetY;
            shape.OnMoveEnd();
        }

        public void UnExecute()
        {
            shape.CanvasCenterX -= offsetX;
            shape.CanvasCenterY -= offsetY;
            shape.OnMoveEnd();
        }

    }
}
