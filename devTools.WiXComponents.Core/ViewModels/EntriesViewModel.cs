using System.ComponentModel.DataAnnotations;
using essentialMix.Collections;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels;

/// <inheritdoc cref="CommandViewModelBase" />
[Display(Name = "List", Order = 1)]
public sealed class EntriesViewModel : CommandViewModelBase, IResettableView
{
	/// <inheritdoc />
	public EntriesViewModel(ILogger<EntriesViewModel> logger)
		: base(logger)
	{
	}

	[NotNull]
	public ObservableList<string> Entries { get; set; } = new ObservableList<string>();

	/// <inheritdoc />
	public void Reset() { }
}