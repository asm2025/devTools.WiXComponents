using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using devTools.WiXComponents.Core.Commands;
using devTools.WiXComponents.Core.Services;
using essentialMix.Collections;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "From directory", Order = 0)]
	public sealed class EntriesFromDirectoryViewModel : ViewModelCancellableCommandBase
	{
		private readonly EntriesGeneratorService _generator;

		private string _path;
		private string _pattern;
		private string _exclude;
		private bool _includeSubdirectories;
		private bool _append;

		/// <inheritdoc />
		public EntriesFromDirectoryViewModel(EntriesGeneratorService generator, ILogger<EntriesFromDirectoryViewModel> logger)
			: base(logger)
		{
			_generator = generator;
			_generator.PropertyChanged += OnGeneratorChanged;
			StartCommand = new RelayCommand<EntriesFromDirectoryViewModel>(vm => vm.Generate(), 
																			vm => !vm.IsBusy && vm.Path != null);
		}

		public string RootPath
		{
			get => _generator.RootPath;
			set
			{
				if (_generator.RootPath == value) return;
				_generator.RootPath = value;
				OnPropertyChanged();
			}
		}

		public string Path
		{
			get => _path;
			set
			{
				if (_path == value) return;
				_path = System.IO.Path.GetFullPath(value.ToNullIfEmpty() ?? ".\\");
				OnPropertyChanged();
			}
		}

		public string Pattern
		{
			get => _pattern;
			set
			{
				if (_pattern == value) return;
				_pattern = value.ToNullIfEmpty();
				OnPropertyChanged();
			}
		}

		public string Exclude
		{
			get => _exclude;
			set
			{
				if (_exclude == value) return;
				_exclude = value.ToNullIfEmpty();
				OnPropertyChanged();
			}
		}

		public bool IncludeSubdirectories
		{
			get => _includeSubdirectories;
			set
			{
				if (_includeSubdirectories == value) return;
				_includeSubdirectories = value;
				OnPropertyChanged();
			}
		}

		public bool Append
		{
			get => _append;
			set
			{
				if (_append == value) return;
				_append = value;
				OnPropertyChanged();
			}
		}

		public IReadOnlySet<string> Entries => _generator.Entries;

		/// <inheritdoc />
		public override ICommand StartCommand { get; }

		public void Generate()
		{
			if (IsBusy) return;
			IsBusy = true;

			try
			{
				_generator.GenerateFromDirectory(Path, new GenerateSettings(Pattern, IncludeSubdirectories, Append)
				{
					Exclude = Exclude
				});
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.CollectMessages());
				throw;
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void OnGeneratorChanged(object sender, [NotNull] PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(EntriesGeneratorService.RootPath):
					OnPropertyChanged(nameof(RootPath));
					break;
				case nameof(EntriesGeneratorService.Entries):
					OnPropertyChanged(nameof(Entries));
					break;
			}
		}
	}
}