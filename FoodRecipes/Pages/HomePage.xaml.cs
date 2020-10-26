using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for HomePage.xaml
	/// </summary>
	public partial class HomePage : Page
	{

		public HomePage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = this;
			
		}

		private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
		{

		}

		private void foodGroupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (var item in foodGroupListBox.SelectedItems)
			{
				var selectedButton = ((Button)item);

				selectedButton.Background = (SolidColorBrush) FindResource("MyYellow");
			}
		}

		private void groupButton_Click(object sender, RoutedEventArgs e)
		{
			//Convert current clicked button to list item
			var clickedButton = ((Button)sender);
			var clickedItemIdx = int.Parse(clickedButton.Tag.ToString());
			var clickedItem = foodGroupListBox.Items.GetItemAt(clickedItemIdx);

			//Add this converted item if new else remove it
			if (foodGroupListBox.SelectedItems.Contains(clickedItem))
			{
				foodGroupListBox.SelectedItems.Remove(clickedItem);
				clickedButton.Background = (SolidColorBrush)FindResource("MyLightGray");
			}
			else
			{
				foodGroupListBox.SelectedItems.Add(clickedItem);
			}				
			

			Debug.WriteLine(((Button)sender).Tag.ToString());

			foreach (var item in foodGroupListBox.SelectedItems)
			{
				
				Debug.WriteLine(((Button)item).Name);
			}	
		}

		private void filterButton_Click(object sender, RoutedEventArgs e)
		{
			if (foodGroupListBox.Visibility == Visibility.Visible)
			{
				foodGroupListBox.Visibility = Visibility.Collapsed;
			}
			else
			{
				foodGroupListBox.Visibility = Visibility.Visible;
			}
		}

		private void eraserAllFilterButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in foodGroupListBox.SelectedItems)
			{
				var selectedButton = ((Button)item);

				selectedButton.Background = (SolidColorBrush)FindResource("MyLightGray");
			}

			foodGroupListBox.SelectedItems.Clear();
		}
	}
}
