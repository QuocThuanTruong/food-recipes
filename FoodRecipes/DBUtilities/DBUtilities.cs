using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecipes.DBUtilities
{
    class DBUtilities
    {
        private DBUtilities() { }

        private static DBUtilities _dbInstance;
        private static FoodRecipeEntities _dbFoodRecipe;

        public static DBUtilities GetDBInstance() {
            if (_dbInstance == null) {
                _dbInstance = new DBUtilities();
                _dbFoodRecipe = new FoodRecipeEntities();
            } else {
                //Do Nothing
            }

            return _dbInstance;
        }

        public IQueryable<GetRecipeById_Result> GetRecipeById(Nullable<int> id_recipe) {
            var result = _dbFoodRecipe.GetRecipeById(id_recipe);

            return result;
        }

        public IQueryable<GetAllRecipeSummary_Result> GetAllRecipeSummary() {
            var result = _dbFoodRecipe.GetAllRecipeSummary();

            return result;
        }

        public IQueryable<GetAllFromRecipe_Result> GetAllFromRecipe() {
            var result = _dbFoodRecipe.GetAllFromRecipe();

            return result;
        }

        public IQueryable<GetIDRecipeByFavoriteFlag_Result> GetIDRecipeByFavoriteFlag(Nullable<bool> favorite_flag) {
            var result = _dbFoodRecipe.GetIDRecipeByFavoriteFlag(favorite_flag);

            return result;
        }

        public IQueryable<GetIDRecipeByFoodGroup_Result> GetIDRecipeByFoodGroup(string food_group) {
            var result = _dbFoodRecipe.GetIDRecipeByFoodGroup(food_group);

            return result;
        }

        public IQueryable<GetIDRecipeByFoodLevel_Result> GetIDRecipeByFoodLevel(string food_level) {
            var result = _dbFoodRecipe.GetIDRecipeByFoodLevel(food_level);

            return result;
        }

        public IQueryable<GetIDRecipeByShoppingFlag_Result> GetIDRecipeByShoppingFlag(Nullable<bool> shopping_flag) {
            var result = _dbFoodRecipe.GetIDRecipeByShoppingFlag(shopping_flag);

            return result;
        }

        public IQueryable<GetIDRecipeByTime_Result> GetIDRecipeByTime(string time) {
            var result = _dbFoodRecipe.GetIDRecipeByTime(time);

            return result;
        }

        public virtual IQueryable<GetIgredientByIDRecipe_Result> GetIgredientByIDRecipe(Nullable<int> id_recipe) {
            var result = _dbFoodRecipe.GetIgredientByIDRecipe(id_recipe);

            return result;
        }

        public IQueryable<GetLinkImageSByIDRecipeAndNOStep_Result> GetLinkImageSByIDRecipeAndNOStep(Nullable<int> id_recipe, Nullable<int> nO_step) {
            var result = _dbFoodRecipe.GetLinkImageSByIDRecipeAndNOStep(id_recipe, nO_step);

            return result;
        }

        public IQueryable<GetNumericalOrderAndDetailOfStepByIDRecipe_Result> GetNumericalOrderAndDetailOfStepByIDRecipe(Nullable<int> id_recipe) {
            var result = _dbFoodRecipe.GetNumericalOrderAndDetailOfStepByIDRecipe(id_recipe);

            return result;
        }

        public int InsertRecipe(Nullable<int> id_recipe,
                                string name,
                                string description,
                                string link_video,
                                string link_avatar,
                                string time,
                                string food_group,
                                string food_level,
                                Nullable<bool> shopping_flag,
                                Nullable<bool> favorite_flag)
        {
            var result = _dbFoodRecipe.InsertRecipe(id_recipe, name, description, link_video, link_avatar, time, food_group, food_level, shopping_flag, favorite_flag);

            return result;
        }

        public int InsertIgredient(Nullable<int> id_recipe, string name, string quantity) {
            var result = _dbFoodRecipe.InsertIgredient(id_recipe, name, quantity);

            return result;
        }

        public int InsertStep(Nullable<int> id_recipe, Nullable<int> no_step, string detail) {
            var result = _dbFoodRecipe.InsertStep(id_recipe, no_step, detail);

            return result;
        }

        public int InsertStepImages(Nullable<int> id_recipe, Nullable<int> no_step, string link_images) {
            var result = _dbFoodRecipe.InsertStepImages(id_recipe, no_step, link_images);

            return result;
        }

        public int TurnFavoriteFlagOff(Nullable<int> id_recipe) {
            var result = _dbFoodRecipe.TurnFavoriteFlagOff(id_recipe);

            return result;
        }

        public int TurnFavoriteFlagOn(Nullable<int> id_recipe)
        {
            var result = _dbFoodRecipe.TurnFavoriteFlagOn(id_recipe);

            return result;
        }

        public int TurnShoppingFlagOff(Nullable<int> id_recipe)
        {
            var result = _dbFoodRecipe.TurnShoppingFlagOff(id_recipe);

            return result;
        }

        public int TurnShoppingFlagOn(Nullable<int> id_recipe)
        {
            var result = _dbFoodRecipe.TurnShoppingFlagOn(id_recipe);

            return result;
        }

        public IQueryable<SearchByName_Result> SearchByName(string search_text) {
            var result = _dbFoodRecipe.SearchByName(search_text);

            return result;
        }

        public List<GetAllRecipeSummary_Result> GetRecipesSearchResult(string search_text) {
            var recipesSummary = GetAllRecipeSummary();

            var recipesSearchResult = SearchByName(search_text).OrderByDescending(r => r.RANK);

            List<GetAllRecipeSummary_Result> result = new List<GetAllRecipeSummary_Result>();

            if (recipesSearchResult.Any()) {
                foreach (var recipeSearchResult in recipesSearchResult)
                {
                    var recipe = from r in recipesSummary
                                 where r.ID_RECIPE == recipeSearchResult.ID_RECIPE
                                 select r;

                    result.Add(recipe.FirstOrDefault());
                }
            } else {

                //Do Nothing 

            }

            return result;
        }
    }
}
