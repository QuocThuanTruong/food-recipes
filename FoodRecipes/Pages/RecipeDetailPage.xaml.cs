using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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
using FoodRecipes.Converter;
using FoodRecipes.Utilities;
using System.Diagnostics;
using System.Media;
using System.Windows.Controls.Primitives;
using System.Security.Policy;

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for RecipeDetailPage.xaml
	/// </summary>
	public partial class RecipeDetailPage : Page
	{
		public delegate void GoShoppingHandler();
		public event GoShoppingHandler GoShopping;

		private DBUtilities _dbUtilities = DBUtilities.GetDBInstance();
		private AppUtilities _appUtilities = new AppUtilities();
		private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();
		private int _recipeID;
		private Recipe _recipe;

		public RecipeDetailPage()
		{
			InitializeComponent();

			carouselDialog.SetParent(mainContainer);
		}

		public RecipeDetailPage(int recipeID)
		{
			InitializeComponent();			

			carouselDialog.SetParent(mainContainer);

			_recipeID = recipeID;

			_recipe = _dbUtilities.GetRecipeById(recipeID);

			_recipe = _appUtilities.getRecipeForBindingInRecipeDetail(_recipe, false);

			playVideoTutorial(_recipe.LINK_VIDEO);

			mainContainer.DataContext = _recipe;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{

		}
		private void playVideoTutorial(string url)
        {
			if (url.IndexOf("http") != -1 || url.IndexOf("https") != -1)
            {
				youtubeWebView.Visibility = Visibility.Visible;

				var errorMessage = youtubeWebView.PlayVideoFromUrl(url);

				if (errorMessage != "")
				{
					notiMessageSnackbar.MessageQueue.Enqueue(errorMessage, "OK", () => { });
				}
			} 
			else
            {
				localMediaPlayer.Visibility = Visibility.Visible;

				var errorMessage = localMediaPlayer.PlayeVideoFromUrl(url);

				if (errorMessage != "")
				{
					notiMessageSnackbar.MessageQueue.Enqueue(errorMessage, "OK", () => { });
				}
			}
		}

		private void addShoppingButton_Click(object sender, RoutedEventArgs e)
		{
			//Test Show snack bar
			notiMessageSnackbar.MessageQueue.Enqueue("Đã thêm...", "GO SHOPPING", () => { GoShoppingPage(); });

			_dbUtilities.TurnShoppingFlagOn(_recipeID);
		}

		private void GoShoppingPage()
		{
			GoShopping?.Invoke();
		}

        private void videoContainer_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void videoContainer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void favButton_Checked(object sender, RoutedEventArgs e)
        {
			bool isFavoriteRecipe = ((ToggleButton)sender).IsChecked.Value;

			if (isFavoriteRecipe)
			{
				_dbUtilities.TurnFavoriteFlagOn(_recipeID);
			}
			else
			{
				_dbUtilities.TurnFavoriteFlagOff(_recipeID);
			}
		}
		private void imageRecipeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine(imageRecipeListView.SelectedIndex);

			youtubeWebView.Visibility = Visibility.Hidden;

			carouselDialog.ShowDialog(_recipe.IMAGES_LIST_FOR_BINDING, imageRecipeListView.SelectedIndex);
		}

		private void CarouselDialog_CloseCarouselDialog()
		{
			playVideoTutorial(_recipe.LINK_VIDEO);
		}
	}
}
