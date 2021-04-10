using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using devTools.WiXComponents.Core.Models;
using essentialMix.Collections;
using essentialMix.Helpers;
using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Services
{
	public class GeneratorService : NotifyPropertyChangedBase
	{
		public GeneratorService()
		{
			Settings = new GenerateSettings();
			Entries = new ObservableHashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		[NotNull]
		public GenerateSettings Settings { get; }

		[NotNull]
		public ObservableHashSet<string> Entries { get; }

		public void GenerateUsingHeat(string heatPath, string path)
		{
		}

		public void GenerateFromDirectory(string path)
		{
			path = Path.GetFullPath(PathHelper.Trim(path) ?? ".\\");
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException();
			if (!Settings.Append) Entries.Clear();

			IEnumerable<string> files = DirectoryHelper.EnumerateFiles(path, Settings.Pattern, Settings.IncludeSubdirectories
																									? SearchOption.AllDirectories
																									: SearchOption.TopDirectoryOnly);
			string exclude = Settings.Exclude == null
								? null
								: RegexHelper.FromFilePattern(Settings.Exclude);
			if (exclude != null && RegexHelper.AllAsterisks.IsMatch(exclude)) exclude = null;

			string rootPath = Settings.RootPath;

			if (exclude == null)
			{
				if (rootPath == null)
				{
					foreach (string file in files) 
						Entries.Add(file);
				}
				else
				{
					foreach (string file in files) 
						Entries.Add(Path.GetRelativePath(rootPath, file));
				}
			}
			else
			{
				Regex rgxExclude = new Regex(exclude, RegexHelper.OPTIONS_I);

				if (rootPath == null)
				{
					foreach (string file in files)
					{
						if (rgxExclude.IsMatch(Path.GetFileName(file))) continue;
						Entries.Add(file);
					}
				}
				else
				{
					foreach (string file in files)
					{
						if (rgxExclude.IsMatch(Path.GetFileName(file))) continue;
						Entries.Add(Path.GetRelativePath(rootPath, file));
					}
				}
			}
		}

		public void GenerateFromMissing(string path, string fileName)
		{
		}

		public bool Add([NotNull] string fileName)
		{
			fileName = Path.GetFullPath(fileName);
			return Entries.Add(Path.GetRelativePath(Settings.RootPath, fileName));
		}

		public bool Remove(string fileName)
		{
			fileName = Path.GetFullPath(fileName);
			return Entries.Remove(Path.GetRelativePath(Settings.RootPath, fileName));
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

			string rootPath = Settings.RootPath;
			Func<string, bool> _add;

			if (pattern == null && exclude == null)
			{
				_add = item => Entries.Add(Path.GetRelativePath(rootPath, item));
			}
			else if (exclude == null)
			{
				Regex rgxPattern = new Regex(pattern, RegexHelper.OPTIONS_I);
				_add = item => rgxPattern.IsMatch(Path.GetFileName(item)) && Entries.Add(Path.GetRelativePath(rootPath, item));
			}
			else if (pattern == null)
			{
				Regex rgxExclude = new Regex(exclude, RegexHelper.OPTIONS_I);
				_add = item => !rgxExclude.IsMatch(Path.GetFileName(item)) && Entries.Add(Path.GetRelativePath(rootPath, item));
			}
			else
			{
				Regex rgxPattern = new Regex(pattern, RegexHelper.OPTIONS_I);
				Regex rgxExclude = new Regex(exclude, RegexHelper.OPTIONS_I);
				_add = item =>
				{
					string fileName = Path.GetFileName(item);
					return rgxPattern.IsMatch(fileName) && !rgxExclude.IsMatch(fileName) && Entries.Add(Path.GetRelativePath(rootPath, item));
				};
			}

			foreach (string item in collection)
				_add(item);

			return true;
		}

		public bool RemoveRange([NotNull] IEnumerable<string> collection)
		{
			string rootPath = Settings.RootPath;

			foreach (string item in collection)
				Entries.Remove(Path.GetRelativePath(rootPath, item));

			return true;
		}

		public void Clear()
		{
			Entries.Clear();
		}
	}
}