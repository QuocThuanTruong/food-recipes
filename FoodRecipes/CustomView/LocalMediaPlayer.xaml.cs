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

namespace FoodRecipes.CustomView
{
	/// <summary>
	/// Interaction logic for LocalMediaPlayer.xaml
	/// </summary>
	public partial class LocalMediaPlayer : UserControl
	{
		public LocalMediaPlayer()
		{
			InitializeComponent();
		}

		public string PlayeVideoFromUrl(string url)
		{
			var errorMessage = "";

			try
			{
				videoContainerFromLocal.Visibility = Visibility.Visible;
				videoContainerFromLocal.Source = new Uri(url);
			}
			catch (Exception e)
			{
				errorMessage = $"Không thể phát video với đường dẫn \"{url}\"";
				Debug.WriteLine(errorMessage);
			}

			return errorMessage;
		}

		private void play_Click(object sender, RoutedEventArgs e)
		{
			videoContainerFromLocal.Play();
		}
	}
}
