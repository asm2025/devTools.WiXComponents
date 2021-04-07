using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using devTools.WiXComponents.Core.Models;
using essentialMix.Collections;
using essentialMix.Extensions;
using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Services
{
	public class ComponentsService : NotifyPropertyChangedBase
	{
		private const string DEF_DIR = "INSTALLDIR";

		//{0} = Uppercase file name, {1} = file name, {2} = GUID, {3} = add line
		private const string GROUP_BEGIN = "<ComponentGroup Id=\"{0}Components\" Directory=\"{1}\">";
		private const string GROUP_END = "</ComponentGroup>";
		private const string COMPONENT_BEGIN = "<Component Id=\"{0}{1}\" DiskId=\"1\" Win64=\"$(var.Win64)\" Guid=\"{2}\">";
		private const string COMPONENT_END = "</Component>";
		private const string ADD_KEY_FILE = "<File Id=\"{0}{1}\" Name=\"{2}\" Source=\"$(var.SourcePath){3}\" KeyPath=\"yes\" />";
		private const string ADD_FILE = "<File Id=\"{0}{1}\" Name=\"{2}\" Source=\"$(var.SourcePath){2}\" />";

		[NotNull]
		private readonly IDictionary<string, KeyedDictionary<string, WiXComponent>> _entries;

		[NotNull]
		private string _rootPath;

		public ComponentsService() 
		{
			_entries = new Dictionary<string, KeyedDictionary<string, WiXComponent>>(StringComparer.OrdinalIgnoreCase);
			Entries = new ReadOnlyDictionary<string, KeyedDictionary<string, WiXComponent>>(_entries);
		}

		public string RootPath
		{
			get => _rootPath;
			set
			{
				string path = Path.GetFullPath(value.ToNullIfEmpty() ?? ".\\").Suffix(Path.DirectorySeparatorChar);
				if (path.Equals(_rootPath, StringComparison.OrdinalIgnoreCase)) return;
				_rootPath = value;
				OnPropertyChanged();
			}
		}

		[NotNull]
		public IReadOnlyDictionary<string, KeyedDictionary<string, WiXComponent>> Entries { get; }
	}
}
