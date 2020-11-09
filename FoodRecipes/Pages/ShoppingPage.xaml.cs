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

		private int _selectedID = 0;

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

				if (_deleteRecipeID == -1)
				{
					var selectedRecipe = (Recipe)shoppingRecipeListView.SelectedItem;

					var recipe = from r in _shoppingRecipes
								 where r.ID_RECIPE == selectedRecipe.ID_RECIPE
								 select r;

					_selectedID = selectedRecipe.ID_RECIPE;

					shoppingIgredientListView.ItemsSource = recipe.First().IGREDIENT_LIST_FOR_BINDING;
				}
				else
				{
					//Do nothing
				}
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
			var isChangeSelected = false;
			var selectedButton = (Button)sender;
			int currentSelectedID = -1;

			while (currentSelectedID == -1)
			{
				currentSelectedID = ((Recipe)shoppingRecipeListView.SelectedItem).ID_RECIPE;
			}

			_deleteRecipeID = int.Parse(selectedButton.Tag.ToString());

			var deleteRecipeName = (from r in _shoppingRecipes
									where r.ID_RECIPE == _deleteRecipeID
									select r).Single().NAME;

			notiMessageSnackbar.MessageQueue.Enqueue($"Đã xóa {_appUtilities.getStandardName(deleteRecipeName, true)}", "UNDO", () => { UndoDeleteShoppingItem(currentSelectedID); });

			_dbUtilities.TurnShoppingFlagOff(_deleteRecipeID);

			if (_deleteRecipeID == currentSelectedID)
			{
				isChangeSelected = true;
			}

			loadRecipes();

			if (isChangeSelected)
			{
				shoppingRecipeListView.SelectedIndex = 0;
			}
			else
			{
				for (int i = 0; i < _shoppingRecipes.Count; i++)
				{
					if (_shoppingRecipes[i].ID_RECIPE == currentSelectedID)
					{
						shoppingRecipeListView.SelectedIndex = i;
						shoppingIgredientListView.ItemsSource = _shoppingRecipes[i].IGREDIENT_LIST_FOR_BINDING;
						break;
					}
				}
			}
		}

		private void UndoDeleteShoppingItem(int currentSelectedID)
		{
			_dbUtilities.TurnShoppingFlagOn(_deleteRecipeID);

			loadRecipes();

			for (int i = 0; i < _shoppingRecipes.Count; i++)
			{
				var selectedID = (_deleteRecipeID == currentSelectedID) ? _deleteRecipeID : currentSelectedID;

				if (_shoppingRecipes[i].ID_RECIPE == selectedID)
				{
					shoppingRecipeListView.SelectedIndex = i;
					shoppingIgredientListView.ItemsSource = _shoppingRecipes[i].IGREDIENT_LIST_FOR_BINDING;
					break;
				}
			}

			_deleteRecipeID = -1;
		}

		private void loadRecipes()
		{
			string condition = getConditionInQuery();
			_shoppingRecipes = _dbUtilities.GetShoppingRecipes(condition, _conditionSortedBy[_sortedBy]);

			for (int i = 0; i < _shoppingRecipes.Count; ++i)
			{
				_shoppingRecipes[i] = _appUtilities.getRecipeForBindingInRecipeDetail(_shoppingRecipes[i]);
			}

			if (_shoppingRecipes.Count > 0)
			{
				shoppingRecipeListView.ItemsSource = _shoppingRecipes;

				bool indexFlag = false;
				int index;

				for (index = 0; index < _shoppingRecipes.Count; ++index)
                {
					if (_shoppingRecipes[index].ID_RECIPE == _selectedID)
                    {
						indexFlag = true;
						break;
					} 
                }

				if (indexFlag)
                {
					shoppingRecipeListView.SelectedIndex = index;
					shoppingIgredientListView.ItemsSource = _shoppingRecipes[index].IGREDIENT_LIST_FOR_BINDING;
				}
				else
                {
					shoppingRecipeListView.SelectedIndex = 0;
					shoppingIgredientListView.ItemsSource = _shoppingRecipes[0].IGREDIENT_LIST_FOR_BINDING;
				}
			}
			else
			{
				shoppingRecipeListView.ItemsSource = null;
				shoppingIgredientListView.ItemsSource = null;
			}
		}		
	}
}
