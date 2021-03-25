using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	public abstract class ViewModelCommandBase : ViewModelBase
	{
		private string _displayName;

		/// <inheritdoc />
		protected ViewModelCommandBase(ILogger logger)
			: base(logger)
		{
			_displayName = GetType().Name;
		}

		[NotNull]
		public string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName == value) return;
				_displayName = value.ToNullIfEmpty() ?? GetType().Name;
				OnPropertyChanged();
			}
		}

		public abstract bool CanView();
	}
}