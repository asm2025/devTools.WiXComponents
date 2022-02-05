using System;
using System.IO;
using devTools.WiXComponents.Core.Models;
using essentialMix.Collections;
using essentialMix.Extensions;
using essentialMix.Helpers;
using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Services;

public class ComponentsService : NotifyPropertyChangedBase
{
	private string _rootPath;

	public ComponentsService() 
	{
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

	[NotNull]
	public KeyedDictionary<string, WiXComponentBase> Entries { get; } = new KeyedDictionary<string, WiXComponentBase>(e => e.Id, StringComparer.OrdinalIgnoreCase);
}