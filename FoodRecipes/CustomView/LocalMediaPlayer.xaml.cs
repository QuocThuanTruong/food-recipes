using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
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
		public double CurrentTime { get; set; }
		public bool IsPlay { get; set; }
		public bool IsMute { get; set; }
		public double CurrentVolume { get; set; }
		public bool IsHideController { get; set; } = true;

		public delegate void FullScreenClickHandler(bool isFullScreen); //, double currentTime, double startTime, double endTime, bool isPlay, bool isMute, double currentVolume);
		public event FullScreenClickHandler FullScreenClick;

		private Timer _loadingTmer;
		private Timer _loadFrameTimer;

		private const int TIME_LOAD_UNIT = 1000;

		private bool _endVideo = false;
		private bool _isChangedPosition = false;
		private bool _isFirstTimePlay = true;

		public LocalMediaPlayer()
		{
			InitializeComponent();
		}

		public bool PlayVideoFromUri(string uri)
		{
			var isSuccessful = true;

			try
			{
				videoContainerFromLocal.Source = new Uri(uri);
			}
			catch (Exception e)
			{
				isSuccessful = false;
				Debug.WriteLine(e.Message);
			}

			InitControl();

			return isSuccessful;
		}

		private void InitControl()
		{
			_loadFrameTimer = new Timer(TIME_LOAD_UNIT/2);
			_loadFrameTimer.Elapsed += _loadFrameTimer_Elapsed;

			videoContainerFromLocal.IsMuted = true;
			videoContainerFromLocal.Play();
			_loadFrameTimer.Start();
			videoContainerFromLocal.IsMuted = false;

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

			if (IsHideController)
			{
				mediaControlContainer.Visibility = Visibility.Hidden;
			}
			else
			{
				mediaControlContainer.Visibility = Visibility.Visible;
			}				

			videoContainerFromLocal.Volume = CurrentVolume;
			volumeSlider.Value = CurrentVolume;

			videoContainerFromLocal.IsMuted = IsMute;

			videoProgressSlider.Value = CurrentTime;

			_loadingTmer = new Timer(TIME_LOAD_UNIT);
			_loadingTmer.Elapsed += LoadingTmer_Elapsed;

			if (IsFullScreen)
            {
				if (IsPlay)
				{
					_isFirstTimePlay = false;

					var SliderValue = videoProgressSlider.Value;
					TimeSpan ts = new TimeSpan(0, 0, 0, (int)SliderValue, 0);

					videoContainerFromLocal.Position = ts;

					videoContainerFromLocal.Play();

					_loadingTmer.Start();
				}
				else
                {
					//Do Nothing
                }
			}
			else
            {
				//Do Nothing
            }			
		}

		private void _loadFrameTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				videoContainerFromLocal.Pause();
				iconPause.Source = new BitmapImage(new Uri(FindResource("IconBlueNext").ToString()));
				_loadFrameTimer.Stop();
			});
		}

		private void videoContainerFromLocal_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			MessageBox.Show(e.ErrorException.Message);
		}

        private void pauseButon_Click(object sender, RoutedEventArgs e)
		{
			if (_endVideo)
            {
				_endVideo = false;

				videoProgressSlider.Value = 0;
            }

			else
            {
				//Do nothing
            }

			if (IsPlay)
			{
				iconPause.Source = new BitmapImage(new Uri(FindResource("IconBlueNext").ToString()));
				
				videoContainerFromLocal.Pause();

				_loadingTmer.Stop();
			}
			else
			{
				if (_isFirstTimePlay)
				{
					var SliderValue = videoProgressSlider.Value;

					Debug.WriteLine(SliderValue);

					TimeSpan ts = new TimeSpan(0, 0, 0, (int)SliderValue, 0);

					videoContainerFromLocal.Position = ts;

					_isFirstTimePlay = false;
				}

				iconPause.Source = new BitmapImage(new Uri(FindResource("IconBluePause").ToString()));

				videoContainerFromLocal.Play();

				_loadingTmer.Start();
			}

			IsPlay = !IsPlay;
		}

		private void LoadingTmer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				if (!_isChangedPosition)
				{
					++videoProgressSlider.Value; // = CurrentTime;
				}
				else
                {
					//Do Nothing
                }
			});
		}

        private void muteButton_Click(object sender, RoutedEventArgs e)
		{
			IsMute = !IsMute;
			videoContainerFromLocal.IsMuted = IsMute;

			if (IsMute)
			{
				iconMute.Source = new BitmapImage(new Uri(FindResource("IconBlueMute").ToString()));
			}
			else
			{
				iconMute.Source = new BitmapImage(new Uri(FindResource("IconBlueSoundOn").ToString()));
			}
		}

		private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			videoContainerFromLocal.Volume = (double)volumeSlider.Value;

			Debug.WriteLine(videoContainerFromLocal.Volume);
		}

		private void fullScreenButton_Click(object sender, RoutedEventArgs e)
		{
			IsFullScreen = IsFullScreen == true ? false : true;

			CurrentTime = videoProgressSlider.Value;
			CurrentVolume = videoContainerFromLocal.Volume;

			videoContainerFromLocal.Pause();
			FullScreenClick?.Invoke(IsFullScreen);
		}

		private void videoContainerFromLocal_Opened(object sender, RoutedEventArgs e)
		{
			videoProgressSlider.Maximum = videoContainerFromLocal.NaturalDuration.TimeSpan.TotalSeconds;

			//EndTime = videoProgressSlider.Maximum;
		}

		// When the media playback is finished. Stop() the media to seek to media start.
		private void videoContainerFromLocal_Ended(object sender, EventArgs e)
		{
			videoContainerFromLocal.Stop();

			_loadingTmer.Stop();

			iconPause.Source = new BitmapImage(new Uri(FindResource("IconBlueNext").ToString()));

			IsPlay = false;

			_endVideo = true;
		}

        private void videoProgressSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
			if (!_isFirstTimePlay)
            {
				var SliderValue = videoProgressSlider.Value;
				TimeSpan ts = new TimeSpan(0, 0, 0, (int)SliderValue, 0);

				if (IsPlay)
				{
					videoContainerFromLocal.Pause();
					videoContainerFromLocal.Position = ts;
					videoContainerFromLocal.Play();
				}
				else
				{
					videoContainerFromLocal.Position = ts;
				}
			}
		}

		public void playUnfinishedVideo()
		{
			_isFirstTimePlay = false;

			var SliderValue = videoProgressSlider.Value;
			TimeSpan ts = new TimeSpan(0, 0, 0, (int)SliderValue, 0);

			videoContainerFromLocal.Position = ts;

			videoContainerFromLocal.Play();

			_loadingTmer.Start();
		}
	}
}
