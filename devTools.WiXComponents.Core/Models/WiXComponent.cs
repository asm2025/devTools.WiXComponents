﻿using System;
using System.IO;
using essentialMix.Extensions;

namespace devTools.WiXComponents.Core.Models;

public class WiXComponent : WiXComponentBase
{
	private int _diskId = 1;

	public WiXComponent()
	{
	}

	/// <inheritdoc />
	public sealed override string Tag { get; } = "Component";

	public string Directory { get; set; }

	public int DiskId
	{
		get => _diskId;
		set => _diskId = value.NotBelow(1);
	}

	public Guid Guid { get; set; } = Guid.NewGuid();

	public bool KeyPath { get; set; }

	public ComponentLocation? Location { get; set; }

	public bool NeverOverwrite { get; set; }

	public bool Permanent { get; set; }

	public bool Shared { get; set; }

	public bool SharedDllRefCount { get; set; }

	public bool Transitive { get; set; }

	public bool UninstallWhenSuperseded { get; set; }

	public bool Win64 { get; set; }

	public WiXFile File { get; set; }

	/// <inheritdoc />
	public override void WriteStartTag(TextWriter writer)
	{
		writer.Write("<" + Tag);
		WriteIf(writer, nameof(Id), Id);
		WriteIf(writer, nameof(Directory), Directory);
		WriteIf(writer, nameof(DiskId), DiskId);
		WriteIf(writer, nameof(Guid), Guid);
		WriteIf(writer, nameof(KeyPath), KeyPath);
		writer.Write(">");
	}

	/// <inheritdoc />
	public override void WriteContent(TextWriter writer)
	{
		/*
	private const string COMPONENT_BEGIN = "<Component Id=\"{0}{1}\" DiskId=\"1\" Win64=\"$(var.Win64)\" Guid=\"{2}\">";
	private const string COMPONENT_END = "</Component>";
	private const string ADD_KEY_FILE = "<File Id=\"{0}{1}\" Name=\"{2}\" Source=\"$(var.SourcePath){3}\" KeyPath=\"yes\" />";
	private const string ADD_FILE = "<File Id=\"{0}{1}\" Name=\"{2}\" Source=\"$(var.SourcePath){2}\" />";
		*/
		// todo
	}

	/// <inheritdoc />
	public override void WriteEndTag(TextWriter writer)
	{
		writer.Write("</" + Tag + ">");
	}
}