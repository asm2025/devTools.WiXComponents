using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using devTools.WiXComponents.Core.ViewModels;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <inheritdoc />
		public MainWindow([NotNull] IServiceProvider services)
		{
			Services = services;
			Logger = (ILogger)services.GetService(typeof(ILogger<MainWindow>));
			ViewModel = new MainViewModel((ILogger)services.GetService(typeof(ILogger<MainViewModel>)));
			DataContext = ViewModel;
			InitializeComponent();
		}

		[NotNull]
		public IServiceProvider Services { get; }

		[NotNull]
		public MainViewModel ViewModel { get; }

		public ILogger Logger { get; }

		/// <inheritdoc />
		protected override void OnSourceInitialized(EventArgs e)
		{
			Assembly asm = typeof(ViewModelCommandBase).Assembly;
			ObservableCollection<ViewModelCommandBase> viewModels = ViewModel.ViewModels;
			IEnumerable<(Type e, DisplayAttribute)> viewModelTypes = asm.GetTypes()
																		.Where(type => !type.IsAbstract && typeof(ViewModelCommandBase).IsAssignableFrom(type))
																		.Select(type => (type, type.GetCustomAttribute<DisplayAttribute>()))
																		.OrderBy(tuple => tuple.Item2?.Order ?? short.MaxValue);

			foreach ((Type type, DisplayAttribute displayAttribute) in viewModelTypes)
			{
				Type loggerType = typeof(ILogger<>).MakeGenericType(type);
				ILogger viewModelLogger = (ILogger)Services.GetService(loggerType);
				ViewModelCommandBase viewModel = (ViewModelCommandBase)type.CreateInstance(viewModelLogger);
				viewModel.Order = viewModels.Count;
				if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name)) viewModel.DisplayName = displayAttribute.Name;
				viewModels.Add(viewModel);
			}

			ViewModel.ChangeView.Execute(ViewModel.ViewModels[0]);
			base.OnSourceInitialized(e);
		}

		/// <inheritdoc />
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Dispatcher.InvokeShutdown();
		}

		/// <inheritdoc />
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			DragMove();
			base.OnMouseLeftButtonDown(e);
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
