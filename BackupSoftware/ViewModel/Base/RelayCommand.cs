using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupSoftware
{
	 public class RelayCommand : ICommand
	 {
		  private Action m_action;

		  public RelayCommand(Action action)
		  {
			   this.m_action = action;
		  }

		  public event EventHandler CanExecuteChanged;

		  public bool CanExecute(object parameter)
		  {
			   return true;
		  }

		  public void Execute(object parameter)
		  {
			   if(m_action != null)
			   {
					m_action();
			   }
		  }
	 }
}
