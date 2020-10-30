using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
	/// Interaction logic for ShoppingPage.xaml
	/// </summary>
	public partial class ShoppingPage : Page
	{
		private List<Button> _shoppingButtonItems;
		public ShoppingPage()
		{
			InitializeComponent();
			_shoppingButtonItems = new List<Button>();
		}

		private void foodGroupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (var item in foodGroupListBox.SelectedItems)
			{
				var selectedButton = ((Button)item);

				selectedButton.Background = (SolidColorBrush)FindResource("MyYellow");
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
				shoppingListsContainer.SetValue(Grid.RowProperty, 1);
				shoppingListsContainer.SetValue(Grid.RowSpanProperty, 2);
			}
			else
			{
				foodGroupListBox.Visibility = Visibility.Visible;
				shoppingListsContainer.SetValue(Grid.RowProperty, 2);
				shoppingListsContainer.SetValue(Grid.RowSpanProperty, 1);
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

		private void sortTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void shoppingRecipeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (shoppingRecipeListView.SelectedIndex != -1)
			{
				Debug.WriteLine(shoppingRecipeListView.SelectedIndex);
				Debug.WriteLine(((Button)shoppingRecipeListView.SelectedItem).Tag);

				((Button)shoppingRecipeListView.SelectedItem).Background = (SolidColorBrush)FindResource("MyRed");
			}
		}

		private void shoppingCardContainer_Click(object sender, RoutedEventArgs e)
		{
			var selectedButton = (Button)sender;

			if (!_shoppingButtonItems.Contains(selectedButton))
			{
				_shoppingButtonItems.Add(selectedButton);
			}
			
			foreach(var button in _shoppingButtonItems)
			{
				button.Background = (SolidColorBrush)FindResource("MyOrange");
			}

			selectedButton.Background = (SolidColorBrush)FindResource("MyRed");
		}

		private void deleteShoppingRecipeButton_Click(object sender, RoutedEventArgs e)
		{
			//Test Show snack bar
			notiMessageSnackbar.MessageQueue.Enqueue("Test", "UNDO", () => { UndoDeleteShoppingItem(); });
		}

		private void UndoDeleteShoppingItem()
		{

		}

	}
}
