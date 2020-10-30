using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for AboutPage.xaml
	/// </summary>
	public partial class AboutPage : Page
	{
		private ObservableCollection<Tuple<string, string, string>> _memberDetails = new ObservableCollection<Tuple<string, string,string>>();
		public AboutPage()
		{
			InitializeComponent();

			_memberDetails.Add(new Tuple<string, string, string>("QT", Properties.Resources.text_name_qt, Properties.Resources.text_mssv_qt));
			_memberDetails.Add(new Tuple<string, string, string>("HT", Properties.Resources.text_name_ht, Properties.Resources.text_mssv_ht));
			_memberDetails.Add(new Tuple<string, string, string>("NT", Properties.Resources.text_name_nt, Properties.Resources.text_name_nt));

			membersListview.ItemsSource = _memberDetails;
		}
	}
}
