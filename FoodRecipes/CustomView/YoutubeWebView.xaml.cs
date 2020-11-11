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
	/// Interaction logic for YoutubeWebView.xaml
	/// </summary>
	public partial class YoutubeWebView : UserControl
	{

		public YoutubeWebView()
		{
			InitializeComponent();
		}

		public string PlayVideoFromUrl(string url)
		{
			var errorMessage = "";

			try
			{
				string html = "<html><head>";
				html += "<meta content='IE=Edge' http-equiv='X-UA-Compatible'/>";
				html += "<iframe id='video' src= 'https://www.youtube.com/embed/{0}' frameborder='0' height='205' width='345' allowfullscreen></iframe>";
				html += "</body></html>";

				string[] urlParams = url.Split('=');

				string urlID = "";

				if (url.IndexOf("=") != -1)
				{

					string urlParamsIDAndFeture = urlParams[1];
					string[] rawUrl = urlParamsIDAndFeture.Split('&');

					if (rawUrl.Length > 0)
					{
						urlID = rawUrl[0];
					}
					else
					{
						urlID = urlParams[1];
					}
				}
				else
				{
					urlParams = url.Split('/');
					urlID = urlParams[3];
				}

				videoContainerFromWeb.NavigateToString(string.Format(html, urlID));

			}
			catch (Exception e)
			{
				errorMessage = $"Không thể phát video với đường dẫn \"{url}\"";
				Debug.WriteLine(errorMessage);
			}

			return errorMessage;
		}
	}
}
