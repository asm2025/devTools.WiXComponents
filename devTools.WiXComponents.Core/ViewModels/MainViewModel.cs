using System.Collections.ObjectModel;
using System.Windows.Input;
using devTools.WiXComponents.Core.Commands;
using essentialMix;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	public sealed class MainViewModel : ViewModelBase
	{
		private const string STATUS_DEF = "Ready...";

		private ViewModelCommandBase _selectedViewModel;
		private string _status;
		private string _operation;
		private int _progress;

		/// <inheritdoc />
		public MainViewModel(ILogger logger)
			: base(logger)
		{
			AppInfo appInfo = new AppInfo(AssemblyHelper.GetEntryAssembly());
			Title = appInfo.Title;
			ViewModels = new ObservableCollection<ViewModelCommandBase>();
			ChangeView = new RelayCommand<ViewModelCommandBase>(vm =>
			{
				if (vm is IResettableView resettableView) resettableView.Reset();
				SelectedViewModel = vm;
			}, vm => vm != SelectedViewModel && vm.CanView() && SelectedViewModel is not ViewModelCancellableCommandBase { IsBusy: true });
			Reset();
		}

		[NotNull]
		public string Title { get; }

		[NotNull]
		public ICommand ChangeView { get; }

		public ViewModelCommandBase SelectedViewModel
		{
			get => _selectedViewModel;
			set
			{
				if (_selectedViewModel == value) return;
				_selectedViewModel = value;
				OnPropertyChanged();
			}
		}

		public string Status
		{
			get => _status;
			set
			{
				_status = value ?? STATUS_DEF;
				OnPropertyChanged();
			}
		}

		public string Operation
		{
			get => _operation;
			set
			{
				_operation = value;
				OnPropertyChanged();
			}
		}

		public int Progress
		{
			get => _progress;
			set
			{
				_progress = value;
				OnPropertyChanged();
			}
		}

		[NotNull]
		public ObservableCollection<ViewModelCommandBase> ViewModels { get; }

		public void Reset()
		{
			Status = Operation = null;
			Progress = 0;
		}
	}
}