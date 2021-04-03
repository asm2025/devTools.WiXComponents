using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

		private static readonly HashSet<string> WATCHED_CHILD_PROPERTIES = new HashSet<string>
		{
			nameof(Status),
			nameof(Operation),
			nameof(Progress)
		};

		private ViewModelCommandBase _selectedViewModel;
		private ViewModelCancellableCommandBase _cancellableCommandBaseRef;

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
		}

		[NotNull]
		public string Title { get; }

		[NotNull]
		public ICommand ChangeView { get; }

		public string Status => _cancellableCommandBaseRef?.Status ?? STATUS_DEF;
		public string Operation => _cancellableCommandBaseRef?.Operation;
		public int Progress => _cancellableCommandBaseRef?.Progress ?? 0;

		public ViewModelCommandBase SelectedViewModel
		{
			get => _selectedViewModel;
			set
			{
				if (_selectedViewModel == value) return;
				if (_cancellableCommandBaseRef != null) _cancellableCommandBaseRef.PropertyChanged -= OnChildPropertyChanged;
				_selectedViewModel = value;
				_cancellableCommandBaseRef = _selectedViewModel as ViewModelCancellableCommandBase;

				if (_cancellableCommandBaseRef != null)
				{
					_cancellableCommandBaseRef.PropertyChanged += OnChildPropertyChanged;
					_cancellableCommandBaseRef.Reset();
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
		public ObservableCollection<ViewModelCommandBase> ViewModels { get; }

		private void OnChildPropertyChanged(object sender, [NotNull] PropertyChangedEventArgs e)
		{
			if (!WATCHED_CHILD_PROPERTIES.Contains(e.PropertyName)) return;
			OnPropertyChanged(e);
		}
	}
}