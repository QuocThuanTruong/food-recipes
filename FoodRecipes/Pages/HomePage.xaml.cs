using System;
using System.Collections.Generic;
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
	/// Interaction logic for HomePage.xaml
	/// </summary>
	public partial class HomePage : Page
	{
		public string[] SortTypes
		{
			get => new string[] {
				Properties.Resources.text_item_sort_date,
				Properties.Resources.text_item_sort_asc,
				Properties.Resources.text_item_sort_desc,
				Properties.Resources.text_item_sort_group,
				Properties.Resources.text_item_sort_level,
				Properties.Resources.text_item_sort_time
			};
		}


		public HomePage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = this;
		}

		private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
		{

		}
	}
}
