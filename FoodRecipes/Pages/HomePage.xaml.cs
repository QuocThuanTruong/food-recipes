using MaterialDesignThemes.Wpf;
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
using FoodRecipes.Utilities;
using FoodRecipes.Converter;
using System.Windows.Controls.Primitives;
using System.Configuration;
using System.Timers;

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for HomePage.xaml
	/// </summary>
	public partial class HomePage : Page
	{
		public delegate void ShowRecipeDetailPageHandler(int recipeID);
		public event ShowRecipeDetailPageHandler ShowRecipeDetailPage;

		private DBUtilities _dbUtilities = DBUtilities.GetDBInstance();
		private AppUtilities _appUtilities = new AppUtilities();
		private Configuration _configuration;

		private Timer _loadingTmer;
		private int _timeCounter = 0;

		private const int TIME_LOAD_UNIT = 100;
		private const int TOTAL_TIME_LOAD_IN_SECOND = 5;

		private int _currentPage;
		private int _maxPage = 0;
		private bool _isFavorite = false;
		private int _typeGridCard = 0;
		private bool _isSearching = false;
		private string _prevCondition = "init";
		private bool _isFirstSearch = true;
		private int _sortedBy = 0;
		private bool _canSearchRequest = false;
		private string _search_text = "";
		private string _condition = "";

		private (string column, string type)[] _conditionSortedBy = {("ADD_DATE", "DESC"), ("ADD_DATE", "ASC"),
																	 ("NAME", "ASC"), ("NAME", "DESC"),
																	 ("TIME", "DESC"), ("TIME", "ASC"),
																	 ("FOOD_LEVEL", "DESC"), ("FOOD_LEVEL", "ASC")};

		public HomePage()
		{
			InitializeComponent();

			_configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			_typeGridCard = int.Parse(ConfigurationManager.AppSettings["GridType"]);
			gridTypeComboBox.SelectedIndex = _typeGridCard;

			_sortedBy = int.Parse(ConfigurationManager.AppSettings["SortedByHomePage"]);
			sortTypeComboBox.SelectedIndex = _sortedBy;

			_loadingTmer = new Timer(TIME_LOAD_UNIT);
			_loadingTmer.Elapsed += LoadingTmer_Elapsed;
			_loadingTmer.Start();

			_isFavorite = false;

			_currentPage = 1;
	
			loadRecipes();
		} 

		public HomePage(bool isFavorite)
		{
			InitializeComponent();

			if (isFavorite)
			{
				_configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

				_typeGridCard = int.Parse(ConfigurationManager.AppSettings["GridType"]);
				gridTypeComboBox.SelectedIndex = _typeGridCard;

				_sortedBy = int.Parse(ConfigurationManager.AppSettings["SortedByHomePage"]);
				sortTypeComboBox.SelectedIndex = _sortedBy;

				_loadingTmer = new Timer(TIME_LOAD_UNIT);
				_loadingTmer.Elapsed += LoadingTmer_Elapsed;
				_loadingTmer.Start();

				_isFavorite = true;

				_currentPage = 1;

				_typeGridCard = gridTypeComboBox.SelectedIndex;

				loadRecipes();
			}	
		}
		private void LoadingTmer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				if (_isSearching)
                {
					++_timeCounter;

					if (_timeCounter % TOTAL_TIME_LOAD_IN_SECOND == 0 && _canSearchRequest)
					{
						_timeCounter = 0;

						loadRecipesSearch();
					}
				}
				else
                {
					_timeCounter = 0;
                }
				
			});
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = this;
		}

		private void foodGroupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (var item in foodGroupListBox.SelectedItems)
			{
				var selectedButton = ((Button)item);

				selectedButton.Background = (SolidColorBrush) FindResource("MyYellow");
			}

			if (this.IsLoaded)
			{
				_typeGridCard = gridTypeComboBox.SelectedIndex;
				if (_isSearching)
				{
					loadRecipesSearch();
				}
				else
				{
					loadRecipes();
				}
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
				recipesListView.SetValue(Grid.RowProperty, 1);
				recipesListView.SetValue(Grid.RowSpanProperty, 2);
			}
			else
			{
				foodGroupListBox.Visibility = Visibility.Visible;
				recipesListView.SetValue(Grid.RowProperty, 2);
				recipesListView.SetValue(Grid.RowSpanProperty, 1);
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
				_typeGridCard = gridTypeComboBox.SelectedIndex;

				_prevCondition = "init";

				if (_isSearching)
				{
					loadRecipesSearch();
				}
				else
				{
					loadRecipes();
				}
			}
		}

		private void gridTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.IsLoaded)
            {
				_typeGridCard = gridTypeComboBox.SelectedIndex;

				_configuration.AppSettings.Settings["GridType"].Value = _typeGridCard.ToString();
				_configuration.Save(ConfigurationSaveMode.Minimal);


				_currentPage = 1;
				if (_isSearching)
                {
					loadRecipesSearch();
                }
				else
                {
					loadRecipes();
				}
			}
		}

		private void sortTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			if (this.IsLoaded)
            {
				_sortedBy = sortTypeComboBox.SelectedIndex;

				_configuration.AppSettings.Settings["SortedByHomePage"].Value = _sortedBy.ToString();
				_configuration.Save(ConfigurationSaveMode.Minimal);

				if (_isSearching)
				{
					loadRecipesSearch();
				}
				else
				{
					loadRecipes();
				}
			}
			
		}

		private void recipesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedItemIndex = recipesListView.SelectedIndex;
			int selectedID = -1;

			if (selectedItemIndex != -1)
			{
				selectedID = ((Recipe)recipesListView.SelectedItem).ID_RECIPE;
				Debug.WriteLine(selectedID);
			}

			//Get Id recipe base on item clikced

			ShowRecipeDetailPage?.Invoke(selectedID);	
		}

		private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
		{
			notiMessageSnackbar.IsActive = false;
		}

		private void prevPageButton_Click(object sender, RoutedEventArgs e) { 
			if (_currentPage > 1)
            {
				--_currentPage;
            }

			if (_isSearching)
            {
				loadRecipesSearch();
            } 
			else
            {
				loadRecipes();
			}
		}

		private void nextPageButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentPage < (int)_maxPage)
			{
				++_currentPage;
			}

			if (_isSearching)
			{
				loadRecipesSearch();
			}
			else
			{
				loadRecipes();
			}
		}

		private void firstPageButton_Click(object sender, RoutedEventArgs e)
		{
			_currentPage = 1;

			if (_isSearching)
			{
				loadRecipesSearch();
			}
			else
			{
				loadRecipes();
			}
		}

		private void lastPageButton_Click(object sender, RoutedEventArgs e)
		{
			_currentPage = _maxPage;

			if (_isSearching)
			{
				loadRecipesSearch();
			}
			else
			{
				loadRecipes();
			}
		}

		private void favButton_Click(object sender, RoutedEventArgs e) {
			Debug.WriteLine(((ToggleButton)sender).Tag);
			 
			int ID_RECIPE = int.Parse(((ToggleButton)sender).Tag.ToString());

			bool isFavoriteRecipe = ((ToggleButton)sender).IsChecked.Value;

			var recipe = _dbUtilities.GetRecipeById(ID_RECIPE);

			if (isFavoriteRecipe) {
				notiMessageSnackbar.MessageQueue.Enqueue($"Đã thêm {_appUtilities.getStandardName(recipe.NAME, true)} vào danh sách yêu thích", "OK", () => {});

				_dbUtilities.TurnFavoriteFlagOn(ID_RECIPE);
			} 
			else
            {
				notiMessageSnackbar.MessageQueue.Enqueue($"Đã xóa {_appUtilities.getStandardName(recipe.NAME, true)} khỏi danh sách yêu thích", "OK", () => {});

				_dbUtilities.TurnFavoriteFlagOff(ID_RECIPE);
			}
		}

		private int getMaxPage(int totalRecipe)
        {
			var result = Math.Ceiling((double)totalRecipe / (double)getTotalRecipePerPage());

			return (int)result;
		}
		private int getTotalRecipePerPage()
        {
			int[] typeGridCardView = { 12, 16, 20 };

			var result = typeGridCardView[_typeGridCard];

			Debug.WriteLine(result);

			return result;
		}

		private string getConditionInQuery()
		{
			string result = "";

			if (_isFavorite)
			{
				//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1
				result += "FAVORITE_FLAG = 1 ";

				//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1 AND (
				if (foodGroupListBox.SelectedItems.Count > 0)
				{
					result += " AND (";
				}
				else
				{
					//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1
				}
			}
			else
			{
				if (foodGroupListBox.SelectedItems.Count > 0)
				{
					result += "(";
				}
				else
				{
					//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1
				}
			}

			//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1 AND (FOOD_GROUP = N'Ăn sáng' OR ...
			//Select * from [dbo].[Recipe] where FOOD_GROUP = N'Ăn sáng' OR
			string[] foodGroups = { "Ăn sáng", "Ăn vặt", "Healthy", "Món chính", "Món chay", "Thức uống" };
			foreach (var foodGroupListBoxSelectedItem in foodGroupListBox.SelectedItems)
			{
				var selectedButton = ((Button)foodGroupListBoxSelectedItem);
				int index = int.Parse(selectedButton.Tag.ToString());
				result += $" FOOD_GROUP = N\'{foodGroups[index]}\' OR";
			}

			if (result.Length > 0)
			{
				//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1 AND (FOOD_GROUP = N'Ăn sáng'
				//Select * from [dbo].[Recipe] where FOOD_GROUP = N'Ăn sáng'
				result = result.Substring(0, result.Length - 3);

				if (_isFavorite)
				{
					//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1 AND (FOOD_GROUP = N'Ăn sáng')
					if (foodGroupListBox.SelectedItems.Count > 0)
					{
						result += ")";
					}
					//Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1
					else
					{
						result += "1";
					}
				}
				else
				{
					if (foodGroupListBox.SelectedItems.Count > 0)
					{
						result += ")";
					}
				}
			}

			return result;
        }

		private void loadRecipes() {
			if (!_isSearching)
			{
				_search_text = "";
				_condition = "";

				string condition = getConditionInQuery();

				_isFirstSearch = true;

				if (_prevCondition != condition)
				{
					_currentPage = 1;
					_prevCondition = condition;
				}
				else
				{
					//Do Nothing
				}

				if (_currentPage == 4 || _currentPage == 3)
				{
					Debug.WriteLine("cc");
				}


				(List<Recipe> recipes, int totalRecipeResult) resultQuery = _dbUtilities.ExecQureyToGetRecipes(condition, _conditionSortedBy[_sortedBy], _currentPage, getTotalRecipePerPage());
				List<Recipe> recipes = resultQuery.recipes;

				_maxPage = getMaxPage(resultQuery.totalRecipeResult);
				if (_maxPage == 0)
				{
					_maxPage = 1;
					_currentPage = 1;

					messageNotFoundContainer.Visibility = Visibility.Visible;
				}
				else
				{
					messageNotFoundContainer.Visibility = Visibility.Hidden;
				}

				currentPageTextBlock.Text = $"{_currentPage} of {_maxPage}";

				if (recipes.Count() > 0)
				{
					for (int i = 0; i < recipes.Count; ++i)
					{
						recipes[i] = _appUtilities.getRecipeForBindingInHomePage(recipes[i]);
					}

					recipesListView.ItemsSource = recipes;

					currentResultTextBlock.Text = $"Hiển thị {recipes.Count} trong tổng số {resultQuery.totalRecipeResult} món ăn";
				}
				else
				{
					recipesListView.ItemsSource = null;
					currentResultTextBlock.Text = "";
				}
			}

			else
			{
				searchTextBox_TextChanged(null, null);
			}
			
		}

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			string search_text = searchTextBox.Text;
			
			if (search_text.Length != 0)
			{
				_isSearching = true;

				if (_isFirstSearch)
                {
					_currentPage = 1;
					_isFirstSearch = false;
				}

				string condition = getConditionInQuery();

				if (_search_text != search_text || _condition != condition)
				{
					_search_text = search_text;
					_condition = condition;

					_canSearchRequest = true;
				}

				_condition = condition;

				if (_prevCondition != condition)
				{
					_currentPage = 1;
					_prevCondition = condition;
				}
				else
				{
					//Do Nothing
				}
			}
			else
			{
				_isSearching = false;

				loadRecipes();
			}
		}

		private void loadRecipesSearch()
        {
			(List<Recipe> recipes, int totalRecipeResult) recipesSearchResults = _dbUtilities.GetRecipesSearchResult(_search_text, _condition, _conditionSortedBy[_sortedBy], _currentPage, getTotalRecipePerPage());

			_maxPage = getMaxPage(recipesSearchResults.totalRecipeResult);
			if (_maxPage == 0)
			{
				_maxPage = 1;
				_currentPage = 1;

				messageNotFoundContainer.Visibility = Visibility.Visible;
			}
			else
			{
				messageNotFoundContainer.Visibility = Visibility.Hidden;
			}

			currentPageTextBlock.Text = $"{_currentPage} of {(_maxPage)}";

			List<Recipe> recipes = recipesSearchResults.recipes;
			if (recipes.Count > 0)
			{
				for (int i = 0; i < recipes.Count; ++i)
				{
					recipes[i] = _appUtilities.getRecipeForBindingInHomePage(recipes[i]);
				}

				recipesListView.ItemsSource = recipes;

				currentResultTextBlock.Text = $"Hiển thị {recipes.Count} trong tổng số {recipesSearchResults.totalRecipeResult} món ăn";
			}
			else
			{
				recipesListView.ItemsSource = null;
				currentResultTextBlock.Text = "Không tìm thấy món ăn thỏa yêu cầu";
			}

			_canSearchRequest = false;
		}
    }
}
