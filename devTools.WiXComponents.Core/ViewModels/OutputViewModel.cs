using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using devTools.WiXComponents.Core.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc cref="ViewModelCommandBase" />
	[Display(Name = "Output", Order = 3)]
	public sealed class OutputViewModel : ViewModelCommandBase, IResettableView
	{
		private readonly ComponentsGeneratorService _service;

		/// <inheritdoc />
		public OutputViewModel([NotNull] ComponentsGeneratorService service, ILogger<OutputViewModel> logger)
			: base(logger)
		{
			_service = service;
		}

		[NotNull]
		public IReadOnlyDictionary<string, IDictionary<string, ComponentInfo>> Entries => _service.Entries;

		/// <inheritdoc />
		public override bool CanView() => Entries.Count > 0;

		/// <inheritdoc />
		public void Reset() { }
	}
}