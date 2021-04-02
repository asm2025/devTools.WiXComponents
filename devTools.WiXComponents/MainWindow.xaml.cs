using System;
using System.Windows;
using System.Windows.Input;
using devTools.WiXComponents.Core.ViewModels;
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
		public MainWindow([NotNull] MainViewModel viewModel, ILogger<MainWindow> logger)
		{
			Logger = logger;
			ViewModel = viewModel;
			DataContext = ViewModel;
			InitializeComponent();
		}

		[NotNull]
		public MainViewModel ViewModel { get; }

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
