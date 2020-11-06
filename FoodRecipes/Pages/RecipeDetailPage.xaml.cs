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
		public RecipeDetailPage()
		{
			InitializeComponent();
		}

		public RecipeDetailPage(int recipeID)
		{
			InitializeComponent();

			var recipeGetRecipeById = _dbUtilities.GetRecipeById(recipeID).FirstOrDefault();

			Recipe recipe = new Recipe();

			recipe.ID_RECIPE = recipeGetRecipeById.ID_RECIPE;
			recipe.NAME = recipeGetRecipeById.NAME;
			recipe.TIME = recipeGetRecipeById.TIME;
			recipe.FOOD_GROUP = recipeGetRecipeById.FOOD_GROUP;
			recipe.FOOD_LEVEL = recipeGetRecipeById.FOOD_LEVEL;
			recipe.DESCRIPTION = recipeGetRecipeById.DESCRIPTION;
			recipe.LINK_AVATAR = recipeGetRecipeById.LINK_AVATAR;
			recipe.LINK_AVATAR = (string)(_absolutePathConverter.Convert($"Images/{recipe.ID_RECIPE}/avatar.{recipe.LINK_AVATAR}", null, null, null));
			recipe.LINK_VIDEO = recipeGetRecipeById.LINK_VIDEO;
			recipe.ADD_DATE = recipeGetRecipeById.ADD_DATE;
			recipe.FAVORITE_FLAG = recipeGetRecipeById.FAVORITE_FLAG;
			recipe.SHOPPING_FLAG = recipeGetRecipeById.SHOPPING_FLAG;

			var igredients = _dbUtilities.GetIgredientByIDRecipe(recipeID);
			foreach (var igredient in igredients.ToList())
			{
				Igredient tempIgredient = new Igredient();
				tempIgredient.NAME = igredient.NAME;
				tempIgredient.QUANTITY = igredient.QUANTITY;

				recipe.Igredients.Add(tempIgredient);
			}

			recipe.IGREDIENT_LIST_FOR_BINDING = recipe.Igredients.ToList();

			var steps = _dbUtilities.GetNumericalOrderAndDetailOfStepByIDRecipe(recipeID);

			foreach (var tempStep in steps)
			{
				Step step = new Step();
				step.NO_STEP = tempStep.NO_STEP;

				if (step.NO_STEP < 10)
				{
					step.NO_STEP_FOR_BINDING = $"0{step.NO_STEP}";
				}
				else
				{
					step.NO_STEP_FOR_BINDING = $"{step.NO_STEP}";
				}

				step.DETAIL = tempStep.DETAIL;

				var allImageInStep = _dbUtilities.GetLinkImageSByIDRecipeAndNOStep(recipeID, step.NO_STEP);

				foreach (var imageInStep in allImageInStep)
				{
					StepImage image = new StepImage();

					image.ID_RECIPE = recipeID;
					image.NO_STEP = step.NO_STEP;
					image.LINK_IMAGES = $"Images/{recipeID}/{imageInStep.LINK_IMAGES}";
					Debug.WriteLine(image.LINK_IMAGES);

					step.StepImages.Add(image);
					recipe.IMAGES_LIST_FOR_BINDING.Add(image);
				}

				step.STEP_IMAGES_LIST_FOR_BINDING = step.StepImages.ToList();
				recipe.Steps.Add(step);
			}

			recipe.STEP_LIST_FOR_BINDING = recipe.Steps.ToList();

			recipe.LINK_VIDEO = recipeGetRecipeById.LINK_VIDEO;
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

		private void imageRecipeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine(imageRecipeListView.SelectedIndex);
			CarouselDialog carouselDialog = new CarouselDialog();
			carouselDialog.Show();
		}
	}
}
