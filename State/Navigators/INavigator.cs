using System.Windows.Input;
using devTools.WiXComponents.ViewModels;
using JetBrains.Annotations;

namespace devTools.WiXComponents.State.Navigators
{
	public interface INavigator
	{
		public ViewModelBase ViewModel { get; set; }
		
		[NotNull]
		public ICommand UpdateViewModel { get; }
	}
}
