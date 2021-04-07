using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Shell;
using essentialMix;
using essentialMix.Core.WPF.Commands;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	public sealed class MainViewModel : ViewModelBase
	{
		private const string STATUS_DEF = "Ready...";

		private static readonly HashSet<string> WATCHED_CHILD_PROPERTIES = new HashSet<string>
		{
			nameof(Status),
			nameof(Operation),
			nameof(Progress),
			nameof(ProgressState),
		};

		private CommandViewModelBase _selectedViewModel;
		private CancellableViewModelBase _cancellableBaseRef;

		/// <inheritdoc />
		public MainViewModel(ILogger logger)
			: base(logger)
		{
			AppInfo appInfo = new AppInfo(AssemblyHelper.GetEntryAssembly());
			Title = appInfo.Title;
			ViewModels = new ObservableCollection<CommandViewModelBase>();
			ChangeView = new RelayCommand<CommandViewModelBase>(vm =>
			{
				if (vm is IResettableView resettableView) resettableView.Reset();
				SelectedViewModel = vm;
			}, vm => vm != SelectedViewModel && vm.CanView() && SelectedViewModel is not CancellableViewModelBase { IsBusy: true });
		}

		[NotNull]
		public string Title { get; }

		[NotNull]
		public ICommand ChangeView { get; }

		public string Status => _cancellableBaseRef?.Status ?? STATUS_DEF;
		public string Operation => _cancellableBaseRef?.Operation;
		public int Progress => _cancellableBaseRef?.Progress ?? 0;
		public TaskbarItemProgressState ProgressState => _cancellableBaseRef?.ProgressState ?? TaskbarItemProgressState.None;

		public CommandViewModelBase SelectedViewModel
		{
			get => _selectedViewModel;
			set
			{
				if (_selectedViewModel == value) return;
				if (_cancellableBaseRef != null) _cancellableBaseRef.PropertyChanged -= OnChildPropertyChanged;
				_selectedViewModel = value;
				_cancellableBaseRef = _selectedViewModel as CancellableViewModelBase;

				if (_cancellableBaseRef != null)
				{
					_cancellableBaseRef.PropertyChanged += OnChildPropertyChanged;
					_cancellableBaseRef.Reset();
				}
				else
				{
					foreach (string prop in WATCHED_CHILD_PROPERTIES) 
						OnPropertyChanged(prop);
				}

				OnPropertyChanged();
			}
		}

		[NotNull]
		public ObservableCollection<CommandViewModelBase> ViewModels { get; }

		private void OnChildPropertyChanged(object sender, [NotNull] PropertyChangedEventArgs e)
		{
			if (!WATCHED_CHILD_PROPERTIES.Contains(e.PropertyName)) return;
			OnPropertyChanged(e);
		}
	}
}