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
		private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();

		private int _currentPage;
		private int _maxPage = 0;
		private bool _isFavorite = false;

		public HomePage()
		{
			InitializeComponent();

			_currentPage = 1;
			_maxPage = getMaxPage();

			loadRecipes();
		}

		public HomePage(bool isFavorite)
		{
			InitializeComponent();

			if (isFavorite)
			{
				_isFavorite = true;

				_currentPage = 1;
				_maxPage = getMaxPage();

				loadRecipes();
			}	
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
		}

		private void gridTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void sortTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void recipesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedItemIndex = recipesListView.SelectedIndex;
			int selectedID = -1;

			if (selectedItemIndex != -1)
			{
				selectedID = ((GetRecipeByPage_Result)recipesListView.SelectedItem).ID_RECIPE;
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

			loadRecipes();
		}

		private void nextPageButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentPage < (int)_maxPage)
			{
				++_currentPage;
			}

			loadRecipes();
		}

		private void firstPageButton_Click(object sender, RoutedEventArgs e)
		{
			_currentPage = 1;

			loadRecipes();
		}

		private void lastPageButton_Click(object sender, RoutedEventArgs e)
		{
			_currentPage = getMaxPage();

			loadRecipes();
		}

		private int getMaxPage()
        {
			var result = Math.Ceiling((double)(_dbUtilities.GetMaxIDRecipe()) / (double)getTotalRecipePerPage());

			return (int)result;
		}
		private int getTotalRecipePerPage()
        {
			var totalRecipePerPageString = gridTypeComboBox.Text;

			string[] paramsRowXColum = totalRecipePerPageString.Split('x');
			var result = int.Parse(paramsRowXColum[0]) * int.Parse(paramsRowXColum[1]);

			return result;
		}

		private void loadRecipes() {
			if (!_isFavorite)
            {
				_maxPage = getMaxPage();
				currentPageTextBlock.Text = $"{_currentPage} of {(_maxPage)}";

				List<GetRecipeByPage_Result> recipes = _dbUtilities.GetRecipeByPage(_currentPage, getTotalRecipePerPage()).ToList();

				foreach (var recipe in recipes)
				{
					recipe.NAME_FOR_BINDING = _appUtilities.getStandardName(recipe.NAME, false);

					recipe.LINK_AVATAR = (string)(_absolutePathConverter.Convert($"Images/{recipe.ID_RECIPE}/avatar.{recipe.LINK_AVATAR}", null, null, null));

					Debug.WriteLine(recipe.LINK_AVATAR);
				}

				recipesListView.ItemsSource = recipes;
			}
			else
            {

            }

	
		}
	}
}
