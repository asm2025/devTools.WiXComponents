using essentialMix.Patterns.NotifyChange;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	public abstract class ViewModelBase : NotifyPropertyChangedBase
	{
		protected ViewModelBase(ILogger logger)
		{
			Logger = logger;
		}

		public ILogger Logger { get; }
	}
}