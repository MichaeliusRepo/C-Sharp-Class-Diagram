using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Class_Diagram.ViewModel
{
    public class AsyncRelayCommand<T> : ICommand

        // This class is from an example project by Shane.

    {
        /////////////////////////////////////////////////////////////////////////////////////////////
        #region // Private Members

        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////
        #region // Constructors

        public AsyncRelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public AsyncRelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////
        #region // Public Methods

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Determines whether the action can be performed based on a provided predicate. It returns true if
        /// no predicate to evaluate was provided or if the predicate returns true; otherwise false.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>True or false based on the provided predicate</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke((T)parameter) ?? true;
        }
        /// <summary>
        /// Executes the underlying action asynchronously by creating a new thread.
        /// The thread will be single-threaded apartment.
        /// </summary>
        /// <param name="parameter">generic parameter will be provided to the underlying action upon execution.</param>
        public void Execute(object parameter)
        {
            if (this.execute == null) throw new ArgumentNullException($"in {nameof(Execute)}() Action execute is null");
            // Could perform this as a task, but then it wouldn't be at STA-thread. 
            // This would not allow creation of visual elements which could be necessary.
            // All UI elements in WPF must be created in a Single Threaded Appartment state.
            // Thus the following is bad:
            // var task = Task.Factory.StartNew(() => { this.execute((T)parameter); });
            // await task;

            var thread = new Thread(() => this.execute((T)parameter));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        #endregion
    }
}
