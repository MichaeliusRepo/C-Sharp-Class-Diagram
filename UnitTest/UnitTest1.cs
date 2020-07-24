using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Class_Diagram.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Class_Diagram.Model;
using Class_Diagram.Commands;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // By default, unit testing assumes that exceptions are thrown when actions are invalid.

            // CTRL R, A to run
            // Alternatively, go to Tests > Run > All Tests

            // arrange
            MainViewModel mainViewModel = new MainViewModel();
            ObservableCollection<Class> Classes = mainViewModel.Classes;
            ObservableCollection<Line> Lines = mainViewModel.Lines;
            ObservableCollection<Dot> Dots = mainViewModel.Dots;
            Point p = new Point(420, 666);
            EClass addingClassType = EClass.Class;

            // If our code had been written in a more maintainable manner, maybe triggering ICommand directly could have worked.

            //if (mainViewModel.DragAndDropClassCommand.CanExecute(null))
            //    mainViewModel.DragAndDropClassCommand.Execute(null);
            //MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            //if (mainViewModel.MouseLeftButtonUpCommand.CanExecute(e))
            //    mainViewModel.MouseLeftButtonUpCommand.Execute(e);

            // assert
            Assert.IsTrue(Classes.Count == 0);

            // act
            mainViewModel.AddAndExecute(new AddClassCommand(Classes, new Class() { CanvasCenterX = p.X, CanvasCenterY = p.Y, Style = addingClassType, Name = "Unnamed " + addingClassType }, Dots));

            // assert
            Assert.IsFalse(Classes.Count == 0);
            Assert.IsTrue(Classes.Count == 1);

            Class addedClass = Classes.ElementAt(0);
            Assert.AreEqual(addedClass.CanvasCenterX, p.X);
            Assert.AreEqual(addedClass.CanvasCenterY, p.Y);
            Assert.AreEqual(Dots.Count, 4); // Adding a class should add 4 dots.

            // act
            if (mainViewModel.UndoCommand.CanExecute(null))
                mainViewModel.UndoCommand.Execute(null);

            // assert
            Assert.IsTrue(Classes.Count == 0);
            Assert.IsTrue(Dots.Count == 0);
        }
    }
}

