using System;
using System.IO;
using essentialMix.Extensions;
using essentialMix.Helpers;
using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Models;

public sealed class GenerateSettings : NotifyPropertyChangedBase, ICloneable
{
	private const string EXCLUDE_DIR_DEF = "obj|bin|debug|release";
	private const string EXCLUDE_FILE_DEF = "JetBrains.Annotations.*|*.pdb|*.user";

	private string _rootPath;
	private string _pattern;
	private string _excludeDirectories;
	private string _excludeFiles;
	private bool _includeSubdirectories;
	private bool _append;

	public GenerateSettings()
		: this(null, true, true)
	{
	}

	public GenerateSettings(string pattern)
		: this(pattern, true, true)
	{
	}

	public GenerateSettings(bool includeSubDirectories)
		: this(null, includeSubDirectories, true)
	{
	}

	public GenerateSettings(string pattern, bool includeSubDirectories, bool append)
	{
		_excludeDirectories = EXCLUDE_DIR_DEF;
		_excludeFiles = EXCLUDE_FILE_DEF;
		Pattern = pattern;
		IncludeSubdirectories = includeSubDirectories;
		Append = append;
	}

	public string RootPath
	{
		get => _rootPath;
		set
		{
			if (_rootPath == value) return;
			string path = PathHelper.Trim(value);
			if (path != null) path = Path.GetFullPath(path).Suffix(Path.DirectorySeparatorChar);
			if (_rootPath == path) return;
			_rootPath = path;
			OnPropertyChanged();
		}
	}

	public string Pattern
	{
		get => _pattern;
		set
		{
			if (_pattern == value) return;
			_pattern = value.ToNullIfEmpty();
			OnPropertyChanged();
		}
	}

	public string ExcludeDirectories
	{
		get => _excludeDirectories;
		set
		{
			if (_excludeDirectories == value) return;
			_excludeDirectories = value.ToNullIfEmpty();
			OnPropertyChanged();
		}
	}

	public string ExcludeFiles
	{
		get => _excludeFiles;
		set
		{
			if (_excludeFiles == value) return;
			_excludeFiles = value.ToNullIfEmpty();
			OnPropertyChanged();
		}
	}

	public bool IncludeSubdirectories
	{
		get => _includeSubdirectories;
		set
		{
			if (_includeSubdirectories == value) return;
			_includeSubdirectories = value;
			OnPropertyChanged();
		}
	}

	public bool Append
	{
		get => _append;
		set
		{
			if (_append == value) return;
			_append = value;
			OnPropertyChanged();
		}
	}

	/// <inheritdoc />
	object ICloneable.Clone() { return Clone(); }
	[NotNull]
	public GenerateSettings Clone()
	{
		return new GenerateSettings(Pattern, IncludeSubdirectories, Append)
		{
			RootPath = RootPath,
			ExcludeFiles = ExcludeFiles
		};
	}
}