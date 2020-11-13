using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.IO;
using FoodRecipes.Converter;
using System.Diagnostics;

namespace FoodRecipes.Utilities
{
    public class AppUtilities
    {
        const int MAX_NAME_LENGTH_IN_SPLASH_SCREEN = 53;
        const int MAX_NAME_LENGTH_IN_CARD_VIEW = 15;

        private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();
        private DBUtilities _dbUtilities = DBUtilities.GetDBInstance();

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

            if (Directory.Exists(path))
            {
                Array.ForEach(Directory.GetFiles(path), delegate(string filePath) { File.Delete(filePath); });
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        public void copyImageToIDDirectory(int ID, string srcPath, string nameFile) {
            var destPath = (string)_absolutePathConverter.Convert($"Images/{ID}/{nameFile}.{getTypeOfImage(srcPath)}", null, null, null);

            File.Copy(srcPath, destPath, true);

        }

        public (int hour, int minute) convertTime(int minutes)
        {
            (int hour, int minute) result = (0, 0);

            result.hour = minutes / 60;
            result.minute = minutes % 60;

            return result;
        }

        public string getTimeString((int hour, int minute) time) {
            string result = "";

            if (time.hour > 0)
            {
                result += $"{time.hour}g";
            }
            else
            {
                //Do Nothing
            }

            if (time.minute > 0)
            {
                result += $"{time.minute}ph";
            }
            else
            {
                //Do Nothing
            }

            return result;
        }

        public Recipe getRecipeForBindingInHomePage(Recipe recipe) {
            Recipe result = new Recipe();

            result.ID_RECIPE = recipe.ID_RECIPE;
            result.NAME = recipe.NAME;
            result.DESCRIPTION = recipe.DESCRIPTION;
            result.LINK_VIDEO = recipe.LINK_VIDEO;
            result.LINK_AVATAR = $"Images/{recipe.ID_RECIPE}/avatar.{recipe.LINK_AVATAR}";
            result.TIME = recipe.TIME;
            result.FOOD_GROUP = recipe.FOOD_GROUP;
            result.FOOD_LEVEL = recipe.FOOD_LEVEL;
            result.SHOPPING_FLAG = recipe.SHOPPING_FLAG;
            result.FAVORITE_FLAG = recipe.FAVORITE_FLAG;
            result.ADD_DATE = recipe.ADD_DATE;

            result.NAME_FOR_BINDING = getStandardName(recipe.NAME, false);
            result.TIME_FOR_BINDING = getTimeString(convertTime(recipe.TIME.Value));

            string[] levels = { "Dễ", "TB", "Khó" };
            result.FOOD_LEVEL_FOR_BINDING = levels[recipe.FOOD_LEVEL.Value];

            return result;
        }

        public Recipe getRecipeForBindingInRecipeDetail(Recipe recipe, bool shoppingFlag)
        {
            Recipe result = getRecipeForBindingInHomePage(recipe);

            if (!shoppingFlag)
            {
                result.NAME = result.NAME.ToUpper();
            }
            else
            {
                //Do Nothing
            }

            var igredients = _dbUtilities.GetIgredientByIDRecipe(recipe.ID_RECIPE);
            foreach (var igredient in igredients.ToList())
            {
                Igredient tempIgredient = new Igredient();
                tempIgredient.NAME = igredient.NAME;
                tempIgredient.QUANTITY = igredient.QUANTITY;

                result.Igredients.Add(tempIgredient);
            }

            result.IGREDIENT_LIST_FOR_BINDING = result.Igredients.ToList();

            var steps = _dbUtilities.GetNumericalOrderAndDetailOfStepByIDRecipe(recipe.ID_RECIPE);

            foreach (var tempStep in steps)
            {
                Step step = new Step();
                step.NO_STEP = tempStep.NO_STEP;

                if (step.NO_STEP < 10)
                {
                    step.NO_STEP_FOR_BINDING = $"0{step.NO_STEP}";
                }
                else
                {
                    step.NO_STEP_FOR_BINDING = $"{step.NO_STEP}";
                }

                step.DETAIL = tempStep.DETAIL;

                var allImageInStep = _dbUtilities.GetLinkImageSByIDRecipeAndNOStep(recipe.ID_RECIPE, step.NO_STEP);

                foreach (var imageInStep in allImageInStep)
                {
                    StepImage image = new StepImage();

                    image.ID_RECIPE = recipe.ID_RECIPE;
                    image.NO_STEP = step.NO_STEP;
                    image.LINK_IMAGES = $"Images/{recipe.ID_RECIPE}/{imageInStep.LINK_IMAGES}";

                    step.StepImages.Add(image);
                    result.IMAGES_LIST_FOR_BINDING.Add(image);
                }

                step.STEP_IMAGES_LIST_FOR_BINDING = step.StepImages.ToList();
                result.Steps.Add(step);
            }

            result.STEP_LIST_FOR_BINDING = result.Steps.ToList();

            return result;
        }
    }
}
