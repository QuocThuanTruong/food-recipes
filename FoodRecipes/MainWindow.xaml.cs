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

			var recipes = _dbUtilitiesInstance.GetAllFromRecipe();

			recipesListView.ItemsSource = recipes;
		}

		private void Load_Recipes_Click(object sender, RoutedEventArgs e)
		{
			var recipes = _dbUtilitiesInstance.GetAllFromRecipe();

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
				var recipes = _dbUtilitiesInstance.GetAllFromRecipe();

				recipesListView.ItemsSource = recipes;
			}
		}

        private void SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
			string name = nameRecipeTextBox.Text;
			string description = descriptionRecipeTextBox.Text;
			string link_video = linkVideoRecipeTextBox.Text;
			string link_avatar = avatarRecipeTextBox.Text;
			string time = timeRecipeTextBox.Text;
			string food_group = groupRecipeTextBox.Text;
			string food_level = levelRecipeTextBox.Text;

			int id = _dbUtilitiesInstance.GetAllFromRecipe().Count() + 1;

			int result = _dbUtilitiesInstance.InsertRecipe(id, name, description, link_video, link_avatar, time, food_group, food_level, false, false);

			string igredientNameRaw = igredientNameRecipeTextBox.Text;
			string[] igredientNames = igredientNameRaw.Split('\n');

			string igredientQuatityRaw = igredientQuatityRecipeTextBox.Text;
			string[] igredientQuatities = igredientQuatityRaw.Split('\n');

			for (int i = 0; i < igredientNames.Length; ++i) {
				_dbUtilitiesInstance.InsertIgredient(id, igredientNames[i], igredientQuatities[i]);
			}
		}

        private void AddIgredient_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddStep_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
