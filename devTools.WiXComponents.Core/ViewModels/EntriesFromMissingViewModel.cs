using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using devTools.WiXComponents.Core.Commands;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "Add missing", Order = 1)]
	public sealed class EntriesFromMissingViewModel : ViewModelCancellableCommandBase
	{
		/// <inheritdoc />
		public EntriesFromMissingViewModel(ILogger logger)
			: base(logger)
		{
			StartCommand = new RelayCommand<EntriesFromMissingViewModel>(vm => vm.GenerateEntries(), vm => !vm.IsBusy);
		}

		/// <inheritdoc />
		public override ICommand StartCommand { get; }

		/// <inheritdoc />
		public override void Reset() { }

		private void GenerateEntries()
		{

		}
	}
}