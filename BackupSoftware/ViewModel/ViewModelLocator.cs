
using Ninject;

namespace BackupSoftware
{
    public class ViewModelLocator
    {
		  public static ViewModelLocator Instance  { get; private set; } = new ViewModelLocator();

		  public static ApplicationViewModel ApplicationViewModel => IoC.Kernel.Get<ApplicationViewModel>();
	}
}
