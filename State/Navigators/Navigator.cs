using System.Windows.Input;
using devTools.WiXComponents.Commands;
using devTools.WiXComponents.ViewModels;
using essentialMix.Patterns.NotifyChange;

namespace devTools.WiXComponents.State.Navigators
{
	public class Navigator : NotifyPropertyChangedBase, INavigator
	{
		private ViewModelBase _viewModel;

		public Navigator()
		{
			UpdateViewModel = new UpdateViewModelCommand(this);
		}

		/// <inheritdoc />
		public ViewModelBase ViewModel
		{
			get => _viewModel;
			set
			{
				if (_viewModel == value) return;
				_viewModel = value;
				OnPropertyChanged();
			}
		}

		/// <inheritdoc />
		public ICommand UpdateViewModel { get; }
	}
}