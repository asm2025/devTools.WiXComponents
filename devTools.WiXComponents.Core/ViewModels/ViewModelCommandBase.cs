using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	public abstract class ViewModelCommandBase : ViewModelBase
	{
		private string _displayName;
		private volatile int _isBusy;

		/// <inheritdoc />
		protected ViewModelCommandBase(ILogger logger)
			: base(logger)
		{
		}

		[NotNull]
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

		public int Order { get; set; }
	}
}