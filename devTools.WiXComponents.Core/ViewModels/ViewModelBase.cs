using System.ComponentModel;
using System.Runtime.CompilerServices;
using essentialMix.Patterns.Object;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	public abstract class ViewModelBase : Disposable, INotifyPropertyChanged
	{
		protected ViewModelBase(ILogger logger)
		{
			Logger = logger;
		}

		public ILogger Logger { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			propertyChanged?.Invoke(this, args);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}