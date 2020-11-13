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
using System.CodeDom;

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for RecipeDetailPage.xaml
	/// </summary>
	public partial class RecipeDetailPage : Page
	{
		public delegate void GoShoppingHandler();
		public event GoShoppingHandler GoShopping;

		public delegate void ReloadRecipePageHandler(int recipeID, bool isPlay, bool isMute, double currentVolume, double currentTime);
		public event ReloadRecipePageHandler ReloadRecipePage;

		private DBUtilities _dbUtilities = DBUtilities.GetDBInstance();
		private AppUtilities _appUtilities = new AppUtilities();
		private int _recipeID;
		private Recipe _recipe;

		public RecipeDetailPage()
		{
			InitializeComponent();

			carouselDialog.SetParent(mainContainer);
			fullScreenVideoDialog.SetParent(mainContainer);
		}

		public RecipeDetailPage(int recipeID, bool isPlay, bool isMute, double currentVolume, double currentTime)
		{
			InitializeComponent();			

			carouselDialog.SetParent(mainContainer);
			fullScreenVideoDialog.SetParent(mainContainer);

			_recipeID = recipeID;

			_recipe = _dbUtilities.GetRecipeById(recipeID);

			_recipe = _appUtilities.getRecipeForBindingInRecipeDetail(_recipe, false);

			playVideoTutorial(_recipe.LINK_VIDEO, isPlay, isMute, currentVolume, currentTime);

			mainContainer.DataContext = _recipe;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{

		}
		private void playVideoTutorial(string url, bool isPlay, bool isMute, double currentVolume, double currentTime)
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

				localMediaPlayer.IsPlay = isPlay;
				localMediaPlayer.IsMute = isMute;
				localMediaPlayer.CurrentVolume = currentVolume;
				localMediaPlayer.CurrentTime = currentTime;

				var errorMessage = localMediaPlayer.PlayVideoFromUri(url);

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
			ReloadRecipePage?.Invoke(_recipeID, localMediaPlayer.IsPlay, localMediaPlayer.IsMute, localMediaPlayer.CurrentVolume, localMediaPlayer.CurrentTime);
		}

		private void foodRecipeImageContainer_Click(object sender, RoutedEventArgs e)
		{
			var selectedButton = (Button)sender;
			StepImage selectedImage = null;
			var selectedStep = 0;
			var selectedIndex = 0;
			List<StepImage> selectedStepImages = new List<StepImage>();

			for (int i = 0; i < _recipe.IMAGES_LIST_FOR_BINDING.Count; i++)
			{
				if (_recipe.IMAGES_LIST_FOR_BINDING[i].LINK_IMAGES == selectedButton.Tag.ToString())
				{
					selectedStep = _recipe.IMAGES_LIST_FOR_BINDING[i].NO_STEP;
					selectedImage = _recipe.IMAGES_LIST_FOR_BINDING[i];
					break;
				}
			}

			foreach (var image in _recipe.IMAGES_LIST_FOR_BINDING)
			{
				if (image.NO_STEP == selectedStep)
				{
					selectedStepImages.Add(image);
				}
			}

			selectedIndex = selectedStepImages.IndexOf(selectedImage);

			youtubeWebView.Visibility = Visibility.Hidden;
			carouselDialog.ShowDialog(selectedStepImages, selectedIndex);
		}

		private void fullScreenVideoDialog_CloseFullScreenVideoDialog(bool isPlay, bool isMute, double currentVolume, double currentTime)
		{
			ReloadRecipePage?.Invoke(_recipeID, isPlay, isMute, currentVolume, currentTime);
		}

		private void getStatusVideo(bool isPlay, bool isMute, double currentVolume, double currentTime)
        {

        }

		private void localMediaPlayer_FullScreenClick(bool isFullScreen)
		{
			if (isFullScreen)
			{
				//Params will define depend on your need

				fullScreenVideoDialog.ShowDialog(_recipe.LINK_VIDEO, localMediaPlayer.IsPlay, localMediaPlayer.IsMute, localMediaPlayer.CurrentVolume, localMediaPlayer.CurrentTime);

				if (localMediaPlayer.IsPlay)
				{
					localMediaPlayer.IsPlay = false;
				}
				else
				{
					//Do Nothing
				}
			}	
		}

		public void playUnfinishedVideo()
        {
			localMediaPlayer.playUnfinishedVideo();
		}
	}
}
