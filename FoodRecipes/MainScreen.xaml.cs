using FoodRecipes.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FoodRecipes.Utilities;

namespace FoodRecipes
{
	/// <summary>
	/// Interaction logic for HomeScreen.xaml
	/// </summary>
	public partial class MainScreen : Window
	{
		private const string DEFAULT_BORDERTHICKNESS = "1";
		private const string NONE_BORDERTHICKNESS = "0";

		private List<Button> _mainScreenButtons;
		public MainScreen()
		{
			InitializeComponent();
			this.WindowState = WindowState.Normal;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			pageNavigation.NavigationService.Navigate(new HomePage());

			_mainScreenButtons = new List<Button>()
			{
				homePageButton, favPageButton, addRecipePageButton, shoppingPageButton, helpPageButton, aboutPageButton
			};

			//Default load page is home page
			DrawerButton_Click(homePageButton, e);
		}

		private void closeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void minimizeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void DrawerButton_Click(object sender, RoutedEventArgs e)
		{
			/** Highlight selected button**/
			var selectedButton = (Button)sender;

			/** Default property of button
			 * <Setter Property="Background" Value="Transparent"/>
			 * <Setter Property="BorderThickness" Value="1"/>**/

			foreach (var button in _mainScreenButtons)
			{
				if (button.Name != selectedButton.Name)
				{
					button.Background = Brushes.Transparent;
					button.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(DEFAULT_BORDERTHICKNESS);
					button.IsEnabled = true;
				}
			}

			//Highlight
			selectedButton.Background = Brushes.White;
			selectedButton.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(NONE_BORDERTHICKNESS);
			selectedButton.IsEnabled = false;
			/****/

			/** Navigating page **/
			pageNavigation.NavigationService.Navigate(getPageFromButton(selectedButton));
		}

		/// <summary>
		/// Return coressponding page with name of selected button
		/// </summary>
		/// <param name="selectedButton"> Current selected button </param>
		/// <returns></returns>
		private Page getPageFromButton(Button selectedButton)
		{
			Page result = new HomePage();

			if (selectedButton.Name == homePageButton.Name)
			{
				result = new HomePage();
				((HomePage)result).ShowRecipeDetailPage += MainScreen_ShowRecipeDetailPage;
			}
			else if (selectedButton.Name == favPageButton.Name)
			{
				result = new HomePage(true);
				((HomePage)result).ShowRecipeDetailPage += MainScreen_ShowRecipeDetailPage;
			}
			else if (selectedButton.Name == addRecipePageButton.Name)
			{
				result = new AddRecipePage();
				((AddRecipePage)result).BackToHome += MainScreen_BackToHome;
			}
			else if (selectedButton.Name == shoppingPageButton.Name)
			{
				result = new ShoppingPage();
			}
			else if (selectedButton.Name == helpPageButton.Name)
			{
				result = new HelpPage();
			}
			else if (selectedButton.Name == aboutPageButton.Name)
			{
				result = new AboutPage();
			}

			return result;
		}

		private void MainScreen_BackToHome()
		{
			pageNavigation.NavigationService.Navigate(getPageFromButton(homePageButton));

			//Clear selected button
			foreach (var button in _mainScreenButtons)
			{
				button.Background = Brushes.Transparent;
				button.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(DEFAULT_BORDERTHICKNESS);
				button.IsEnabled = true;
			}

			homePageButton.Background = Brushes.White;
			homePageButton.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(NONE_BORDERTHICKNESS);
			homePageButton.IsEnabled = false;
		}

		private void MainScreen_ShowRecipeDetailPage(int recipeID)
		{
			var recipeDetailPage = new RecipeDetailPage(recipeID);

			recipeDetailPage.ReloadRecipePage += RecipeDetailPage_ReloadRecipePage;

			recipeDetailPage.GoShopping += RecipeDetailPage_GoShopping;

			pageNavigation.NavigationService.Navigate(recipeDetailPage);

            //Clear selected button
            foreach (var button in _mainScreenButtons)
			{
				button.Background = Brushes.Transparent;
				button.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(DEFAULT_BORDERTHICKNESS);
				button.IsEnabled = true;
			}
		}

		private void RecipeDetailPage_ReloadRecipePage(int recipeID)
		{
			MainScreen_ShowRecipeDetailPage(recipeID);
		}

		private void RecipeDetailPage_GoShopping()
		{
			pageNavigation.NavigationService.Navigate(getPageFromButton(shoppingPageButton));

			//Clear selected button
			foreach (var button in _mainScreenButtons)
			{
				button.Background = Brushes.Transparent;
				button.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(DEFAULT_BORDERTHICKNESS);
				button.IsEnabled = true;
			}

			shoppingPageButton.Background = Brushes.White;
			shoppingPageButton.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(NONE_BORDERTHICKNESS);
			shoppingPageButton.IsEnabled = false;
		}

		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void iconFbButton_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://facebook.com");
		}

		private void iconIgButton_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.instagram.com");
		}

		private void iconYtButton_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com");
		}

		private void iconTTButton_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.tiktok.com");
		}
	}
}
