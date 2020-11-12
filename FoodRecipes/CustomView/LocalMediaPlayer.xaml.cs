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
		public bool IsFullScreen { get; set; } = false;
		public long CurrentTime { get; set; } = 0;
		public long StartTime { get; set; } = 0;
		public long EndTime { get; set; } = 100;
		public bool IsPlay { get; set; } = true;
		public bool IsMute { get; set; } = false;
		public long CurrentVolume { get; set; } = 100;

		public delegate void FullScreenClickHandler(bool isFullScreen);
		public event FullScreenClickHandler FullScreenClick;

		public LocalMediaPlayer()
		{
			InitializeComponent();
		}

		public string PlayVideoFromUri(string uri)
		{
			var errorMessage = "";

			InitControl();

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

		private void InitControl()
		{
			if (IsFullScreen)
			{
				iconFullScreen.Source = new BitmapImage(new Uri(FindResource("IconBlueExitFullScr").ToString()));
			}
			else
			{
				iconFullScreen.Source = new BitmapImage(new Uri(FindResource("IconBlueFullScr").ToString()));
			}

			if (IsPlay)
			{
				iconPause.Source = new BitmapImage(new Uri(FindResource("IconBluePause").ToString()));
			}
			else
			{
				iconPause.Source = new BitmapImage(new Uri(FindResource("IconBlueNext").ToString()));
			}

			if (IsMute)
			{
				iconMute.Source = new BitmapImage(new Uri(FindResource("IconBlueMute").ToString()));
			}
			else
			{
				iconMute.Source = new BitmapImage(new Uri(FindResource("IconBlueSoundOn").ToString()));
			}
		}

		private void videoContainerFromLocal_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			MessageBox.Show(e.ErrorException.Message);
		}

		private void videoProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void pauseButon_Click(object sender, RoutedEventArgs e)
		{

		}

		private void muteButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void fullScreenButton_Click(object sender, RoutedEventArgs e)
		{
			IsFullScreen = IsFullScreen == true ? false : true;

			FullScreenClick?.Invoke(IsFullScreen);
		}
	}
}
