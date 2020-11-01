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
using FoodRecipes.Utilities;
using FoodRecipes.Converter;

namespace FoodRecipes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SplashScreen : Window
	{
		#region Private Fields
		private Timer _loadingTmer;
		private Configuration _configuration;
		private int _timeCounter = 0;

		private const int TIME_LOAD_UNIT = 1000;
		private const int TOTAL_TIME_LOAD_IN_SECOND = 5;
		#endregion

		private DBUtilities _dbUtilities = DBUtilities.GetDBInstance();
		private AppUtilities _appUtilities = new AppUtilities();
		private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();
		private Random _rng = new Random();

		public SplashScreen()
		{
			InitializeComponent();

			int maxID = _dbUtilities.GetMaxIDRecipe();
			int randomIndex = _rng.Next(maxID) + 1;

			GetRecipeById_Result recipe = _dbUtilities.GetRecipeById(randomIndex).FirstOrDefault();

			recipe.NAME = _appUtilities.getStandardName(recipe.NAME, true);
			recipe.LINK_AVATAR = (string)_absolutePathConverter.Convert($"Images/{randomIndex}/ avatar.{recipe.LINK_AVATAR}", null, null, null);

			DataContext = recipe;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//Get splash screen config value
			_configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var appSettingValue = ConfigurationManager.AppSettings["ShowSplashScreen"];
			var isSplashScreenShow = bool.Parse(appSettingValue);
			Debug.WriteLine(isSplashScreenShow);

			if (isSplashScreenShow)
			{
				_loadingTmer = new Timer(TIME_LOAD_UNIT);
				_loadingTmer.Elapsed += LoadingTmer_Elapsed;
				_loadingTmer.Start();
			}
			else
			{
				showMainScreen();
			}
		}

		/// <summary>
		/// Show home screen and close splash screen
		/// </summary>
		private void showMainScreen()
		{
			var homeScreen = new MainScreen();

			this.Hide();
			homeScreen.Show();
			this.Close();
		}

		private void LoadingTmer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				_timeCounter++;

				if (_timeCounter == TOTAL_TIME_LOAD_IN_SECOND)
				{
					_loadingTmer.Stop();
					showMainScreen();
				}

				Debug.WriteLine(_timeCounter);
			});
		}

		private void turnOffSplashCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			_configuration.AppSettings.Settings["ShowSplashScreen"].Value = "False";
			_configuration.Save(ConfigurationSaveMode.Minimal);
		}

		private void turnOffSplashCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			_configuration.AppSettings.Settings["ShowSplashScreen"].Value = "True";
			_configuration.Save(ConfigurationSaveMode.Minimal);
		}

	}
}
