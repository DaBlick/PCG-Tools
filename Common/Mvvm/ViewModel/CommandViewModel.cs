using System;
using System.Windows.Input;

namespace Common.Mvvm.ViewModel
{
    /// <summary>
    /// Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel : ViewModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="command"></param>
        public CommandViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            Command = command;
        }


        /// <summary>
        /// 
        /// </summary>
        public ICommand Command { get; private set; }
    }
}