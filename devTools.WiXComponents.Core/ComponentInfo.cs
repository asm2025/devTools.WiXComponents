using System.Collections.Generic;

namespace devTools.WiXComponents.Core
{
	public class ComponentInfo
	{
		public ComponentInfo()
			: this(null)
		{
		}

		public ComponentInfo(string fileName)
		{
			FileName = fileName;
		}

		public string Prefix { get; set; }

		public string FileName { get; set; }

		public IList<string> GroupFileNames { get; set; }
	}
}
