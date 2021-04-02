using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Ookii.Dialogs.Wpf;

namespace devTools.WiXComponents.Controls
{
	/// <summary>
	/// Interaction logic for SelectFolder.xaml
	/// </summary>
	public partial class SelectFolder : UserControl
	{
		public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(SelectFolder), new FrameworkPropertyMetadata("Select folder...", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty RootFolderProperty = DependencyProperty.Register(nameof(RootFolder), typeof(Environment.SpecialFolder), typeof(SelectFolder), new FrameworkPropertyMetadata(Environment.SpecialFolder.Desktop, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty SelectedPathProperty = DependencyProperty.Register(nameof(SelectedPath), typeof(string), typeof(SelectFolder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public SelectFolder()
		{
			InitializeComponent();
		}

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		public Environment.SpecialFolder RootFolder
		{
			get => (Environment.SpecialFolder)GetValue(RootFolderProperty);
			set => SetValue(RootFolderProperty, value);
		}

		public string SelectedPath
		{
			get => (string)GetValue(SelectedPathProperty);
			set => SetValue(SelectedPathProperty, value);
		}

		private void OnBrowseClick(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog
			{
				Description = Title,
				UseDescriptionForTitle = true,
				RootFolder = RootFolder,
				SelectedPath = SelectedPath,
				ShowNewFolderButton = true
			};

			HwndSource hWnd = (HwndSource)PresentationSource.FromVisual(this);
			IntPtr handle = hWnd?.Handle ?? IntPtr.Zero;
			if (dlg.ShowDialog(handle) != true) return;
			SelectedPath = dlg.SelectedPath;
		}
	}
}
