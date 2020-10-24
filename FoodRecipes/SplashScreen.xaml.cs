using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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
using System.Configuration;
using System.Timers;
using System.Diagnostics;

namespace FoodRecipes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SplashScreen : Window
	{
		#region Private Fields
		private Timer loadingTmer;
		private Configuration configuration;
		private int timeCounter = 0;
		private string isShowSplashValue = "True";

		private const int TIME_LOAD_UNIT = 1000;
		private const int TOTAL_TIME_LOAD_IN_SECOND = 5;
		#endregion

		public SplashScreen()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//Get splash screen config value
			configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var appSettingValue = ConfigurationManager.AppSettings["ShowSplashScreen"];
			var isSplashScreenShow = bool.Parse(appSettingValue);
			Debug.WriteLine(isSplashScreenShow);

			if (isSplashScreenShow)
			{
				loadingTmer = new Timer(TIME_LOAD_UNIT);
				loadingTmer.Elapsed += LoadingTmer_Elapsed;
				loadingTmer.Start();
			}
			else
			{
				showHomeScreen();
			}
		}

		/// <summary>
		/// Show home screen and close splash screen
		/// </summary>
		private void showHomeScreen()
		{
			var homeScreen = new HomeScreen();

			this.Close();
			homeScreen.Show();	
		}

		private void LoadingTmer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				timeCounter++;

				if (timeCounter == TOTAL_TIME_LOAD_IN_SECOND)
				{
					loadingTmer.Stop();
					showHomeScreen();
				}

				Debug.WriteLine(timeCounter);
			});
		}

		private void turnOffSplashCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			isShowSplashValue = "False";
		}

		private void turnOffSplashCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			isShowSplashValue = "True";
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			configuration.AppSettings.Settings["ShowSplashScreen"].Value = isShowSplashValue;
			configuration.Save(ConfigurationSaveMode.Minimal);
		}
	}
}
