using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Ookii.Dialogs.Wpf;

namespace devTools.WiXComponents.Controls
{
	/// <summary>
	/// Interaction logic for OpenFile.xaml
	/// </summary>
	public partial class OpenFile : UserControl
	{
		public static DependencyProperty AddExtensionProperty = DependencyProperty.Register(nameof(AddExtension), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty CheckFileExistsProperty = DependencyProperty.Register(nameof(CheckFileExists), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty CheckPathExistsProperty = DependencyProperty.Register(nameof(CheckPathExists), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty DefaultExtProperty = DependencyProperty.Register(nameof(DefaultExt), typeof(string), typeof(OpenFile), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty DereferenceLinksProperty = DependencyProperty.Register(nameof(DereferenceLinks), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(OpenFile), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(string), typeof(OpenFile), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty FilterIndexProperty = DependencyProperty.Register(nameof(FilterIndex), typeof(int), typeof(OpenFile), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(nameof(InitialDirectory), typeof(string), typeof(OpenFile), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty MultiSelectProperty = DependencyProperty.Register(nameof(MultiSelect), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty ReadOnlyCheckedProperty = DependencyProperty.Register(nameof(ReadOnlyChecked), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty RestoreDirectoryProperty = DependencyProperty.Register(nameof(RestoreDirectory), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty ShowReadOnlyProperty = DependencyProperty.Register(nameof(ShowReadOnly), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(OpenFile), new FrameworkPropertyMetadata("Open File...", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static DependencyProperty ValidateNamesProperty = DependencyProperty.Register(nameof(ValidateNames), typeof(bool), typeof(OpenFile), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public OpenFile()
		{
			InitializeComponent();
		}

		public bool AddExtension
		{
			get => (bool)GetValue(AddExtensionProperty);
			set => SetValue(AddExtensionProperty, value);
		}

		public bool CheckFileExists
		{
			get => (bool)GetValue(CheckFileExistsProperty);
			set => SetValue(CheckFileExistsProperty, value);
		}

		public bool CheckPathExists
		{
			get => (bool)GetValue(CheckPathExistsProperty);
			set => SetValue(CheckPathExistsProperty, value);
		}

		public string DefaultExt
		{
			get => (string)GetValue(DefaultExtProperty);
			set => SetValue(DefaultExtProperty, value);
		}

		public bool DereferenceLinks
		{
			get => (bool)GetValue(DereferenceLinksProperty);
			set => SetValue(DereferenceLinksProperty, value);
		}

		public string FileName
		{
			get => (string)GetValue(FileNameProperty);
			set => SetValue(FileNameProperty, value);
		}
		
		public string Filter
		{
			get => (string)GetValue(FilterProperty);
			set => SetValue(FilterProperty, value);
		}
		
		public int FilterIndex
		{
			get => (int)GetValue(FilterIndexProperty);
			set => SetValue(FilterIndexProperty, value);
		}
		
		public string InitialDirectory
		{
			get => (string)GetValue(InitialDirectoryProperty);
			set => SetValue(InitialDirectoryProperty, value);
		}
		
		public bool MultiSelect
		{
			get => (bool)GetValue(MultiSelectProperty);
			set => SetValue(MultiSelectProperty, value);
		}
		
		public bool ReadOnlyChecked
		{
			get => (bool)GetValue(ReadOnlyCheckedProperty);
			set => SetValue(ReadOnlyCheckedProperty, value);
		}
		
		public bool RestoreDirectory
		{
			get => (bool)GetValue(RestoreDirectoryProperty);
			set => SetValue(RestoreDirectoryProperty, value);
		}
		
		public bool ShowReadOnly
		{
			get => (bool)GetValue(ShowReadOnlyProperty);
			set => SetValue(ShowReadOnlyProperty, value);
		}

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}
		
		public bool ValidateNames
		{
			get => (bool)GetValue(ValidateNamesProperty);
			set => SetValue(ValidateNamesProperty, value);
		}

		private void OnBrowseClick(object sender, RoutedEventArgs e)
		{
			VistaOpenFileDialog dlg = new VistaOpenFileDialog
			{
				AddExtension = AddExtension,
				CheckFileExists = CheckFileExists,
				CheckPathExists = CheckPathExists,
				DefaultExt = DefaultExt,
				DereferenceLinks = DereferenceLinks,
				FileName = FileName,
				Filter = Filter,
				FilterIndex = FilterIndex,
				InitialDirectory = InitialDirectory,
				Multiselect = MultiSelect,
				ReadOnlyChecked = ReadOnlyChecked,
				RestoreDirectory = RestoreDirectory,
				ShowReadOnly = ShowReadOnly,
				Title = Title,
				ValidateNames = ValidateNames
			};

			HwndSource hWnd = (HwndSource)PresentationSource.FromVisual(this);
			IntPtr handle = hWnd?.Handle ?? IntPtr.Zero;
			if (dlg.ShowDialog(handle) != true) return;
			FileName = dlg.FileName;
		}
	}
}
