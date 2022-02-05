using System.ComponentModel.DataAnnotations;
using devTools.WiXComponents.Core.Models;
using devTools.WiXComponents.Core.Services;
using essentialMix.Collections;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels;

/// <inheritdoc cref="CommandViewModelBase" />
[Display(Name = "Output", Order = 2)]
public sealed class OutputViewModel : CommandViewModelBase, IResettableView
{
	private readonly ComponentsService _service;

	/// <inheritdoc />
	public OutputViewModel([NotNull] ComponentsService service, ILogger<OutputViewModel> logger)
		: base(logger)
	{
		_service = service;
	}

	[NotNull]
	public KeyedDictionary<string, WiXComponentBase> Entries => _service.Entries;

	/// <inheritdoc />
	public override bool CanView() => Entries.Count > 0;

	/// <inheritdoc />
	public void Reset() { }
}