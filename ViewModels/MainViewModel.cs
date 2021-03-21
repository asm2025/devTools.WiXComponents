using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Input;
using devTools.WiXComponents.Commands;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.ViewModels
{
	/// <inheritdoc />
	public class MainViewModel : AppViewModel
	{
		private readonly ConcurrentDictionary<Type, Func<Type, ViewModelBase>> _registeredViewModels = new ConcurrentDictionary<Type, Func<Type, ViewModelBase>>();
		private readonly ConcurrentDictionary<Type, ViewModelBase> _viewModels = new ConcurrentDictionary<Type, ViewModelBase>();

		private ViewModelBase _viewModel;

		/// <inheritdoc />
		public MainViewModel(ILogger logger)
			: base(logger)
		{
			RegisterViewModel(typeof(GenerateViewModel));
			RegisterViewModel(typeof(FindViewModel));
			RegisterViewModel(typeof(AboutViewModel));
			ChangeView = new RelayCommand<Type>(type => ViewModel = GetViewModel(type), type => IsRegisteredViewModel(type) && (ViewModel == null || !ViewModel.IsBusy));
		}

		public ViewModelBase ViewModel
		{
			get => _viewModel;
			set
			{
				if (_viewModel == value) return;
				_viewModel = value;
				OnPropertyChanged();
			}
		}

		[NotNull]
		public ICollection<Type> ViewModelsTypes => _registeredViewModels.Keys;

		[NotNull]
		public IEnumerable<ViewModelBase> ViewModels
		{
			get
			{
				foreach (Type type in _registeredViewModels.Keys)
				{
					yield return GetViewModel(type);
				}
			}
		}

		public ICommand ChangeView { get; }

		public void RegisterViewModel([NotNull] Type type)
		{
			RegisterViewModel(type, t =>
			{
				IServiceProvider services = App.Instance.ServiceProvider;
				ILogger viewModelLogger = (ILogger)services?.GetService(typeof(ILogger<>).MakeGenericType(t));
				return (ViewModelBase)t.CreateInstance(viewModelLogger);
			});
		}

		public void RegisterViewModel([NotNull] Type type, [NotNull] Func<Type, ViewModelBase> factory)
		{
			if (!typeof(ViewModelBase).IsAssignableFrom(type)) throw new InvalidCastException("Type is not a ViewModelBase.");
			if (!_registeredViewModels.TryAdd(type, factory)) return;
			OnPropertyChanged(nameof(ViewModels));
		}

		public void DeregisterViewModel([NotNull] Type type)
		{
			if (!_registeredViewModels.TryRemove(type, out _)) return;
			OnPropertyChanged(nameof(ViewModels));
		}

		public bool IsRegisteredViewModel([NotNull] Type type)
		{
			return _registeredViewModels.TryGetValue(type, out _);
		}

		public void ClearViewModels()
		{
			if (_registeredViewModels.Count == 0) return;
			_registeredViewModels.Clear();
			OnPropertyChanged(nameof(ViewModels));
		}

		public ViewModelBase GetViewModel([NotNull] Type type)
		{
			if (!typeof(ViewModelBase).IsAssignableFrom(type)) throw new InvalidCastException("Type is not a ViewModelBase.");
			if (_viewModels.TryGetValue(type, out ViewModelBase viewModel)) return viewModel;
			if (!_registeredViewModels.TryGetValue(type, out Func<Type, ViewModelBase> factory)) throw new InvalidOperationException("Type factory is not registered.");
			return _viewModels.GetOrAdd(type, factory(type));
		}
	}
}