using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using essentialMix.Collections;
using essentialMix.Extensions;
using essentialMix.Helpers;
using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Services
{
	public class EntriesGeneratorService : NotifyPropertyChangedBase
	{
		[NotNull]
		private readonly HashSet<string> _entries;

		[NotNull]
		private string _rootPath;

		public EntriesGeneratorService()
		{
			_rootPath = Directory.GetCurrentDirectory();
			_entries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			Entries = new ReadOnlySet<string>(_entries);
		}

		[NotNull]
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
		public essentialMix.Collections.IReadOnlySet<string> Entries { get; }

		public void GenerateFromDirectory(string path, GenerateSettings settings = default(GenerateSettings))
		{
			path = Path.GetFullPath(path.ToNullIfEmpty() ?? ".\\");
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException();

			bool changed = false;

			if (!settings.Append)
			{
				_entries.Clear();
				changed = true;
			}
			
			IEnumerable<string> files = DirectoryHelper.EnumerateFiles(path, settings.Pattern, settings.IncludeSubDirectories
																									? SearchOption.AllDirectories
																									: SearchOption.TopDirectoryOnly);
			string exclude = settings.Exclude == null
								? null
								: RegexHelper.FromFilePattern(settings.Exclude);
			if (exclude != null && RegexHelper.AllAsterisks.IsMatch(exclude)) exclude = null;

			string rootPath = RootPath;

			if (exclude == null)
			{
				foreach (string file in files) 
					changed |= _entries.Add(Path.GetRelativePath(rootPath, file));
			}
			else
			{
				Regex rgxExclude = new Regex(exclude, RegexHelper.OPTIONS_I);

				foreach (string file in files)
				{
					if (rgxExclude.IsMatch(Path.GetFileName(file))) continue;
					changed |= _entries.Add(Path.GetRelativePath(rootPath, file));
				}
			}

			if (!changed) return;
			RaiseEntriesChanged();
		}

		public void GenerateFromMissing(string path, string fileName, GenerateSettings settings = default(GenerateSettings))
		{
		}

		public bool Add([NotNull] string fileName)
		{
			fileName = Path.GetFullPath(fileName);
			if (!_entries.Add(Path.GetRelativePath(RootPath, fileName))) return false;
			RaiseEntriesChanged();
			return true;
		}

		public bool Remove(string fileName)
		{
			fileName = Path.GetFullPath(fileName);
			if (!_entries.Remove(Path.GetRelativePath(RootPath, fileName))) return false;
			RaiseEntriesChanged();
			return true;
		}

		public bool AddRange([NotNull] IEnumerable<string> collection, string pattern = null, string exclude = null)
		{
			pattern = string.IsNullOrWhiteSpace(pattern)
						? null
						: RegexHelper.FromFilePattern(pattern);
			if (pattern != null && RegexHelper.AllAsterisks.IsMatch(pattern)) pattern = null;
			exclude = string.IsNullOrWhiteSpace(exclude)
						? null
						: RegexHelper.FromFilePattern(exclude);
			if (exclude != null && RegexHelper.AllAsterisks.IsMatch(exclude)) exclude = null;

			string rootPath = RootPath;
			Func<string, bool> _add;

			if (pattern == null && exclude == null)
			{
				_add = item => _entries.Add(Path.GetRelativePath(rootPath, item));
			}
			else if (exclude == null)
			{
				Regex rgxPattern = new Regex(pattern, RegexHelper.OPTIONS_I);
				_add = item => rgxPattern.IsMatch(Path.GetFileName(item)) && _entries.Add(Path.GetRelativePath(rootPath, item));
			}
			else if (pattern == null)
			{
				Regex rgxExclude = new Regex(exclude, RegexHelper.OPTIONS_I);
				_add = item => !rgxExclude.IsMatch(Path.GetFileName(item)) && _entries.Add(Path.GetRelativePath(rootPath, item));
			}
			else
			{
				Regex rgxPattern = new Regex(pattern, RegexHelper.OPTIONS_I);
				Regex rgxExclude = new Regex(exclude, RegexHelper.OPTIONS_I);
				_add = item =>
				{
					string fileName = Path.GetFileName(item);
					return rgxPattern.IsMatch(fileName) && !rgxExclude.IsMatch(fileName) && _entries.Add(Path.GetRelativePath(rootPath, item));
				};
			}

			bool changed = false;

			foreach (string item in collection)
				changed |= _add(item);

			if (!changed) return false;
			RaiseEntriesChanged();
			return true;
		}

		public bool RemoveRange([NotNull] IEnumerable<string> collection)
		{
			bool changed = false;
			string rootPath = RootPath;

			foreach (string item in collection)
				changed |= _entries.Remove(Path.GetRelativePath(rootPath, item));

			if (!changed) return false;
			RaiseEntriesChanged();
			return true;
		}

		public void Clear()
		{
			_entries.Clear();
			RaiseEntriesChanged();
		}

		private void RaiseEntriesChanged() { OnPropertyChanged(nameof(Entries)); }
	}
}