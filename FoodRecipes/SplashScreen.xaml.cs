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

		private bool _showSplashScreenFlag = true;

		public SplashScreen()
		{
			InitializeComponent();

			int maxID = _dbUtilities.GetMaxIDRecipe();

			if (maxID > 0)
            {
				_showSplashScreenFlag = true;

				int randomIndex = _rng.Next(maxID) + 1;

				Recipe recipe = _dbUtilities.GetRecipeById(randomIndex);
				recipe = _appUtilities.getRecipeForBindingInHomePage(recipe);

				recipe.NAME = _appUtilities.getStandardName(recipe.NAME, true);

				DataContext = recipe;
			}
			else
            {
				_showSplashScreenFlag = false;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (_showSplashScreenFlag)
            {
				//Get splash screen config value
				_configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var appSettingValue = ConfigurationManager.AppSettings["ShowSplashScreen"];
				var isSplashScreenShow = bool.Parse(appSettingValue);
				Debug.WriteLine(isSplashScreenShow);

				////Crop image to fit in opacity mask
				//int cropWidth = 330;
				//int cropHeight = 384;
				//CroppedBitmap cb = null;
				//bool isScale = false;

				//while (recipeAvatarImage.Width < cropWidth || recipeAvatarImage.Height < cropHeight)
				//{
				//	cropWidth = (int) (cropWidth / 2);
				//	cropHeight = (int) (cropHeight / 2);

				//	isScale = true;
				//}	

				//if (cropWidth == 0 || cropHeight == 0)
				//{
				//	cropWidth = 80;
				//	cropHeight = 93;

				//	isScale = true;
				//}	

				//int leftTopCoord = ((int)((recipeAvatarImage.Width - cropWidth) / 2) > 0 ? (int)((recipeAvatarImage.Width - cropWidth) / 2) : 0);
				//int rightBottomCoord = ((int)((recipeAvatarImage.Height - cropHeight) / 2) > 0 ? (int)((recipeAvatarImage.Height - cropHeight) / 2) : 0);

				//try
				//{
				//	cb = new CroppedBitmap((BitmapSource)recipeAvatarImage.Source, new Int32Rect(leftTopCoord, rightBottomCoord, cropWidth, cropHeight));
				//} 
				//catch (Exception exep)
				//{
				//	Debug.WriteLine(exep.Message);
				//}

				//if (cb != null)
				//{
				//	if (isScale)
				//	{
				//		BitmapImage source = new BitmapImage();
				//		source.BeginInit();
				//		source.UriSource = new Uri(cb.Source.ToString());
				//		source.DecodePixelHeight = 384;
				//		source.DecodePixelWidth = 330;
				//		source.EndInit();

				//		recipeAvatarImage.Source = source;
				//	}
				//	else
				//	{
				//		recipeAvatarImage.Source = cb;
				//	}	
				//}
						

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

				//Debug.WriteLine(_timeCounter);
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
