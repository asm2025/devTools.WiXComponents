using System;
using System.IO;
using essentialMix.Extensions;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Models
{
	public abstract class WiXBase
	{
		protected WiXBase() 
		{
		}

		[NotNull]
		public abstract string Tag { get; }

		public void Write([NotNull] TextWriter writer)
		{
			WriteStartTag(writer);
			WriteContent(writer);
			WriteEndTag(writer);
		}

		public abstract void WriteStartTag([NotNull] TextWriter writer);

		public abstract void WriteContent([NotNull] TextWriter writer);

		public abstract void WriteEndTag([NotNull] TextWriter writer);

		protected void WriteIf<T>([NotNull] TextWriter writer, string key, T value)
		{
			if (string.IsNullOrEmpty(key)) return;

			string valueString = value switch
			{
				null => null,
				string s => s,
				Enum => typeof(T).GetDisplayName(value),
				bool bValue => bValue.ToYesNo(),
				_ => Convert.ToString(value)
			};

			if (string.IsNullOrEmpty(valueString)) return;
			writer.Write($" {key}=\"{valueString}\"");
		}
	}
}