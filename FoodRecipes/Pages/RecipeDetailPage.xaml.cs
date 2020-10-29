using System;
using System.Collections.Generic;
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

namespace FoodRecipes.Pages
{
	/// <summary>
	/// Interaction logic for RecipeDetailPage.xaml
	/// </summary>
	public partial class RecipeDetailPage : Page
	{
		public RecipeDetailPage()
		{
			InitializeComponent();
		}

		public RecipeDetailPage(int recipeID)
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
		{
			notiMessageSnackbar.IsActive = false;
		}
	}
}
