using FoodRecipes.Pages;
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
using System.Windows.Shapes;

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

		private void maximizeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.WindowState == WindowState.Maximized)
			{
				this.WindowState = WindowState.Normal;
				iconMaximizeImage.Source = new BitmapImage(new Uri(FindResource("IconMaximize").ToString()));
				iconMaximizeImage.ToolTip = Properties.Resources.tip_maximize_window_button;
			}
			else
			{
				this.WindowState = WindowState.Maximized;
				iconMaximizeImage.Source = new BitmapImage(new Uri(FindResource("IconRestoreDown").ToString()));
				iconMaximizeImage.ToolTip = Properties.Resources.tip_restore_window_button;
			}
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
				}
			}

			//Highlight
			selectedButton.Background = Brushes.White;
			selectedButton.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromString(NONE_BORDERTHICKNESS);
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
			}
			else if (selectedButton.Name == favPageButton.Name)
			{
				result = new HomePage(true);
			}
			else if (selectedButton.Name == addRecipePageButton.Name)
			{
				result = new AddRecipePage();
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

	}
}
