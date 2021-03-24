using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "Generate", Order = 0)]
	public class GenerateViewModel : ViewModelCommandBase
	{
		/// <inheritdoc />
		public GenerateViewModel(ILogger logger)
			: base(logger)
		{
		}
	}
}