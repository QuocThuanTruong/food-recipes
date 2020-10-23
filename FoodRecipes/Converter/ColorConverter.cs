using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FoodRecipes.Converter
{
	class ColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string colorCode = "#000000";
			string color = (string)value;
			var brushConverter = new BrushConverter();

			if (color == "myRed")
			{
				colorCode = "#d92027";
			} 
			else if (color == "myOrange")
			{
				colorCode = "#fe9e0c";
			}
			else if (color == "myYellow")
			{
				colorCode = "#ffcd3c";
			}
			else if (color == "myBlue")
			{
				colorCode = "#172975";
			}
			else if (color == "myBlack")
			{
				colorCode = "#303030";
			}

			var solidBrush = (SolidColorBrush)brushConverter.ConvertFrom(colorCode);

			return solidBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
