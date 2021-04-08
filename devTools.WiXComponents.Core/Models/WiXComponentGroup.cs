using System;
using System.IO;
using essentialMix.Collections;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Models
{
	public class WiXComponentGroup : WiXComponentBase
	{
		private KeyedDictionary<string, WiXComponentBase> _components;

		/// <inheritdoc />
		public WiXComponentGroup() 
		{
		}

		/// <inheritdoc />
		public sealed override string Tag { get; } = "ComponentGroup";

		public string Directory { get; set; }
		public string Source { get; set; }

		public bool HasComponents => _components != null && _components.Count > 0;

		[NotNull]
		public KeyedDictionary<string, WiXComponentBase> Components => _components ??= new KeyedDictionary<string, WiXComponentBase>(e => e.Id, StringComparer.OrdinalIgnoreCase);

		/// <inheritdoc />
		public override void WriteStartTag(TextWriter writer)
		{
			writer.Write("<" + Tag);
			WriteIf(writer, nameof(Id), Id);
			WriteIf(writer, nameof(Directory), Directory);
			WriteIf(writer, nameof(Source), Source);
			writer.Write(">");
		}

		/// <inheritdoc />
		public override void WriteContent(TextWriter writer)
		{
			if (!HasComponents) return;

			foreach (WiXComponentBase component in _components) 
				component.Write(writer);
		}

		/// <inheritdoc />
		public override void WriteEndTag(TextWriter writer)
		{
			writer.Write("</" + Tag + ">");
		}
	}
}