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

		public string PlayVideoFromUri(string uri)
		{
			var errorMessage = "";

			try
			{
				videoContainerFromLocal.Source = new Uri(uri);
			}
			catch (Exception e)
			{
				errorMessage = $"Không thể phát video với đường dẫn \"{uri}\"";
				Debug.WriteLine(errorMessage);
			}

			return errorMessage;
		}
	}
}
