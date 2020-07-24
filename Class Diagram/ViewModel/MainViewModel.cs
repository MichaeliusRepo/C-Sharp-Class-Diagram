using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Class_Diagram.Model;
using Class_Diagram.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Threading;

namespace Class_Diagram.ViewModel
{
    public class MainViewModel : BaseViewModel

    {
        //TODO Visual Highlighting
        //TODO Adorner layer on classes (A.K.A better mouse movement on classes)
        //TODO Comments on arrows
        //TODO Fancy Zoom (zoom on mouse, not at origo (0,0))

        #region Fields

        private UndoRedoController undoRedoController = UndoRedoController.Instance;
        private EMainViewModel State;
        private EClass addingClassType;
        private ELine addingLineType;

        private Dot addingLineFrom;
        private object focusedObject;
        private Point initialMousePosition;
        private Point initialObjectPosition;
        private Class Clipholder;
        private bool asyncProcess;

        public ObservableCollection<Class> Classes { get; set; }
        public ObservableCollection<Line> Lines { get; set; }
        public ObservableCollection<Dot> Dots { get; set; }

        public ICommand NewDiagramCommand => new RelayCommand(NewDiagram);
        public ICommand OpenDiagramCommand => new AsyncRelayCommand<object>(OpenDiagram, (a) => { return !this.asyncProcess; });
        public ICommand SaveDiagramCommand => new AsyncRelayCommand<object>(SaveDiagram, (a) => { return !this.asyncProcess; });
        public ICommand UndoCommand => new RelayCommand(Undo, undoRedoController.CanUndo);
        public ICommand RedoCommand => new RelayCommand(Redo, undoRedoController.CanRedo);
        public ICommand CutCommand => new RelayCommand(Cut);
        public ICommand CopyCommand => new RelayCommand(Copy);
        public ICommand PasteCommand => new RelayCommand(Paste);
        public ICommand DeleteCommand => new RelayCommand(Delete);
        public ICommand ExitCommand => new RelayCommand(Exit);

        public ICommand MouseMoveCommand => new RelayCommand<MouseEventArgs>(MouseMove);
        public ICommand MouseLeftButtonUpCommand => new RelayCommand<MouseButtonEventArgs>(MouseLeftButtonUp);
        public ICommand MouseWheelCommand => new RelayCommand<MouseWheelEventArgs>(MouseWheel);

        public ICommand MouseDownObjectCommand => new RelayCommand<MouseButtonEventArgs>(MouseDownObject);
        public ICommand MouseUpObjectCommand => new RelayCommand<MouseButtonEventArgs>(MouseUpObject);
        public ICommand SizeChangedClassCommand => new RelayCommand<SizeChangedEventArgs>(SizeChangedClass);
        public ICommand DragAndDropDotCommand => new RelayCommand(DragAndDropDot);
        public ICommand DragAndDropClassCommand => new RelayCommand(DragAndDropClass);
        public ICommand DragAndDropInterfaceCommand => new RelayCommand(DragAndDropInterface);
        public ICommand DragAndDropAbstractCommand => new RelayCommand(DragAndDropAbstract);
        public ICommand DragAndDropEnumerationCommand => new RelayCommand(DragAndDropEnumeration);
        public ICommand AddSolidCommand => new RelayCommand(AddSolid);
        public ICommand AddDashedCommand => new RelayCommand(AddDashed);
        public ICommand AddArrowCommand => new RelayCommand(AddArrow);

        public ICommand ExportCommand => new RelayCommand(Export);
        //public ICommand SaveUsingEncoderCommand => new RelayCommand(SaveUsingEncoder);

        #endregion

        #region BindableProperties

        public string ClassName
        {
            get { return (focusedObject is Class) ? (focusedObject as Class).Name : ""; }
            set
            {
                if (focusedObject is Class)
                    (focusedObject as Class).Name = value;
                OnPropertyChanged("ClassName");
            }
        }

        public string MouseXY { get { return "X: " + MousePosX + ", Y: " + MousePosY; } }

        private int mouseposx;
        public int MousePosX
        {
            get { return mouseposx; }
            set
            {
                mouseposx = value;
                OnPropertyChanged("MousePosX");
            }
        }

        private int mouseposy;
        public int MousePosY
        {
            get { return mouseposy; }
            set
            {
                mouseposy = value;
                OnPropertyChanged("MousePosY");
            }
        }

        private string message = "Ready.";
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private double zoomvalue = 1;
        public double ZoomValue
        {
            get { return zoomvalue; }
            set
            {
                if (value < 0.5)
                    zoomvalue = 0.5;
                else if (value > 2)
                    zoomvalue = 2;
                else
                    zoomvalue = value;
                OnPropertyChanged("ZoomValue");
            }
        }

        public string LinesAmount { get { return "Lines: " + (Lines.Count); } }

        public string ClassAmount { get { return "Classes: " + (Classes.Count); } }

        public string Zoom { get { return "Zoom: " + Math.Round(ZoomValue * 100) + "%"; } }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            Classes = new ObservableCollection<Class>() { };
            Lines = new ObservableCollection<Line>() { };
            Dots = new ObservableCollection<Dot>() { };
        }

        #endregion

        #region Methods (shortcuts, mouse movement)

        private void NewDiagram()
        { // Using this will also shut down the Visual Studio Diagnostic Tools.
            System.Windows.Forms.Application.Restart();
            Exit();
        }

        private void OpenDiagram(object param)
        {
            Monitor.Enter(this);
            if (!asyncProcess)
            {
                this.asyncProcess = true;
                var openFileDialog = new Microsoft.Win32.OpenFileDialog() { FileName = "Class Diagram", DefaultExt = ".xml", Filter = "XML Diagrams|*.xml" };
                if (openFileDialog.ShowDialog() == true)
                {
                    var serializer = new XmlSerializer(typeof(Diagram));
                    var fileStream = new FileStream(openFileDialog.FileName, FileMode.Open);
                    try
                    {
                        var diagram = serializer.Deserialize(fileStream) as Diagram;
                        diagram.OnDeserialization();

                        this.Classes = new ObservableCollection<Class>(diagram.Classes);
                        OnPropertyChanged("Classes");
                        this.Dots = new ObservableCollection<Dot>(diagram.Dots);
                        OnPropertyChanged("Dots");
                        this.Lines = new ObservableCollection<Line>(diagram.Lines);
                        OnPropertyChanged("Lines");

                        fileStream.Close();
                        Message = "Diagram was opened.";
                    }
                    catch (InvalidOperationException)
                    { Message = "Diagram could not be opened."; }
                }
                this.asyncProcess = false;
            }
            Monitor.Exit(this);

        }

        private void SaveDiagram(object param)
        {
            Monitor.Enter(this);
            if (!asyncProcess)
            {
                this.asyncProcess = true;

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog() { FileName = "Class Diagram", DefaultExt = ".xml", Filter = "XML Diagrams|*.xml" };
                var diagram = new Diagram() { Classes = new List<Class>(Classes), Dots = new List<Dot>(Dots), Lines = new List<Line>(Lines) };
                diagram.OnSerialization();
                if (saveFileDialog.ShowDialog() == true)
                {
                    var serializer = new XmlSerializer(typeof(Diagram));
                    var writer = new StreamWriter(saveFileDialog.FileName);
                    serializer.Serialize(writer, diagram);
                    writer.Close();
                    Message = "Diagram was saved.";
                }
                this.asyncProcess = false;
            }
            Monitor.Exit(this);
        }

        private void Cut()
        {
            Copy();
            if (focusedObject is Class)
            {
                AddAndExecute(new RemoveClassCommand(Classes, focusedObject as Class, Dots, Lines));
                Message = "Class was cut.";
            }
        }

        private void Copy()
        {
            if (focusedObject is Class)
            {
                Clipholder = new Class(focusedObject as Class);
                Message = "Class was copied.";
            }
        }

        private void Paste()
        {
            Point p = RelativeMousePosition();
            if (MouseInCanvas() && Clipholder != null)
            {
                AddAndExecute(new AddClassCommand(Classes, new Class(Clipholder) { CanvasCenterX = p.X, CanvasCenterY = p.Y }, Dots));
                Message = "Class was pasted.";
            }
        }

        private void Delete()
        {
            if (focusedObject is Line)
                AddAndExecute(new RemoveLineCommand(Lines, focusedObject as Line));
            else if (focusedObject is Dot)
                AddAndExecute(new RemoveDotCommand(Dots, focusedObject as Dot, Lines));
            else if (focusedObject is Class)
                AddAndExecute(new RemoveClassCommand(Classes, focusedObject as Class, Dots, Lines));
            else return;
            Message = focusedObject.ToString() + " was deleted.";
            focusedObject = null;
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void Undo()
        {
            undoRedoController.Undo();
            UpdateUIforUndoRedo();
        }

        private void Redo()
        {
            undoRedoController.Redo();
            UpdateUIforUndoRedo();
        }

        public void AddAndExecute(IUndoRedoCommand command)
        {
            undoRedoController.AddAndExecute(command);
            UpdateUIforUndoRedo();
        }

        private void MouseMove(MouseEventArgs e)
        {
            Point point = RelativeMousePosition();
            MousePosX = Convert.ToInt32(point.X);
            MousePosY = Convert.ToInt32(point.Y);
            OnPropertyChanged("MouseXY");
            if (Mouse.Captured != null && State == EMainViewModel.MovingObject)
            {
                var shape = focusedObject as IShape;
                shape.OnMove();
                shape.X = initialObjectPosition.X + (point.X - initialMousePosition.X);
                shape.Y = initialObjectPosition.Y + (point.Y - initialMousePosition.Y);
                if (MouseInCanvas())
                    Mouse.SetCursor(Cursors.ScrollAll);
                else
                    Mouse.SetCursor(Cursors.No);
            }

            else if (State == EMainViewModel.AddingDot || State == EMainViewModel.AddingClass)
                if (MouseInCanvas())
                    Mouse.SetCursor(Cursors.Cross);
                else
                    Mouse.SetCursor(Cursors.No);

            else if (State == EMainViewModel.AddingLine)
                if (MouseInCanvas())
                    Mouse.SetCursor(Cursors.Pen);
                else
                    Mouse.SetCursor(Cursors.No);
        }

        private void MouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Point p = e.GetPosition(Application.Current.MainWindow.FindName("MainCanvas") as Grid);
            Point p = RelativeMousePosition();

            if (State == EMainViewModel.AddingDot)
            {
                e.Handled = true;
                State = EMainViewModel.Null;
                if (!MouseInCanvas())
                    return;

                // TODO Break line by dropping dot on line. Needs its own Command class.
                //var visualElement = TargetObject(e);
                //if (visualElement is Line)
                //{
                //    var line = visualElement as Line;
                //    AddAndExecute(new AddDotCommand(Dots, new Dot() { CanvasCenterX = p.X, CanvasCenterY = p.Y }));
                //    var dot = Dots.Last();
                //    if (line.Style == ELine.Dashed)
                //        AddAndExecute(new AddLineCommand(Lines, new Line() { From = line.From, To = dot, Style = ELine.Dashed }));
                //    else
                //        AddAndExecute(new AddLineCommand(Lines, new Line() { From = line.From, To = dot, Style = ELine.Solid }));
                //    AddAndExecute(new AddLineCommand(Lines, new Line() { From = dot, To = line.To, Style = line.Style }));
                //    AddAndExecute(new RemoveLineCommand(Lines, line));
                //}
                //else

                    AddAndExecute(new AddDotCommand(Dots, new Dot() { CanvasCenterX = p.X, CanvasCenterY = p.Y }));
                Message = "Dot added.";

            }
            else if (State == EMainViewModel.AddingClass)
            {
                e.Handled = true;
                State = EMainViewModel.Null;
                if (!MouseInCanvas())
                    return;
                AddAndExecute(new AddClassCommand(Classes, new Class() { CanvasCenterX = p.X, CanvasCenterY = p.Y, Style = addingClassType, Name = "Unnamed " + addingClassType }, Dots));
                Message = addingClassType + " was added.";
            }
            else if (State == EMainViewModel.AddingLine && !(TargetObject(e) is Dot) && MouseInCanvas())
            {
                e.Handled = true;
                focusedObject = null;
                State = EMainViewModel.Null;
                Message = "Adding line failed.";
            }

        }

        public Point RelativeMousePosition()
        {
            return Mouse.GetPosition(Application.Current.MainWindow.FindName("MainCanvas") as Grid);
        }

        private Object TargetObject(MouseEventArgs e)
        {
            return (e.MouseDevice.Target as FrameworkElement).DataContext;
        }

        private bool MouseInCanvas()
        {
            Point p = RelativeMousePosition();
            int windowOffset = 87;
            double offset = Application.Current.MainWindow.ActualHeight - windowOffset;
            return (p.X > 0 && p.Y > 0 && p.Y < offset);
        }

        private void MouseWheel(MouseWheelEventArgs e)
        {
            ZoomValue = e.Delta > 0 ? ZoomValue += 0.1 : ZoomValue -= 0.1;
            OnPropertyChanged("Zoom");
        }

        #endregion


        #region Methods (objects)

        private void MouseDownObject(MouseButtonEventArgs e)
        {
            if (focusedObject != TargetObject(e))
            {
                focusedObject = TargetObject(e);
                OnPropertyChanged("ClassName");
                if (State != EMainViewModel.AddingLine)
                    Message = focusedObject.ToString() + " was selected.";
                return;
            }
            if (focusedObject is IShape)
            {
                var shape = focusedObject as IShape;
                if (shape is Dot && (shape as Dot).attachedClass != null)
                    return;
                initialMousePosition = RelativeMousePosition();
                initialObjectPosition = new Point(shape.X, shape.Y);
                e.MouseDevice.Target.CaptureMouse();
                State = EMainViewModel.MovingObject;
            }
        }

        private void MouseUpObject(MouseButtonEventArgs e)
        {
            var shape = focusedObject as IShape;
            var mousePosition = RelativeMousePosition();

            if (State == EMainViewModel.AddingLine)
            {
                if (focusedObject is Dot)
                    if (addingLineFrom == null)
                    {
                        addingLineFrom = focusedObject as Dot;
                        Message = "Select second dot.";
                        return;
                    }
                    else if (addingLineFrom != focusedObject as Dot)
                    {
                        AddAndExecute(new AddLineCommand(Lines, new Line() { From = addingLineFrom, To = focusedObject as Dot, Style = addingLineType }));
                        Message = "Line added.";
                    }

                focusedObject = null;
                addingLineFrom = null;
                e.Handled = true;
                State = EMainViewModel.Null;
            }

            else if (Mouse.Captured != null && State == EMainViewModel.MovingObject)
            {
                shape.X = initialObjectPosition.X;
                shape.Y = initialObjectPosition.Y;
                e.MouseDevice.Target.ReleaseMouseCapture();
                State = EMainViewModel.Null;
                if (MouseInCanvas())
                    AddAndExecute(new MoveShapeCommand(shape, mousePosition.X - initialMousePosition.X, mousePosition.Y - initialMousePosition.Y));
                else
                {
                    shape.OnMoveEnd();
                    Message = "Shape was out of bounds.";
                }
            }

        }

        private void SizeChangedClass(SizeChangedEventArgs e)
        {
            var visualElement = e.Source as FrameworkElement;
            var element = visualElement.DataContext as Class;
            element.OnSizeChanged(e.NewSize.Width, e.NewSize.Height);
        }

        private void DragAndDropDot()
        {
            State = EMainViewModel.AddingDot;
            Message = "Drag dot into canvas.";
        }

        private void DragAndDropClass()
        {
            State = EMainViewModel.AddingClass;
            addingClassType = EClass.Class;
            Message = "Drag class into canvas.";
        }

        private void DragAndDropInterface()
        {
            State = EMainViewModel.AddingClass;
            addingClassType = EClass.Interface;
            Message = "Drag interface into canvas.";
        }

        private void DragAndDropAbstract()
        {
            State = EMainViewModel.AddingClass;
            addingClassType = EClass.Abstract;
            Message = "Drag abstract into canvas.";
        }

        private void DragAndDropEnumeration()
        {
            State = EMainViewModel.AddingClass;
            addingClassType = EClass.Enumeration;
            Message = "Drag enumeration into canvas.";
        }

        private void AddSolid()
        {
            State = EMainViewModel.AddingLine;
            addingLineType = ELine.Solid;
            focusedObject = null;
            Message = "Select first dot.";
        }

        private void AddDashed()
        {
            State = EMainViewModel.AddingLine;
            addingLineType = ELine.Dashed;
            focusedObject = null;
            Message = "Select first dot.";
        }

        private void AddArrow()
        {
            State = EMainViewModel.AddingLine;
            addingLineType = ELine.Arrow;
            focusedObject = null;
            Message = "Select first dot.";
        }

        public void Export()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog() { Filter = "PNG Files (*.png)|*.png", DefaultExt = ".png", FileName = "DesignerImage.png", RestoreDirectory = true };

            if (saveFileDialog.ShowDialog() == true)
            {
                Grid grid = Application.Current.MainWindow.FindName("MainCanvas") as Grid;
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)grid.ActualWidth, (int)grid.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
                grid.Measure(new Size((int)grid.ActualWidth, (int)grid.ActualHeight));
                grid.Arrange(new Rect(new Size((int)grid.ActualWidth, (int)grid.ActualHeight)));
                renderBitmap.Render(grid);
                BitmapEncoder imageEncoder = new PngBitmapEncoder();
                imageEncoder = new PngBitmapEncoder();
                imageEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    imageEncoder.Save(file);
                }
            }

        }

        private void UpdateUIforUndoRedo()
        {
            OnPropertyChanged("ClassAmount");
            OnPropertyChanged("LinesAmount");
        }
        #endregion

    }
}
