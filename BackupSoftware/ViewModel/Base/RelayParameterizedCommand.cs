using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupSoftware
{
	 public class RelayParameterizedCommand<T> : System.Windows.Input.ICommand
	 {
		  private Action<T> m_action;

		  public RelayParameterizedCommand(Action<T> action)
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
					m_action((T)parameter);
			   }
		  }
	 }
}
