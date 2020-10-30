using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	/// Interaction logic for AddRecipePage.xaml
	/// </summary>
	public partial class AddRecipePage : Page
	{
		public delegate void BackToHomeHandler();
		public event BackToHomeHandler BackToHome;
		public AddRecipePage()
		{
			InitializeComponent();

			//Show snack bar
			notiMessageSnackbar.MessageQueue.Enqueue("Test", "BACK HOME", () => { BackHome(); });
		}

		private void BackHome()
		{
			BackToHome?.Invoke();
		}

		private void avatarImagePickerButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void levelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void groupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void hourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void hourTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			//Regex meaning: not match any number digit zero or many times
			var pattern = "[^0-9]+";
			var regex = new Regex(pattern);

			//if true -> input event has handled (skiped this character)
			e.Handled = regex.IsMatch(e.Text);
		}

		private void addStepButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void cancelAddRecipeButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void saveRecipeButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void avatarPickerFrameButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
