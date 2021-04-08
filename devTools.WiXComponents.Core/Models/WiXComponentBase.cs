using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Models
{
	[DebuggerDisplay("{Id}")]
	public abstract class WiXComponentBase
	{
		protected WiXComponentBase() 
		{
		}

		[NotNull]
		public abstract string Tag { get; }

		public string Id { get; set; }

		public void Write([NotNull] TextWriter writer)
		{
			WriteStartTag(writer);
			WriteContent(writer);
			WriteEndTag(writer);
		}

		public abstract void WriteStartTag([NotNull] TextWriter writer);

		public abstract void WriteContent([NotNull] TextWriter writer);

		public abstract void WriteEndTag([NotNull] TextWriter writer);

		protected void WriteIf([NotNull] TextWriter writer, string key, string value)
		{
			if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return;
			writer.Write($" {key}={value}" + value);
		}

		protected void WriteIf<T>([NotNull] TextWriter writer, string key, T value)
		{
			if (string.IsNullOrEmpty(key) || value is null) return;
			writer.Write($" {key}={value}" + value);
		}
	}
}