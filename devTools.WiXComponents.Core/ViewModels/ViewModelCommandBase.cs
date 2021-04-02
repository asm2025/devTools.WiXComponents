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
	public abstract class ViewModelCommandBase : ViewModelBase
	{
		private string _displayName;

		/// <inheritdoc />
		protected ViewModelCommandBase(ILogger logger)
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

		public abstract bool CanView();
	}
}