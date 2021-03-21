using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace devTools.WiXComponents.Commands
{
	public class RelayCommand<T> : ICommand
	{
		[NotNull]
		private readonly Action<T> _execute;
		private readonly Predicate<T> _canExecute;

		public RelayCommand([NotNull] Action<T> execute)
			: this(execute, null)
		{
		}

		public RelayCommand([NotNull] Action<T> execute, Predicate<T> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		/// <inheritdoc />
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || parameter is T tp && _canExecute(tp);
		}

		/// <inheritdoc />
		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}
	}
}
