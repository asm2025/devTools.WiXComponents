using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using devTools.WiXComponents.Controls.Helpers;
using essentialMix.Helpers;
using Image = System.Windows.Controls.Image;

namespace devTools.WiXComponents.Controls
{
	/// <inheritdoc cref="Button" />
	public class UACShieldButton : Button
	{
		public static readonly DependencyProperty IconRequestedProperty = DependencyProperty.Register(nameof(IconRequested),
																										typeof(bool),
																										typeof(UACShieldButton),
																										new FrameworkPropertyMetadata(true,
																																	FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender,
																																	OnIconRequestedChanged));

		public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
																								typeof(ImageSource),
																								typeof(UACShieldButton),
																								new FrameworkPropertyMetadata(ImageSourceHelper.FromSystem(SHSTOCKICONID.SIID_SHIELD),
																															FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender,
																															OnIconChanged));

		public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(UACShieldButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		private readonly Image _image;

		/// <inheritdoc />
		public UACShieldButton()
		{
			_image = new Image
			{
				Source = Icon,
				Margin = new Thickness(0, 0, 4, 0),
				Stretch = Stretch.None,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				Visibility = IconRequested && !WindowsIdentityHelper.HasElevatedPrivileges
								? Visibility.Visible
								: Visibility.Collapsed
			};
			TextBlock text = new TextBlock
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center
			};
			text.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text))
			{
				RelativeSource = new RelativeSource
				{
					Mode = RelativeSourceMode.FindAncestor,
					AncestorType = GetType()
				}
			});
			Grid.SetColumn(text, 1);
			Grid grid = new Grid
			{
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				},
				Children =
				{
					_image,
					text
				}
			};
			Content = grid;
		}

		public bool IconRequested
		{
			get => (bool)GetValue(IconRequestedProperty);
			set => SetValue(IconRequestedProperty, value);
		}

		public ImageSource Icon
		{
			get => (ImageSource)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		private static void OnIconRequestedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			UACShieldButton button = (UACShieldButton)sender;
			button._image.Visibility = (bool)args.NewValue && !WindowsIdentityHelper.HasElevatedPrivileges
									? Visibility.Visible
									: Visibility.Collapsed;
		}

		private static void OnIconChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			UACShieldButton button = (UACShieldButton)sender;
			button._image.Source = args.NewValue as ImageSource;
			button._image.Visibility = (bool)sender.GetValue(IconRequestedProperty) && !WindowsIdentityHelper.HasElevatedPrivileges
											? Visibility.Visible
											: Visibility.Collapsed;
		}
	}
}
