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

		public RecipeDetailPage()
		{
			InitializeComponent();
		}

		public RecipeDetailPage(int recipeID)
		{
			InitializeComponent();

			_recipeID = recipeID;

			Recipe recipe = _dbUtilities.GetRecipeById(recipeID);

			recipe = _appUtilities.getRecipeForBindingInRecipeDetail(recipe);

			playVideoTutorial(recipe.LINK_VIDEO);

			mainContainer.DataContext = recipe;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{

		}
		private void playVideoTutorial(string url)
        {
			if (url.IndexOf("http") != -1 || url.IndexOf("https") != -1)
            {
				try
				{
					videoContainerFromWeb.Visibility = Visibility.Visible;

					string html = "<html><head>";
					html += "<meta content='IE=Edge' http-equiv='X-UA-Compatible'/>";
					html += "<iframe id='video' src= 'https://www.youtube.com/embed/{0}' frameborder='0' height='205' width='345' allowfullscreen></iframe>";
					html += "</body></html>";

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

					videoContainerFromWeb.NavigateToString(string.Format(html, urlID));

				}
				catch (Exception e)
				{
					string noti = $"\"{url}\"";
					Debug.WriteLine(noti);
					notiMessageSnackbar.MessageQueue.Enqueue(noti, "OK", () => { });
				}
			} 
			else
            {
				try
                {
					videoContainerFromLocal.Visibility = Visibility.Visible;
					videoContainerFromLocal.Source = new Uri(url);
				}
				catch (Exception e)
                {
					string noti = $"Không thể phát video với đường dẫn \"{url}\"";
					Debug.WriteLine(noti);
					notiMessageSnackbar.MessageQueue.Enqueue(noti, "OK", () => { });
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
			CarouselDialog carouselDialog = new CarouselDialog();
			carouselDialog.Show();
		}
	}
}
