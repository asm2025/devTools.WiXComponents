using System.Threading;
using essentialMix.Extensions;
using essentialMix.Patterns.NotifyChange;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.ViewModels
{
	public abstract class ViewModelBase : NotifyPropertyChangedBase
	{
		private string _displayName;
		private volatile int _isBusy;

		protected ViewModelBase(ILogger logger)
			: this(null, logger)
		{
		}

		protected ViewModelBase(string displayName, ILogger logger)
		{
			DisplayName = displayName.ToNullIfEmpty();
			Logger = logger;
		}

		public string DisplayName
		{
			get => _displayName ??= GetType().Name;
			set => _displayName = value;
		}

		public bool IsBusy
		{
			get => _isBusy != 0;
			protected set
			{
				Interlocked.CompareExchange(ref _isBusy, value
															? 1
															: 0, _isBusy);
				OnPropertyChanged();
			}
		}

		public ILogger Logger { get; }
	}
}