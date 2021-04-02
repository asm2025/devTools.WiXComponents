using essentialMix.Extensions;

namespace devTools.WiXComponents.Core
{
	public struct GenerateSettings
	{
		private const string EXCLUDE_DEF = "JetBrains.Annotations*.dll|*.pdb";

		private string _pattern;
		private string _exclude;

		public GenerateSettings(string pattern)
			: this(pattern, true, true)
		{
		}

		public GenerateSettings(bool includeSubDirectories)
			: this(null, includeSubDirectories, true)
		{
		}

		public GenerateSettings(string pattern, bool includeSubDirectories, bool append)
			: this()
		{
			Pattern = pattern;
			Exclude = EXCLUDE_DEF;
			IncludeSubDirectories = includeSubDirectories;
			Append = append;
		}

		public string Pattern
		{
			get => _pattern; 
			set => _pattern = value.ToNullIfEmpty();
		}

		public string Exclude
		{
			get => _exclude; 
			set => _exclude = value.ToNullIfEmpty();
		}

		public bool IncludeSubDirectories { get; set; }
		public bool Append { get; set; }
	}
}