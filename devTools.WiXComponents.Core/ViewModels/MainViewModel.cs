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

namespace devTools.WiXComponents.Core.ViewModels;

/// <inheritdoc />
public sealed class MainViewModel : ViewModelBase
{
	private const string THEME_LIGHT = "Light theme";
	private const string THEME_DARK = "Dark theme";
	private const string STATUS_DEF = "Ready...";

	private static readonly HashSet<string> WATCHED_CHILD_PROPERTIES = new HashSet<string>
	{
		nameof(Status),
		nameof(Operation),
		nameof(Progress),
		nameof(ProgressState),
	};

	private CommandViewModelBase _selectedViewModel;

	/// <inheritdoc />
	public MainViewModel([NotNull] IApp app, ILogger logger)
		: base(logger)
	{
		App = app;
		Info = new AppInfo(AssemblyHelper.GetEntryAssembly());
		ViewModels = new ObservableCollection<CommandViewModelBase>();
		ChangeView = new RelayCommand<CommandViewModelBase>((_, vm) =>
		{
			if (vm is IResettableView resettableView) resettableView.Reset();
			SelectedViewModel = vm;
		}, (_, vm) => vm != SelectedViewModel && vm.CanView() && SelectedViewModel is not CancellableViewModelBase { IsBusy: true });
	}

	[NotNull]
	public IApp App { get; }

	[NotNull]
	public AppInfo Info { get; }

	public bool DarkTheme
	{
		get => App.DarkTheme;
		set
		{
			if (App.DarkTheme == value) return;
			App.DarkTheme = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(ThemeName));
		}
	}

	[NotNull]
	public string ThemeName => DarkTheme ? THEME_DARK : THEME_LIGHT;

	[NotNull]
	public ICommand ChangeView { get; }

	[NotNull]
	public string Status => SelectedCancellableViewModel?.Status ?? STATUS_DEF;
	public string Operation => SelectedCancellableViewModel?.Operation;
	public int Progress => SelectedCancellableViewModel?.Progress ?? 0;
	public TaskbarItemProgressState ProgressState => SelectedCancellableViewModel?.ProgressState ?? TaskbarItemProgressState.None;

	public CommandViewModelBase SelectedViewModel
	{
		get => _selectedViewModel;
		set
		{
			if (_selectedViewModel == value) return;
			if (_selectedViewModel != null) _selectedViewModel.PropertyChanged -= OnChildPropertyChanged;
			_selectedViewModel = value;
			if (_selectedViewModel != null) _selectedViewModel.PropertyChanged += OnChildPropertyChanged;
			SelectedCancellableViewModel = _selectedViewModel as CancellableViewModelBase;
			OnPropertyChanged();

			if (SelectedCancellableViewModel != null)
			{
				SelectedCancellableViewModel.Reset();
			}
			else
			{
				foreach (string prop in WATCHED_CHILD_PROPERTIES)
					OnPropertyChanged(prop);
			}
		}
	}

	public CancellableViewModelBase SelectedCancellableViewModel { get; private set; }

	[NotNull]
	public ObservableCollection<CommandViewModelBase> ViewModels { get; }

	private void OnChildPropertyChanged(object sender, [NotNull] PropertyChangedEventArgs e)
	{
		if (!WATCHED_CHILD_PROPERTIES.Contains(e.PropertyName)) return;
		OnPropertyChanged(e);
	}
}