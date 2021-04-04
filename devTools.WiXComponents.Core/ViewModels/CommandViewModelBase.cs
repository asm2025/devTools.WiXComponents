using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	[DebuggerDisplay("{DisplayName}")]
	public abstract class CommandViewModelBase : ViewModelBase
	{
		private string _displayName;

		/// <inheritdoc />
		protected CommandViewModelBase(ILogger logger)
			: base(logger)
		{
			Type type = GetType();
			DisplayAttribute display = type.GetCustomAttribute<DisplayAttribute>();
			_displayName = display?.Name.ToNullIfEmpty() ?? type.Name;
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

		public virtual bool CanView() => true;
	}
}