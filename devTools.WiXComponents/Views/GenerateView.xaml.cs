using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace devTools.WiXComponents.Views;

/// <summary>
/// Interaction logic for Home.xaml
/// </summary>
public partial class GenerateView : UserControl
{
	public GenerateView()
	{
		InitializeComponent();
	}

	private void OnSelectAllClick(object sender, RoutedEventArgs e)
	{
		lbEntries.SelectAll();
	}

	private void OnSelectNoneClick(object sender, RoutedEventArgs e)
	{
		lbEntries.SelectedItems.Clear();
	}

	private void OnSelectInvertClick(object sender, RoutedEventArgs e)
	{
		ItemCollection items = lbEntries.Items;
		IList selected = lbEntries.SelectedItems;

		foreach (object item in items)
		{
			if (selected.Contains(item))
				selected.Remove(item);
			else
				selected.Add(item);
		}
	}
}