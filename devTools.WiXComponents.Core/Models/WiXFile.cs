using System;
using System.Collections.Generic;
using System.IO;

namespace devTools.WiXComponents.Core.Models;

public class WiXFile : WiXBase
{
	private ISet<string> _files;

	public WiXFile() 
	{
	}

	/// <inheritdoc />
	public override string Tag { get; } = "File";

	public string Id { get; set; }

	public string Name { get; set; }

	public string Source { get; set; }

	public bool? Vital { get; set; }

	public ISet<string> Files => _files ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	public bool HasFiles => _files != null && _files.Count > 0;

	/// <inheritdoc />
	public override void WriteStartTag(TextWriter writer)
	{
		writer.Write("<" + Tag);
		WriteIf(writer, nameof(Id), Id);
		WriteIf(writer, nameof(Name), Name);
		WriteIf(writer, nameof(Source), Source);
		WriteIf(writer, nameof(Vital), Vital);
		writer.Write(">");
	}

	/// <inheritdoc />
	public override void WriteContent(TextWriter writer)
	{
		if (!HasFiles) return;
	}

	/// <inheritdoc />
	public override void WriteEndTag(TextWriter writer)
	{
		writer.Write("</" + Tag + ">");
	}
}