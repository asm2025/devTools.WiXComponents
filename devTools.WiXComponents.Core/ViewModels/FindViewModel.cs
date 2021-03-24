using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "Find", Order = 1)]
	public class FindViewModel : ViewModelCommandBase
	{
		/// <inheritdoc />
		public FindViewModel(ILogger logger)
			: base(logger)
		{
		}
	}
}