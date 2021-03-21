using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.ViewModels
{
	/// <inheritdoc />
	public class AboutViewModel : AppViewModel
	{
		/// <inheritdoc />
		public AboutViewModel(ILogger logger)
			: base("About", logger)
		{
		}
	}
}