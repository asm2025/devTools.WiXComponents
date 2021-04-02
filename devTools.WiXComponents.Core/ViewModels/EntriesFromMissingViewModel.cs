using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using devTools.WiXComponents.Core.Commands;
using essentialMix.Extensions;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "From missing", Order = 1)]
	public sealed class EntriesFromMissingViewModel : ViewModelCancellableCommandBase
	{
		private string _path;
		private string _targetFile;

		/// <inheritdoc />
		public EntriesFromMissingViewModel(ILogger<EntriesFromMissingViewModel> logger)
			: base(logger)
		{
			StartCommand = new RelayCommand<EntriesFromMissingViewModel>(vm => vm.GenerateEntries(), 
																		vm => !vm.IsBusy && vm.Path != null && vm.TargetFile != null);
		}

		public string Path
		{
			get => _path;
			set
			{
				if (_path == value) return;
				_path = value.ToNullIfEmpty();
				OnPropertyChanged();
			}
		}

		public string TargetFile
		{
			get => _targetFile;
			set
			{
				if (_targetFile == value) return;
				_targetFile = value.ToNullIfEmpty();
				OnPropertyChanged();
			}
		}

		/// <inheritdoc />
		public override ICommand StartCommand { get; }

		/// <inheritdoc />
		public override bool CanView() => true;

		/// <inheritdoc />
		public override void Reset() { }

		private void GenerateEntries()
		{

		}
	}
}