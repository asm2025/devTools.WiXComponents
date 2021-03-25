using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc cref="ViewModelCommandBase" />
	[Display(Name = "Add from list", Order = 2)]
	public sealed class EntriesFromListViewModel : ViewModelCommandBase, IResettableView
	{
		/// <inheritdoc />
		public EntriesFromListViewModel(ILogger logger)
			: base(logger)
		{
		}

		/// <inheritdoc />
		public void Reset() { }
	}
}