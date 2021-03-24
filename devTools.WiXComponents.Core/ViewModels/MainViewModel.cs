using System.Collections.Generic;
using essentialMix;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	public class MainViewModel : ViewModelBase
	{
		private ViewModelCommandBase _selectedViewModel;

		/// <inheritdoc />
		public MainViewModel([NotNull] IReadOnlyList<ViewModelCommandBase> viewModels, ILogger logger)
			: base(logger)
		{
			AppInfo appInfo = new AppInfo(AssemblyHelper.GetEntryAssembly());
			Title = appInfo.Title;
			ViewModels = viewModels;
		}

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

		[NotNull]
		public IReadOnlyList<ViewModelCommandBase> ViewModels { get; }

		[NotNull]
		public string Title { get; }
	}
}