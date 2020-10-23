using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

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

        public int GetMaxIDRecipe()
        {
           var result = _dbFoodRecipe
                .Database
                .SqlQuery<int>("select [dbo].[GetMaxIDRecipe]()")
                .Single();

            return result;
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
            List<GetAllRecipeSummary_Result> result = new List<GetAllRecipeSummary_Result>();

            try
            {
                string[] OPERATOR = { "and", "or", "and not" };

                search_text = GetStandardString(search_text);
                Stack<string> keywords = GetListKeyWords(search_text);
                Queue<int> operators = GetListOperator(search_text);

                var recipesSummary = GetAllRecipeSummary();

                if (keywords.Count < 0)
                {
                    search_text = search_text.Replace("\"", "#");
                }
                else if (operators.Count > 0)
                {
                    HashSet<int> tempIDsResult = new HashSet<int>();
                    HashSet<int> deathID = new HashSet<int>();

                    int count = 1;

                    while (operators.Count > 0)
                    {
                        var operatorStr = OPERATOR[operators.Dequeue() - 1];

                        List<string> params1 = new List<string>();

                        while (count > 0)
                        {
                            params1.Add(keywords.Pop());
                            --count;
                        }

                        string param2 = keywords.Pop();

                        HashSet<string> tempKeyWords = new HashSet<string>();

                        foreach (var param1 in params1)
                        {
                            if (operatorStr == "and not") {
                                string deathSearchText = param1 + " " + "and" + " " + param2;

                                var tempRecipesSearchResultDeath = SearchByName(deathSearchText);

                                foreach (var tempRecipeSearchResultDeath in tempRecipesSearchResultDeath)
                                {
                                    var recipe = from r in recipesSummary
                                                 where r.ID_RECIPE == tempRecipeSearchResultDeath.ID_RECIPE
                                                 select r;

                                    deathID.Add(recipe.FirstOrDefault().ID_RECIPE);
                                }
                            }

                            string tempSearchText = param1 + " " + operatorStr + " " + param2;

                            var tempRecipesSearchResult = SearchByName(tempSearchText);

                            count += tempRecipesSearchResult.Count();

                            foreach (var tempRecipeSearchResult in tempRecipesSearchResult)
                            {
                                var recipe = from r in recipesSummary
                                             where r.ID_RECIPE == tempRecipeSearchResult.ID_RECIPE
                                             select r;

                                if (operators.Count == 0)
                                {
                                    tempIDsResult.Add(recipe.FirstOrDefault().ID_RECIPE);
                                }
                                else {
                                    tempKeyWords.Add("\"" + recipe.FirstOrDefault().NAME + "\"");
                                }
                              
                            }

                            while (tempKeyWords.Count > 0) {
                                keywords.Push(tempKeyWords.First());

                                tempKeyWords.Remove(tempKeyWords.First());
                            }
                        }
                    }

                    while (tempIDsResult.Count > 0) {
                        int tempID = tempIDsResult.First();

                        tempIDsResult.Remove(tempID);

                        if (!deathID.Contains(tempID))
                        {
                            var recipe = from r in recipesSummary
                                         where r.ID_RECIPE == tempID
                                         select r;

                            result.Add(recipe.FirstOrDefault());
                        }
                        else { 
                            //Do Nothing
                        }

                    }

                }
                else {
                    var recipesSearchResult = SearchByName(search_text).OrderByDescending(r => r.RANK);

                    foreach (var recipeSearchResult in recipesSearchResult)
                    {
                        var recipe = from r in recipesSummary
                                     where r.ID_RECIPE == recipeSearchResult.ID_RECIPE
                                     select r;

                        result.Add(recipe.FirstOrDefault());
                    }
                } 
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.InnerException);
            }
           

            return result;
        }

        private string GetStandardString(string srcString)
        {
            string result = srcString;

            result = result.Trim();

            while (result.IndexOf("  ") != -1)
            {
                result = result.Replace("  ", " ");
            }

            result = result.ToLower();

            return result;
        }

        private Stack<string> GetListKeyWords(string search_text) {
            Stack<string> result = new Stack<string>();
            Stack<string> temp = new Stack<string>();

            List<int> indexQuotes = new List<int>();

            for (int i = 0; i < search_text.Length; ++i)
            {
                if (search_text[i] == '"')
                {
                    indexQuotes.Add(i);
                }
                else
                {
                    //Do Nothing
                }
            }

            if (indexQuotes.Count % 2 == 0) {
                for (int i = 0; i < indexQuotes.Count - 1; i += 2)
                {
                    string tempString = "";

                    for (int j = indexQuotes[i]; j <= indexQuotes[i + 1]; ++j) {
                        tempString += search_text[j];
                    }

                    //"abc" and "123"
                    if (tempString.Length > 2) {
                        temp.Push(tempString);
                    }
                }
            }

            while (temp.Count > 0) {
                result.Push(temp.Pop());
            }

            return result;
        }

        private Queue<int> GetListOperator(string search_text) {
            Queue<int> result = new Queue<int>();

            var tokens = search_text.Split(' ');

            for (int i = 0; i < tokens.Length; ++i) {
                if (tokens[i] == "and")
                {
                    if (i + 1 < tokens.Length && tokens[i + 1] == "not")
                    {
                        result.Enqueue(3);
                    }
                    else
                    {
                        result.Enqueue(1);
                    }
                }
                else if (tokens[i] == "or") {
                    result.Enqueue(2);
                }
            }
            return result;
        }
    }
}
