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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FoodRecipes.DBUtilities;

namespace FoodRecipes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private DBUtilities.DBUtilities _dbUtilitiesInstance;

		public MainWindow()
		{
			InitializeComponent();

			Init();
		}

		public void Init() {
			_dbUtilitiesInstance = DBUtilities.DBUtilities.GetDBInstance();
		}

		private void Load_Recipes_Click(object sender, RoutedEventArgs e)
		{
			var recipes = _dbUtilitiesInstance.GetAllRecipeSummary();

			recipesListView.ItemsSource = recipes;
		}

        private void searchKeyWord_TextChanged(object sender, TextChangedEventArgs e)
        {
			string search_text = searchKeyWordTextBox.Text;

			if (search_text.Length != 0)
			{
				var recipesSearchResults = _dbUtilitiesInstance.GetRecipesSearchResult(search_text);

				if (recipesSearchResults.Count > 0) {
					recipesListView.ItemsSource = recipesSearchResults;
				}
				else {
					recipesListView.ItemsSource = null;
				}

			} else {
				recipesListView.ItemsSource = null;
			}
		}
    }
}
