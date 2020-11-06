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
using FoodRecipes.Utilities;
using System.Configuration;

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for ShoppingPage.xaml
	/// </summary>
	public partial class ShoppingPage : Page
	{
		private List<Button> _shoppingButtonItems;
		private AppUtilities _appUtilities = new AppUtilities();
		private DBUtilities _dbUtilities = DBUtilities.GetDBInstance();
		private Configuration _configuration;

		private List<Recipe> _shoppingRecipes = new List<Recipe>();

		private int _deleteRecipeID = -1;

		private int _sortedBy = 0;
		private (string column, string type)[] _conditionSortedBy = {("ADD_DATE", "DESC"), ("ADD_DATE", "ASC"),
																	 ("NAME", "ASC"), ("NAME", "DESC")};

		public ShoppingPage()
		{
			InitializeComponent();

			_configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			_sortedBy = int.Parse(ConfigurationManager.AppSettings["SortedByShoppingPage"]);
			sortTypeComboBox.SelectedIndex = _sortedBy;

			_shoppingButtonItems = new List<Button>();

			loadRecipes();
		}

		private void foodGroupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (var item in foodGroupListBox.SelectedItems)
			{
				var selectedButton = ((Button)item);

				selectedButton.Background = (SolidColorBrush)FindResource("MyYellow");
			}

			if (this.IsLoaded)
            {
				loadRecipes();
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

			if (this.IsLoaded)
			{
				loadRecipes();
			}
		}

		private void sortTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.IsLoaded)
			{
				_sortedBy = sortTypeComboBox.SelectedIndex;

				_configuration.AppSettings.Settings["SortedByShoppingPage"].Value = _sortedBy.ToString();
				_configuration.Save(ConfigurationSaveMode.Minimal);

				loadRecipes();
			}
		}

		private void shoppingRecipeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedItemIndex = shoppingRecipeListView.SelectedIndex;
			Debug.WriteLine(selectedItemIndex);

			//Get Id recipe base on item clikced
			if (shoppingRecipeListView.SelectedIndex != -1)
			{
				Debug.WriteLine(shoppingRecipeListView.SelectedIndex);
				//((Button)shoppingRecipeListView.SelectedItem).Background = (SolidColorBrush)FindResource("MyRed");
			}
		}

		private void shoppingCardContainer_Click(object sender, RoutedEventArgs e)
		{
			if (_deleteRecipeID == -1)
            {
				var selectedButton = (Button)sender;

				if (!_shoppingButtonItems.Contains(selectedButton))
				{
					_shoppingButtonItems.Add(selectedButton);
				}

				foreach (var button in _shoppingButtonItems)
				{
					button.Background = (SolidColorBrush)FindResource("MyOrange");
				}

				selectedButton.Background = (SolidColorBrush)FindResource("MyRed");

				var selectID = int.Parse(selectedButton.Tag.ToString());

				var recipe = from r in _shoppingRecipes
							 where r.ID_RECIPE == selectID
							 select r;

				shoppingIgredientListView.ItemsSource = recipe.First().IGREDIENT_LIST_FOR_BINDING;
			}
			else
            {
				//Do nothing
            }
		
		}

		private string getConditionInQuery()
		{
			string result = "";

			if (foodGroupListBox.SelectedItems.Count > 0)
			{
				result += "(";

				string[] foodGroups = { "Ăn sáng", "Ăn vặt", "Healthy", "Món chính", "Món chay", "Thức uống" };
				foreach (var foodGroupListBoxSelectedItem in foodGroupListBox.SelectedItems)
				{
					var selectedButton = ((Button)foodGroupListBoxSelectedItem);
					int index = int.Parse(selectedButton.Tag.ToString());
					result += $" FOOD_GROUP = N\'{foodGroups[index]}\' OR";
				}

				result = result.Substring(0, result.Length - 3);

				if (foodGroupListBox.SelectedItems.Count > 0)
				{
					result += ")";
				}
				
			}
			else
			{
				//Do Nothing
			}
			
			return result;
		}

		private void deleteShoppingRecipeButton_Click(object sender, RoutedEventArgs e)
		{
			//Test Show snack bar
            var selectedButton = (Button)sender;
            _deleteRecipeID = int.Parse(selectedButton.Tag.ToString());

            var deleteRecipeName = (from r in _shoppingRecipes
                        where r.ID_RECIPE == _deleteRecipeID
						select r).Single().NAME;

			notiMessageSnackbar.MessageQueue.Enqueue($"Đã xóa {_appUtilities.getStandardName(deleteRecipeName, true)}", "UNDO", () => { UndoDeleteShoppingItem(); });

			_dbUtilities.TurnShoppingFlagOff(_deleteRecipeID);

			loadRecipes();
		}

		private void UndoDeleteShoppingItem()
		{
			_dbUtilities.TurnShoppingFlagOn(_deleteRecipeID);
			_deleteRecipeID = -1;
			loadRecipes();
		}

		private void loadRecipes()
        {
			//_shoppingButtonItems[0].Background = (SolidColorBrush)FindResource("MyRed");

			string condition = getConditionInQuery();
			_shoppingRecipes = _dbUtilities.GetShoppingRecipes(condition, _conditionSortedBy[_sortedBy]);
				
			for (int i = 0; i < _shoppingRecipes.Count; ++i)
			{
				_shoppingRecipes[i] = _appUtilities.getRecipeForBindingInRecipeDetail(_shoppingRecipes[i]);
			}

			if (_shoppingRecipes.Count > 0)
            {
				shoppingRecipeListView.ItemsSource = _shoppingRecipes;
				//shoppingIgredientListView.ItemsSource = _shoppingRecipes[0].IGREDIENT_LIST_FOR_BINDING;
			}
			else
            {
				shoppingRecipeListView.ItemsSource = null;
				shoppingIgredientListView.ItemsSource = null;
			}
		}
	}
}
