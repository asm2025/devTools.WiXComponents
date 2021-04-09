using System.Diagnostics;

namespace devTools.WiXComponents.Core.Models
{
	[DebuggerDisplay("{Id}")]
	public abstract class WiXComponentBase : WiXBase
	{
		protected WiXComponentBase() 
		{
		}

		public string Id { get; set; }
	}
}