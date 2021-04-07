using System.IO;

namespace devTools.WiXComponents.Core.Models
{
	public class WiXComponentGroup : WiXComponentBase
	{
		/// <inheritdoc />
		public WiXComponentGroup() 
		{
		}

		/// <inheritdoc />
		public sealed override string Tag { get; } = "ComponentGroup";

		public string Directory { get; set; }
		public string Source { get; set; }

		/// <inheritdoc />
		public override void WriteStartTag(TextWriter writer)
		{
			writer.Write("<" + Tag);
			WriteIfNotEmpty(writer, nameof(Id), Id);
			WriteIfNotEmpty(writer, nameof(Directory), Directory);
			WriteIfNotEmpty(writer, nameof(Source), Source);
			writer.Write(">");
		}

		/// <inheritdoc />
		public override void WriteContent(TextWriter writer)
		{
			// todo
			???
		}

		/// <inheritdoc />
		public override void WriteEndTag(TextWriter writer)
		{
			writer.Write("</" + Tag + ">");
		}
	}
}