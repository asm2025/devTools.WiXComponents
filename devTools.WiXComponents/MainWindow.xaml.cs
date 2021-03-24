using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using devTools.WiXComponents.Core.Commands;
using devTools.WiXComponents.Core.ViewModels;
using essentialMix.Collections;
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
			Logger = (ILogger)services.GetService(typeof(ILogger<MainWindow>));

			Assembly asm = typeof(ViewModelCommandBase).Assembly;
			List<ViewModelCommandBase> viewModels = new List<ViewModelCommandBase>();
			IEnumerable<(Type e, DisplayAttribute)> viewModelTypes = asm.GetTypes()
																		.Where(type => !type.IsAbstract && typeof(ViewModelCommandBase).IsAssignableFrom(type))
																		.Select(type => (type, type.GetCustomAttribute<DisplayAttribute>()))
																		.OrderBy(tuple => tuple.Item2?.Order ?? short.MaxValue);

			foreach ((Type type, DisplayAttribute displayAttribute) in viewModelTypes)
			{
				Type loggerType = typeof(ILogger<>).MakeGenericType(type);
				ILogger viewModelLogger = (ILogger)services.GetService(loggerType);
				ViewModelCommandBase viewModel = (ViewModelCommandBase)type.CreateInstance(viewModelLogger);
				viewModel.Order = viewModels.Count;
				if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name)) viewModel.DisplayName = displayAttribute.Name;
				viewModels.Add(viewModel);
			}

			MainViewModel mainViewModel = new MainViewModel(new ReadOnlyList<ViewModelCommandBase>(viewModels), (ILogger)services.GetService(typeof(ILogger<MainViewModel>)));
			ChangeView = new RelayCommand<ViewModelCommandBase>(vm => mainViewModel.SelectedViewModel = vm,
																vm => vm != mainViewModel.SelectedViewModel &&
																	(mainViewModel.SelectedViewModel == null || !mainViewModel.SelectedViewModel.IsBusy));
			ChangeView.Execute(mainViewModel.ViewModels[0]);
			DataContext = mainViewModel;
			InitializeComponent();
		}

		[NotNull]
		public ICommand ChangeView { get; }

		public ILogger Logger { get; }

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
