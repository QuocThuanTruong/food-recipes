using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
	/// Interaction logic for HelpPage.xaml
	/// </summary>
	public partial class HelpPage : Page
	{
		private ObservableCollection<Tuple<string, string>> _howToUsePages = new ObservableCollection<Tuple<string, string>>();
		
		public HelpPage()
		{
			InitializeComponent();
			
			_howToUsePages.Add(new Tuple<string, string>(Properties.Resources.home_page_upper, Properties.Resources.text_help_home));
			_howToUsePages.Add(new Tuple<string, string>(Properties.Resources.fav_page_upper, Properties.Resources.text_help_fav));
			_howToUsePages.Add(new Tuple<string, string>(Properties.Resources.add_recipe_page_upper, Properties.Resources.text_help_add_recipe));
			_howToUsePages.Add(new Tuple<string, string>(Properties.Resources.shopping_page_upper, Properties.Resources.text_help_shopping));
			_howToUsePages.Add(new Tuple<string, string>(Properties.Resources.help_page_upper, Properties.Resources.text_help_help));
			_howToUsePages.Add(new Tuple<string, string>(Properties.Resources.about_page_upper, Properties.Resources.text_help_about));

			helpDetailListView.ItemsSource = _howToUsePages;

		}

		private void linkVideoTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			System.Diagnostics.Process.Start("https://youtu.be/Rzg9LGXxTtI");
		}
	}
}
