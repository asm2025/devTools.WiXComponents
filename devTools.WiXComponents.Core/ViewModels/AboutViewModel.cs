using System.ComponentModel.DataAnnotations;
using essentialMix;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc />
	[Display(Name = "About", Order = int.MaxValue)]
	public sealed class AboutViewModel : CommandViewModelBase
	{
		/// <inheritdoc />
		public AboutViewModel(ILogger<AboutViewModel> logger)
			: base(logger)
		{
			AppInfo appInfo = new AppInfo(AssemblyHelper.GetEntryAssembly());
			Title = appInfo.Title;
			Description = appInfo.Description;
			Version = appInfo.Version;
			Company = appInfo.Company;
			Copyright = appInfo.Copyright;
		}

		[NotNull]
		public string Title { get; }

		[NotNull]
		public string Description { get; }

		[NotNull]
		public string Version { get; }

		[NotNull]
		public string Company { get; }

		[NotNull]
		public string Copyright { get; }
	}
}