using Microsoft.Extensions.Logging;

namespace WiXComponents.ViewModels
{
	/// <inheritdoc />
	public class AboutViewModel : AppViewModel
	{
		/// <inheritdoc />
		public AboutViewModel(ILogger logger)
			: base(logger)
		{
		}
	}
}