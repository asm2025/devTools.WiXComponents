using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	/// <inheritdoc cref="ViewModelCommandBase" />
	[Display(Name = "Output", Order = 3)]
	public sealed class OutputViewModel : ViewModelCommandBase, IResettableView
	{
		/// <inheritdoc />
		public OutputViewModel(ILogger logger)
			: base(logger)
		{
		}

		[NotNull]
		public ObservableCollection<string> Directories { get; } = new ObservableCollection<string>();

		[NotNull]
		public ObservableDictionary<string, > Directories { get; } = new ObservableCollection<string>();

		/// <inheritdoc />
		public void Reset() { }

		/// <inheritdoc />
		public override bool CanView() { return Directories.Count > 0; }
	}
}