using System;
using System.IO;
using essentialMix.Extensions;
using essentialMix.Helpers;
using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Core.Models
{
	public sealed class GenerateSettings : NotifyPropertyChangedBase, ICloneable
	{
		private const string EXCLUDE_DEF = "JetBrains.Annotations.*|*.pdb";

		private string _rootPath;
		private string _pattern;
		private string _exclude;
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
			_exclude = EXCLUDE_DEF;
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

		public string Exclude
		{
			get => _exclude;
			set
			{
				if (_exclude == value) return;
				_exclude = value.ToNullIfEmpty();
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
				Exclude = Exclude
			};
		}
	}
}