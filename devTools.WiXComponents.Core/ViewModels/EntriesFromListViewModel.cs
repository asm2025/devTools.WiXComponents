using System.ComponentModel.DataAnnotations;
using essentialMix.Collections;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc cref="ViewModelCommandBase" />
	[Display(Name = "From list", Order = 2)]
	public sealed class EntriesFromListViewModel : ViewModelCommandBase, IResettableView
	{
		/// <inheritdoc />
		public EntriesFromListViewModel(ILogger<EntriesFromListViewModel> logger)
			: base(logger)
		{
		}

		[NotNull]
		public ObservableList<string> Entries { get; set; } = new ObservableList<string>();

		/// <inheritdoc />
		public override bool CanView() => true;

		/// <inheritdoc />
		public void Reset() { }
	}
}