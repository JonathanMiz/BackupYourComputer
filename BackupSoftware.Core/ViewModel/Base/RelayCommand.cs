using System;
using System.Windows.Input;

namespace BackupSoftware.Core
{
	 public class RelayCommand : ICommand
	 {
		  private Action _Action;
		  private Func<object, bool> _CanExecute;

		  public RelayCommand(Action action, Func<object, bool> canExecute = null)
		  {
			   this._Action = action;
			   _CanExecute = canExecute;
		  }

		  public event EventHandler CanExecuteChanged
		  {
			   add { CommandManager.RequerySuggested += value; }
			   remove { CommandManager.RequerySuggested -= value; }
		  }

		  public bool CanExecute(object parameter)
		  {
			   if (_CanExecute != null)
					return _CanExecute(parameter);

			   return true;
		  }

		  public void Execute(object parameter)
		  {
			   if (_Action != null)
			   {
					_Action();
			   }
		  }
	 }
}
