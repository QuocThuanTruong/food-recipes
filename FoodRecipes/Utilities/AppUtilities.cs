using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.IO;
using FoodRecipes.Converter;

namespace FoodRecipes.Utilities
{
    public class AppUtilities
    {
        const int MAX_NAME_LENGTH_IN_SPLASH_SCREEN = 53;
        const int MAX_NAME_LENGTH_IN_CARD_VIEW = 26;

        private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();

        /// <summary>
        ///     standardize names into standard form
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isNameInSplashScreen"></param>
        /// <returns>standard string</returns>
        public string getStandardName(string name, bool isNameInSplashScreen) {
            var result = name;
            int maxLength = 0;

            if (isNameInSplashScreen)
            {
                maxLength = MAX_NAME_LENGTH_IN_SPLASH_SCREEN;
                result = result.ToUpper();
            }
            else
            {
                maxLength = MAX_NAME_LENGTH_IN_CARD_VIEW;
            }


            if (result.Length > maxLength)
            {
                result = result.Substring(0, maxLength - 2);
                result += "..";
            }


            return result;
        }

        public string getTypeOfImage(string uriImage) {
            string result = "";

            int index = uriImage.Length - 1;

            while (uriImage[index] != '.') {
                result = uriImage[index--] + result;
            }

            return result;
        }

        public void createIDDirectory(int ID) {
            string path = (string)(_absolutePathConverter.Convert($"Images/{ID}", null, null, null));

            Directory.CreateDirectory(path);
        }

        public void copyImageToIDDirectory(int ID, string srcPath, string nameFile) {
            var destSrc = (string)_absolutePathConverter.Convert($"Images/{ID}/{nameFile}.{getTypeOfImage(srcPath)}", null, null, null);

            File.Copy(srcPath, destSrc);
        }
        
    }
}
