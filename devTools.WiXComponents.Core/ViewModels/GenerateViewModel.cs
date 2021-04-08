using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Input;
using devTools.WiXComponents.Core.Models;
using devTools.WiXComponents.Core.Services;
using essentialMix.Core.WPF.Commands;
using essentialMix.Extensions;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "Generate", Order = 0)]
	public sealed class GenerateViewModel : CancellableViewModelBase
	{
		private readonly GeneratorService _service;

		private string _heatPath;
		private string _targetPath;
		private string _targetFile;

		/// <inheritdoc />
		public GenerateViewModel(GeneratorService service, ILogger<GenerateViewModel> logger)
			: base(logger)
		{
			_service = service;
			_service.PropertyChanged += OnServiceChanged;
			UseHeatCommand = new RelayCommand(GenerateUsingHeat, () => !IsBusy && HeatPath != null && TargetPath != null)
				.ListenOn(this, nameof(HeatPath), nameof(TargetPath));
			FromDirectoryCommand = new RelayCommand(GenerateFromDirectory, () => !IsBusy && TargetPath != null)
				.ListenOn(this, nameof(TargetPath));
			FromMissingCommand = new RelayCommand(GenerateFromMissing, () => !IsBusy && TargetPath != null && TargetFile != null)
				.ListenOn(this, nameof(TargetPath), nameof(TargetFile));
		}

		public string HeatPath
		{
			get => _heatPath;
			set
			{
				if (_heatPath == value) return;
				// use PathHelper.Trim because it's a file path. So .\ or ..\ won't work here
				string path = PathHelper.Trim(value);
				if (_heatPath == path) return;
				_heatPath = path;
				OnPropertyChanged();
			}
		}

		public string TargetPath
		{
			get => _targetPath;
			set
			{
				if (_targetPath == value) return;
				string path = PathHelper.Trim(value);
				if (path != null) path = Path.GetFullPath(path).Suffix(Path.DirectorySeparatorChar);
				if (_targetPath == path) return;
				_targetPath = path;
				OnPropertyChanged();
			}
		}

		public string TargetFile
		{
			get => _targetFile;
			set
			{
				if (_targetFile == value) return;
				string path = PathHelper.Trim(value);
				if (path != null) path = Path.GetFullPath(path);
				if (_targetFile == path) return;
				_targetFile = path;
				OnPropertyChanged();
			}
		}

		public GenerateSettings Settings => _service.Settings;

		[NotNull]
		public essentialMix.Collections.IReadOnlySet<string> Entries => _service.Entries;

		public ICommand UseHeatCommand { get; }
		public ICommand FromDirectoryCommand { get; }
		public ICommand FromMissingCommand { get; }

		public void GenerateUsingHeat()
		{
			if (IsBusy) return;
			
			string heatPath = HeatPath;
			if (string.IsNullOrEmpty(heatPath)) return;
			IsBusy = true;

			try
			{
				_service.GenerateUsingHeat(heatPath, TargetPath);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public void GenerateFromDirectory()
		{
			if (IsBusy) return;
			IsBusy = true;

			try
			{
				_service.GenerateFromDirectory(TargetPath);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public void GenerateFromMissing()
		{
			if (IsBusy) return;
			IsBusy = true;

			try
			{
				_service.GenerateFromMissing(TargetPath, TargetFile);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void OnServiceChanged(object sender, [NotNull] PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(Entries)) return;
			OnPropertyChanged(e);
		}
	}
}