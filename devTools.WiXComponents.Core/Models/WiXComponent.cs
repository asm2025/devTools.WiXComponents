using System.Collections.Generic;

namespace devTools.WiXComponents.Core.Models
{
	public class WiXComponent : WiXComponentBase
	{
		public WiXComponent()
			: this(null)
		{
		}

		public WiXComponent(string fileName)
		{
			FileName = fileName;
			Id = fileName?.ToUpperInvariant();
		}

		/// <inheritdoc />
		public sealed override string Tag { get; } = "Component";

		public string Directory { get; set; }

		???
		public string Source { get; set; }

		public string FileName { get; set; }

		public ISet<string> Files { get; set; }
	}
}
