﻿using System;
using System.Windows;
using System.Windows.Input;
using devTools.WiXComponents.ViewModels;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <inheritdoc />
		public MainWindow(ILogger<MainWindow> logger)
		{
			DataContext = new MainViewModel(logger);
			InitializeComponent();
			Logger = logger;
		}

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

		private void CommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
	}
}
