using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			string excludeDirectories = Settings.ExcludeDirectories == null
											? null
											: RegexHelper.FromFilePattern(Settings.ExcludeDirectories);
			if (excludeDirectories != null && RegexHelper.AllAsterisks.IsMatch(excludeDirectories)) excludeDirectories = null;

			string excludeFiles = Settings.ExcludeFiles == null
									? null
									: RegexHelper.FromFilePattern(Settings.ExcludeFiles);
			if (excludeFiles != null && RegexHelper.AllAsterisks.IsMatch(excludeFiles)) excludeFiles = null;

			SearchOption option = Settings.IncludeSubdirectories
									? SearchOption.AllDirectories
									: SearchOption.TopDirectoryOnly;
			IEnumerable<string> files;

			if (excludeDirectories != null)
			{
				Regex rgxExcludeDir = new Regex(excludeDirectories, RegexHelper.OPTIONS_I);
				files = DirectoryHelper.EnumerateFiles(path, Settings.Pattern, option, dir =>
				{
					// use Path.GetFileName for directory too.
					// the weird naming of Path.GetDirectoryName is misleading
					string d = Path.GetFileName(dir);
					return !rgxExcludeDir.IsMatch(d!);
				});
			}
			else
			{
				files = DirectoryHelper.EnumerateFiles(path, Settings.Pattern, option);
			}

			if (excludeFiles != null)
			{
				Regex rgxExcludeFiles = new Regex(excludeFiles, RegexHelper.OPTIONS_I);
				files = files.Where(file =>
				{
					string f = Path.GetFileName(file);
					return !rgxExcludeFiles.IsMatch(f);
				});
			}

			string rootPath = Settings.RootPath;
			if (rootPath != null) files = files.Select(file => Path.GetRelativePath(rootPath, file));

			foreach (string file in files) 
				Entries.Add(file);
		}

		public void GenerateFromMissing(string path, string fileName)
		{
		}

		public bool Add([NotNull] string fileName)
		{
			fileName = Path.GetFullPath(fileName);
			if (Settings.RootPath != null) fileName = Path.GetRelativePath(Settings.RootPath, fileName);
			return Entries.Add(fileName);
		}

		public bool Remove(string fileName)
		{
			return Entries.Remove(fileName);
		}

		public void AddRange([NotNull] IEnumerable<string> collection, string pattern = null, string exclude = null)
		{
			pattern = string.IsNullOrWhiteSpace(pattern)
						? null
						: RegexHelper.FromFilePattern(pattern);
			if (pattern != null && RegexHelper.AllAsterisks.IsMatch(pattern)) pattern = null;
			exclude = string.IsNullOrWhiteSpace(exclude)
						? null
						: RegexHelper.FromFilePattern(exclude);
			if (exclude != null && RegexHelper.AllAsterisks.IsMatch(exclude)) exclude = null;

			if (pattern != null)
			{
				Regex rgxPattern = new Regex(pattern, RegexHelper.OPTIONS_I);
				collection = collection.Where(e => rgxPattern.IsMatch(Path.GetFileName(e)));
			}

			if (exclude != null)
			{
				Regex rgxExclude = new Regex(exclude, RegexHelper.OPTIONS_I);
				collection = collection.Where(e => !rgxExclude.IsMatch(Path.GetFileName(e)));
			}

			string rootPath = Settings.RootPath;
			if (rootPath != null) collection = collection.Select(e => Path.GetRelativePath(rootPath, e));

			foreach (string item in collection) 
				Entries.Add(item);
		}

		public void RemoveRange([NotNull] IEnumerable<string> collection)
		{
			foreach (string item in collection)
				Entries.Remove(item);
		}

		public void Clear()
		{
			Entries.Clear();
		}
	}
}