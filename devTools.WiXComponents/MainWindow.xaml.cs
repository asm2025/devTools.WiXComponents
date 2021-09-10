using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using devTools.WiXComponents.Core.ViewModels;
using essentialMix;
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
		public MainWindow([NotNull] App application, [NotNull] MainViewModel viewModel, ILogger<MainWindow> logger)
		{
			Application = application;
			Logger = logger;
			ViewModel = viewModel;
			DataContext = ViewModel;
			InitializeComponent();
		}

		public bool DarkTheme
		{
			get => Application.DarkTheme;
			set => Application.DarkTheme = value;
		}

		[NotNull]
		public MainViewModel ViewModel { get; }

		[NotNull]
		public App Application { get; }

		public ILogger Logger { get; }

		/// <inheritdoc />
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource hWnd = (HwndSource)PresentationSource.FromVisual(this);
			hWnd?.AddHook(HookProc);
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

		private static IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg != NativeMethods.WM_GETMINMAXINFO) return IntPtr.Zero;
			// We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
			// including the task bar.
			NativeMethods.MINMAXINFO mmi = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MINMAXINFO))!;
			
			// Adjust the maximized size and position to fit the work area of the correct monitor
			IntPtr monitor = NativeMethods.MonitorFromWindow(hWnd, NativeMethods.MONITOR_DEFAULTTONEAREST);

			if (monitor != IntPtr.Zero)
			{
				NativeMethods.MONITORINFO monitorInfo = new NativeMethods.MONITORINFO();
				monitorInfo.cbSize = Marshal.SizeOf(typeof(NativeMethods.MONITORINFO));
				NativeMethods.GetMonitorInfo(monitor, ref monitorInfo);
				RECT rcWorkArea = monitorInfo.rcWork;
				RECT rcMonitorArea = monitorInfo.rcMonitor;
				mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
				mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
				mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
				mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
			}

			Marshal.StructureToPtr(mmi, lParam, true);
			return IntPtr.Zero;
		}
	}
}
