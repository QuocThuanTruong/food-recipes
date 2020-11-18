using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace FoodRecipes.CustomView
{
	/// <summary>
	/// Interaction logic for FullScreenVideoDialog.xaml
	/// </summary>
	public partial class FullScreenVideoDialog : UserControl
	{
		private bool _hideRequest = false;
		private UIElement _parent;

		public delegate void CloseFullScreenVideoDialogHandler();
		public event CloseFullScreenVideoDialogHandler CloseFullScreenVideoDialog;

		public FullScreenVideoDialog()
		{
			InitializeComponent();
			Visibility = Visibility.Collapsed;
		}

		public void SetParent(UIElement parent)
		{
			_parent = parent;
		}

		//Params will define depend on your need
		public void ShowDialog(string url)
		{
			localMediaPlayer.IsHideController = false;
			localMediaPlayer.IsPlay = true;
			localMediaPlayer.IsMute = false;

			localMediaPlayer.PlayVideoFromUri(url);

			_parent.IsEnabled = false;
			_hideRequest = false;

			Visibility = Visibility.Visible;

			while (!_hideRequest)
			{
				//Stop if app close
				if (this.Dispatcher.HasShutdownStarted || this.Dispatcher.HasShutdownFinished)
				{
					break;
				}

				this.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate {}));
				Thread.Sleep(20);
			}
		}

		public void HideDialog()
		{
			_hideRequest = true;
			Visibility = Visibility.Collapsed;
			_parent.IsEnabled = true;
		}

		private void closeDialogButton_Click(object sender, RoutedEventArgs e)
		{
			HideDialog();
			localMediaPlayer.IsFullScreen = false;
			CloseFullScreenVideoDialog?.Invoke();
		}
	}
}
