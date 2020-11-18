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
using System.Net.Http;
using System.Net;

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for RecipeDetailPage.xaml
	/// </summary>
	public partial class RecipeDetailPage : Page
	{
		public delegate void GoShoppingHandler();
		public event GoShoppingHandler GoShopping;

		public delegate void ReloadRecipePageHandler(int recipeID);
		public event ReloadRecipePageHandler ReloadRecipePage;

		private DBUtilities _dbUtilities = DBUtilities.GetDBInstance();
		private AppUtilities _appUtilities = new AppUtilities();
		private int _recipeID;
		private Recipe _recipe;
		private bool _isYoutubeWebView = true;

		private readonly HttpClient _client = new HttpClient();
		public RecipeDetailPage()
		{
			InitializeComponent();

			carouselDialog.SetParent(mainContainer);
			fullScreenVideoDialog.SetParent(mainContainer);
			fullScreenYoutubeDialog.SetParent(mainContainer);
		}

		public RecipeDetailPage(int recipeID)
		{
			InitializeComponent();			

			carouselDialog.SetParent(mainContainer);
			fullScreenVideoDialog.SetParent(mainContainer);
			fullScreenYoutubeDialog.SetParent(mainContainer);

			_recipeID = recipeID;

			_recipe = _dbUtilities.GetRecipeById(recipeID);

			_recipe = _appUtilities.getRecipeForBindingInRecipeDetail(_recipe, false);

			mainContainer.DataContext = _recipe;

			_ = loadVideoTutorial(_recipe.LINK_VIDEO);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			
		}

		private async Task<bool> loadVideoTutorial(string url)
		{
			if (url.IndexOf("http") != -1 || url.IndexOf("https") != -1)
			{
                youtubeThumbnail.Visibility = Visibility.Visible;

                string[] urlParams = url.Split('=');

                string urlID = "";

                if (url.IndexOf("=") != -1)
                {

                    string urlParamsIDAndFeture = urlParams[1];
                    string[] rawUrl = urlParamsIDAndFeture.Split('&');

                    if (rawUrl.Length > 0)
                    {
                        urlID = rawUrl[0];
                    }
                    else
                    {
                        urlID = urlParams[1];
                    }
                }
                else
                {
                    urlParams = url.Split('/');
                    urlID = urlParams[3];
                }

                _isYoutubeWebView = true;

                //bắt ngoại lệ
                var sourceThumbnail = $"https://img.youtube.com/vi/{urlID}/0.jpg";

				var existThumbnail = await checkExistThumbnailYoutubeVideoFromURL(sourceThumbnail);

				if (existThumbnail)
                {
					BitmapImage bitmap = new BitmapImage();

					statusVideoContainer.Visibility = Visibility.Collapsed;

					//nếu không load được thì hiện statusVideoContainer
					bitmap.BeginInit();
					bitmap.CacheOption = BitmapCacheOption.OnLoad;
					bitmap.UriSource = new Uri(sourceThumbnail);
					bitmap.EndInit();

					youtubeThumbnail.Source = bitmap;

					playVideoButton.Visibility = Visibility.Visible;
					statusVideoContainer.Visibility = Visibility.Collapsed;
				}
				else
                {
					statusVideoContainer.Visibility = Visibility.Visible;
					playVideoButton.Visibility = Visibility.Hidden;
				}
            }
			else
			{
				localMediaPlayer.Visibility = Visibility.Visible;
				statusVideoContainer.Visibility = Visibility.Collapsed;
				playVideoButton.Visibility = Visibility.Visible;

				localMediaPlayer.IsPlay = true;

				_isYoutubeWebView = false;

				//không load được thì hiện status video
				if (!localMediaPlayer.PlayVideoFromUri(url))
				{
					statusVideoContainer.Visibility = Visibility.Visible;
				}

			}

			return true;
		}

		private async Task<bool> checkExistThumbnailYoutubeVideoFromURL(string URL)
        {
			bool result = false;

			var response = await _client.GetAsync(URL);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				result = true;
			}
			else
			{
				result = false;
			}

			return result;
		}

		private void addShoppingButton_Click(object sender, RoutedEventArgs e)
		{
			//Test Show snack bar
			notiMessageSnackbar.MessageQueue.Enqueue($"Đã thêm {_recipe.NAME} vào danh sách shopping", "GO SHOPPING", () => { GoShoppingPage(); });

			_dbUtilities.TurnShoppingFlagOn(_recipeID);
		}

		private void GoShoppingPage()
		{
			GoShopping?.Invoke();
		}

        private void favButton_Checked(object sender, RoutedEventArgs e)
        {
			if (this.IsLoaded)
            {
				bool isFavoriteRecipe = ((ToggleButton)sender).IsChecked.Value;

				if (isFavoriteRecipe)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Đã thêm {_recipe.NAME} vào danh sách yêu thích", "OK", () => { });

					_dbUtilities.TurnFavoriteFlagOn(_recipeID);
				}
				else
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Đã xóa {_recipe.NAME} khỏi danh sách yêu thích", "OK", () => { });

					_dbUtilities.TurnFavoriteFlagOff(_recipeID);
				}
			}
		}
		private void imageRecipeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine(imageRecipeListView.SelectedIndex);

			youtubeThumbnail.Visibility = Visibility.Hidden;
			carouselDialog.ShowDialog(_recipe.IMAGES_LIST_FOR_BINDING, imageRecipeListView.SelectedIndex);
		}

		private void CarouselDialog_CloseCarouselDialog()
		{
			ReloadRecipePage?.Invoke(_recipeID);
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

			youtubeThumbnail.Visibility = Visibility.Hidden;
			carouselDialog.ShowDialog(selectedStepImages, selectedIndex);
		}

		private void fullScreenVideoDialog_CloseFullScreenVideoDialog()
		{
			ReloadRecipePage?.Invoke(_recipeID);
		}

		private void fullScreenYoutubeDialog_CloseFullScreenVideoDialog()
		{
			ReloadRecipePage?.Invoke(_recipeID);
		}

		private void playVideoButton_Click(object sender, RoutedEventArgs e)
		{
			if (_isYoutubeWebView)
			{
				fullScreenYoutubeDialog.ShowDialog(_recipe.LINK_VIDEO);
			}
			else
			{
				fullScreenVideoDialog.localMediaPlayer.IsFullScreen = true;
				fullScreenVideoDialog.ShowDialog(_recipe.LINK_VIDEO);
			}
		}
	}
}
