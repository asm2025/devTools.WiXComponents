using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using devTools.WiXComponents.Core.Commands;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "Add from directory", Order = 0)]
	public sealed class EntriesFromDirectoryViewModel : ViewModelCancellableCommandBase
	{
		/// <inheritdoc />
		public EntriesFromDirectoryViewModel(ILogger logger)
			: base(logger)
		{
			StartCommand = new RelayCommand<EntriesFromDirectoryViewModel>(vm => vm.GenerateEntries(), vm => !vm.IsBusy);
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