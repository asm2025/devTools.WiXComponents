﻿using System.Reflection;
using essentialMix.Extensions;
using essentialMix.Helpers;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.ViewModels
{
	/// <inheritdoc />
	public abstract class AppViewModel : ViewModelBase
	{
		private static readonly string __title;
		private static readonly string __company;
		private static readonly string __version;

		static AppViewModel()
		{
			Assembly essentialMix = AssemblyHelper.GetEntryAssembly();
			__title = essentialMix.GetTitle();
			__company = essentialMix.GetCompany();
			__version = essentialMix.GetFileVersion();
		}

		/// <inheritdoc />
		protected AppViewModel(ILogger logger)
			: this(null, logger)
		{
		}

		/// <inheritdoc />
		protected AppViewModel(string displayName, ILogger logger)
			: base(displayName, logger)
		{
		}

		public string Title => __title;

		public string Company => __company;

		public string Version => __version;
	}
}