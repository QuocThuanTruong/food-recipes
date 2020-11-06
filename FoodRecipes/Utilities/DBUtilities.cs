using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace FoodRecipes.Utilities
{
    class DBUtilities
    {
        private DBUtilities() { }

        private static DBUtilities _dbInstance;
        private static FoodRecipeEntities1 _dbFoodRecipe;
        //private AppUtilities _appUtilities = new AppUtilities();

        public static DBUtilities GetDBInstance()
        {
            if (_dbInstance == null)
            {
                _dbInstance = new DBUtilities();
                _dbFoodRecipe = new FoodRecipeEntities1();
            }
            else
            {
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

        public Recipe GetRecipeById(Nullable<int> id_recipe)
        {
            var query = $"SELECT * FROM RECIPE WHERE ID_RECIPE = {id_recipe}";
            var result = _dbFoodRecipe
                .Database
                .SqlQuery<Recipe>(query)
                .Single();

            return result;
        }

        public IQueryable<GetAllFromRecipe_Result> GetAllFromRecipe()
        {
            var result = _dbFoodRecipe.GetAllFromRecipe();

            return result;
        }

        public virtual IQueryable<GetIgredientByIDRecipe_Result> GetIgredientByIDRecipe(Nullable<int> id_recipe)
        {
            var result = _dbFoodRecipe.GetIgredientByIDRecipe(id_recipe);

            return result;
        }

        public IQueryable<GetLinkImageSByIDRecipeAndNOStep_Result> GetLinkImageSByIDRecipeAndNOStep(Nullable<int> id_recipe, Nullable<int> nO_step)
        {
            var result = _dbFoodRecipe.GetLinkImageSByIDRecipeAndNOStep(id_recipe, nO_step);

            return result;
        }

        public IQueryable<GetNumericalOrderAndDetailOfStepByIDRecipe_Result> GetNumericalOrderAndDetailOfStepByIDRecipe(Nullable<int> id_recipe)
        {
            var result = _dbFoodRecipe.GetNumericalOrderAndDetailOfStepByIDRecipe(id_recipe);

            return result;
        }

        public int InsertRecipe(Nullable<int> id_recipe, 
                                string name, 
                                string description, 
                                string link_video, 
                                string link_avatar, 
                                Nullable<int> time, 
                                string food_group, 
                                Nullable<int> food_level, 
                                Nullable<bool> shopping_flag, 
                                Nullable<bool> favorite_flag, 
                                Nullable<System.DateTime> add_date)
        {
            var result = _dbFoodRecipe.InsertRecipe(id_recipe, name, description, link_video, link_avatar, time, food_group, food_level, shopping_flag, favorite_flag, add_date);

            return result;
        }

        public int InsertIgredient(Nullable<int> id_recipe, string name, string quantity)
        {
            var result = _dbFoodRecipe.InsertIgredient(id_recipe, name, quantity);

            return result;
        }

        public int InsertStep(Nullable<int> id_recipe, Nullable<int> no_step, string detail)
        {
            var result = _dbFoodRecipe.InsertStep(id_recipe, no_step, detail);

            return result;
        }

        public int InsertStepImages(Nullable<int> id_recipe, Nullable<int> no_step, string link_images)
        {
            var result = _dbFoodRecipe.InsertStepImages(id_recipe, no_step, link_images);

            return result;
        }

        public int TurnFavoriteFlagOff(Nullable<int> id_recipe)
        {
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

        public IQueryable<SearchByName_Result> SearchByName(string search_text)
        {
            var result = _dbFoodRecipe.SearchByName(search_text);

            return result;
        }

        //(SELECT * FROM [dbo].[Recipe] ORDER BY [ADD_DATE] DESC OFFSET (@current_page - 1)*@total_recipe_per_page ROWS FETCH NEXT @total_recipe_per_page ROWS ONLY);
    
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalRecipePerPage"></param>
        /// <returns></returns>        
        public (List<Recipe>, int) ExecQureyToGetRecipes(string condition, (string column, string type) sortedBy, int currentPage, int totalRecipePerPage)
        {
            (List<Recipe> recipesResult, int totalRecipe) result;
            List<Recipe> recipesResult = new List<Recipe>();
            int totalRecipe = 0;

            string query = "";

            if (condition.Length > 0)
            {
                query = $"SELECT COUNT(ID_RECIPE) FROM [dbo].[Recipe] WHERE {condition}";
            }
            else
            {
                query = $"SELECT COUNT(ID_RECIPE) FROM [dbo].[Recipe]";
            }

            totalRecipe = _dbFoodRecipe
                .Database
                .SqlQuery<int>(query)
                .Single();

            if (totalRecipe > 0)
            { 
                query = query.Replace("COUNT(ID_RECIPE)", "*");
                query += $" ORDER BY [{sortedBy.column}] {sortedBy.type} OFFSET {currentPage - 1}*{totalRecipePerPage} ROWS FETCH NEXT {totalRecipePerPage} ROWS ONLY";

                recipesResult = _dbFoodRecipe
                .Database
                .SqlQuery<Recipe>(query)
                .ToList();
            }

            result.recipesResult = recipesResult;
            result.totalRecipe = totalRecipe;

            return result;
        }

        /*
         select *
         from SearchByName(N'Bò') as search inner join Recipe as r on search.ID_RECIPE = r.ID_RECIPE
         where r.FOOD_GROUP = N'Ăn sáng' OR r.FOOD_GROUP = N'Ăn vặt'
         ORDER BY [ADD_DATE] DESC OFFSET 0 ROWS FETCH NEXT 12 ROWS ONLY
        */
        public (List<Recipe>, int) GetRecipesSearchResult(string search_text, string condition, (string column, string type) sortedBy, int currentPage, int totalRecipePerPage)
        {
            (List<Recipe> recipesSearchResultList, int totalRecipeSearch) result; 
            List<Recipe> recipesSearchResultList = new List<Recipe>();
            int totalRecipeSearch = 0;

            //Nhìn ở trên app thì cũng ngon đấy. Chứ đâu ai biết ở dưới app gồng mình catch exception
            try
            {
                string[] OPERATOR = { "and", "or", "and not" };

                //Chuẩn hóa hết mấy cái khoảng trắng thừa.
                //đưa hết mấy cái operator về and, or, and not. không để AND....
                search_text = GetStandardString(search_text);

                //Lấy hết mấy cái "abcd" vô cái stack để hồi pop ra.
                Stack<string> keywords = GetListKeyWords(search_text);

                //:V lấy hết and, or, and not đẩy vô queue.
                Queue<int> operators = GetListOperator(search_text);

                //Nếu số ngoặc kép " là lẻ thì để khỏi crash. thay " thành # :). Best sửa.
                //Tại sao lại là keywords.Count. Vì lúc lấy cái keywords ra thì chỉ có kết quả khi số " là chẵn. còn nếu " lẻ thì keywords sẽ k có phần tử nào.
                if (keywords.Count == 0)
                {
                    search_text = search_text.Replace("\"", "#");

                    string query = "";

                    if (condition.Length > 0)
                    {
                        query = $"SELECT COUNT(ID_RECIPE) FROM SearchByName(N'{search_text}') WHERE {condition}";
                    }
                    else
                    {
                        query = $"SELECT COUNT(ID_RECIPE) FROM SearchByName(N'{search_text}')";
                    }

                    totalRecipeSearch = _dbFoodRecipe
                        .Database
                        .SqlQuery<int>(query)
                        .Single();

                    if (totalRecipeSearch > 0)
                    {
                        query = query.Replace("COUNT(ID_RECIPE)", "*");
                        query += $" ORDER BY [{sortedBy.column}] {sortedBy.type} OFFSET {currentPage - 1}*{totalRecipePerPage} ROWS FETCH NEXT {totalRecipePerPage} ROWS ONLY";

                        recipesSearchResultList = _dbFoodRecipe
                        .Database
                        .SqlQuery<Recipe>(query)
                        .ToList();
                    }

                    //Thay xong rồi thì tìm bình thường thôi
                    //var recipesSearchResult = SearchByName(search_text).OrderByDescending(r => r.RANK);

                    //foreach (var recipeSearchResult in recipesSearchResult)
                    //{
                    //    var recipe = from r in recipes
                    //                 where r.ID_RECIPE == recipeSearchResult.ID_RECIPE
                    //                 select r;
                    //    recipesSearchResultList.Add(recipe.FirstOrDefault());
                    //}

                }
                //Điều kiện này tại có nhiều khi nguòi ta chỉ nhập "ab" mà không có toán tử and, or, and not á. thì cũng tìm bình thường á.
                else if (operators.Count > 0)
                {
                    /*
                        Những cái mà dùng HashSet là để khỏi loại những kết quả trùng nhau thui. như kiểu để select distinct
                    */

                    //Cái này để hồi lấy kết quả sau khi thực hiện hết các phép toán tìm kiếm
                    HashSet<int> tempIDsResult = new HashSet<int>();

                    //Cái deathID này là lúc dùng and not á.
                    //Ví dụ "a" and not "b" là coi như thằng nào có "a b" là lấy id bỏ vô cái deadthID này
                    //Đến hồi xét mà có thằng nào nằm trong này là loại luôn
                    HashSet<int> deathID = new HashSet<int>();

                    //Bắt đầu với toán tử đầu tiên
                    int count = 1;

                    //Thực hiện đến khi nào hết toán tử
                    while (operators.Count > 0)
                    {
                        //Toán tử tìm kiếm đẩy vô queue từ trái sang phải. kiểu thực hiện từ trái sáng phải á.
                        var operatorStr = OPERATOR[operators.Dequeue() - 1];

                        //params1 là list các param1 :V 
                        //Lợi hại khi bắt đầu kết hợp nếu có từ 2 toán tử tìm kiếm trong search_text
                        List<string> params1 = new List<string>();

                        //Cái chỗ này là á. 
                        //Khi thực hiện "abc" opr "def" nó sẽ ra 1 list kết quả hoặc thậm chí là k có kết quả nào.
                        //Cần đếm số kết quả đó khi push vô stack để hồi pop ra cho đủ nên mới tồn tại cái count này.
                        //pop ra đủ thì mới thực hiện tiếp mấy cái toán tử sau cho nó chuẩn được
                        while (count > 0)
                        {
                            params1.Add(keywords.Pop());
                            --count;
                        }

                        //param2 thì chỉ có 1 thâu.
                        string param2 = keywords.Pop();

                        //Cái này để tránh bị trùng á
                        HashSet<string> tempKeyWords = new HashSet<string>();

                        //Bắt đầu quá trình thực hiện phép toán tìm kiếm
                        foreach (var param1 in params1)
                        {
                            string query = "";
                            //Tìm DeathID
                            if (operatorStr == "and not")
                            {
                                string deathSearchText = param1 + " " + "and" + " " + param2;

                                //var tempRecipesSearchResultDeath = SearchByName(deathSearchText);
                                query = $"SELECT * FROM SearchByName(N'{deathSearchText}')";

                                var tempRecipesSearchResultDeath = _dbFoodRecipe
                                                                .Database
                                                                .SqlQuery<Recipe>(query)
                                                                .ToList();

                                foreach (var tempRecipeSearchResultDeath in tempRecipesSearchResultDeath)
                                {
                                    deathID.Add(tempRecipeSearchResultDeath.ID_RECIPE);
                                }
                            }

                            //Thực hiện tìm lần lượt nào
                            string tempSearchText = param1 + " " + operatorStr + " " + param2;

                            query = $"SELECT * FROM SearchByName(N'{tempSearchText}')";

                            var tempRecipesSearchResult = _dbFoodRecipe
                                                        .Database
                                                        .SqlQuery<Recipe>(query)
                                                        .ToList();

                            count += tempRecipesSearchResult.Count();

                            foreach (var tempRecipeSearchResult in tempRecipesSearchResult)
                            {
                                if (operators.Count == 0)
                                {
                                    tempIDsResult.Add(tempRecipeSearchResult.ID_RECIPE);
                                }
                                else
                                {
                                    //Add cái tên mới tìm ra để tí kết vào tìm theo operator sau tiếp
                                    tempKeyWords.Add("\"" + tempRecipeSearchResult.NAME + "\"");
                                }

                            }

                            while (tempKeyWords.Count > 0)
                            {
                                keywords.Push(tempKeyWords.First());
                                tempKeyWords.Remove(tempKeyWords.First());
                            }
                        }
                    }

                    //Lấy kểt quả cuối
                    bool hasConditionBefore = (condition.Length > 0 ? true : false);
                    string resultQuery = "";

                    if (tempIDsResult.Count > 0)
                    {
                        if (condition.Length > 0)
                        {
                            condition += " AND (";
                        }
                        else
                        {
                            //Do Nothing
                        }

                        foreach (var tempID in tempIDsResult)
                        {
                            if (!deathID.Contains(tempID))
                            {
                                condition += $" ID_RECIPE = {tempID} OR";
                            }
                            else
                            {
                                //Do Nothing
                            }
                        }

                        if (condition.Length > 0)
                        {
                            //Select * from [dbo].[Recipe] where FAVORITE_FLAG = 1 AND (FOOD_GROUP = N'Ăn sáng' OR FOOD_GROUP = N'Món chính')
                            //Select * from [dbo].[Recipe] where FOOD_GROUP = N'Ăn sáng'
                            condition = condition.Substring(0, condition.Length - 3);

                            if (hasConditionBefore)
                            {
                                condition += ")";
                            }
                            else
                            {
                                //Do Nothing
                            }

                            if (condition.Length > 0)
                            {
                                resultQuery = $"SELECT COUNT(ID_RECIPE) FROM RECIPE WHERE {condition}";
                            }
                        }
                        else
                        {
                            //Do Nothing
                        }

                        totalRecipeSearch = _dbFoodRecipe
                            .Database
                            .SqlQuery<int>(resultQuery)
                            .Single();

                        if (totalRecipeSearch > 0)
                        {
                            resultQuery = resultQuery.Replace("COUNT(ID_RECIPE)", "*");
                            resultQuery += $" ORDER BY [{sortedBy.column}] {sortedBy.type} OFFSET {currentPage - 1}*{totalRecipePerPage} ROWS FETCH NEXT {totalRecipePerPage} ROWS ONLY";

                            recipesSearchResultList = _dbFoodRecipe
                            .Database
                            .SqlQuery<Recipe>(resultQuery)
                            .ToList();
                        }
                    }
                    else
                    {
                        //Do Nothing
                    }

                    

                }
                else
                {
                    string query = "";

                    if (condition.Length > 0)
                    {
                        query = $"SELECT COUNT(ID_RECIPE) FROM SearchByName(N'{search_text}') WHERE {condition}";
                    }
                    else
                    {
                        query = $"SELECT COUNT(ID_RECIPE) FROM SearchByName(N'{search_text}')";
                    }

                    totalRecipeSearch = _dbFoodRecipe
                        .Database
                        .SqlQuery<int>(query)
                        .Single();

                    if (totalRecipeSearch > 0)
                    {
                        query = query.Replace("COUNT(ID_RECIPE)", "*");
                        query += $" ORDER BY [{sortedBy.column}] {sortedBy.type} OFFSET {currentPage - 1}*{totalRecipePerPage} ROWS FETCH NEXT {totalRecipePerPage} ROWS ONLY";

                        recipesSearchResultList = _dbFoodRecipe
                        .Database
                        .SqlQuery<Recipe>(query)
                        .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
            }

            result.recipesSearchResultList = recipesSearchResultList;
            result.totalRecipeSearch = totalRecipeSearch;

            return result;
        }

        public string GetStandardString(string srcString)
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

        //Lấy cái list KeyWord để search nè.
        public Stack<string> GetListKeyWords(string search_text)
        {
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

            if (indexQuotes.Count % 2 == 0)
            {
                for (int i = 0; i < indexQuotes.Count - 1; i += 2)
                {
                    string tempString = "";

                    for (int j = indexQuotes[i]; j <= indexQuotes[i + 1]; ++j)
                    {
                        tempString += search_text[j];
                    }

                    //"abc" and "123"
                    if (tempString.Length > 2)
                    {
                        temp.Push(tempString);
                    }
                }
            }

            while (temp.Count > 0)
            {
                result.Push(temp.Pop());
            }

            return result;
        }

        //Lấy cái list toán tử nè 
        public Queue<int> GetListOperator(string search_text)
        {
            Queue<int> result = new Queue<int>();

            var tokens = search_text.Split(' ');

            for (int i = 0; i < tokens.Length; ++i)
            {
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
                else if (tokens[i] == "or")
                {
                    result.Enqueue(2);
                }
            }
            return result;
        }

        public List<Recipe> GetShoppingRecipes(string condition, (string column, string type) sortedBy) {
            List<Recipe> result = new List<Recipe>();
            string query = "";
            if (condition.Length > 0)
            {
                query = $"SELECT * FROM RECIPE WHERE SHOPPING_FLAG = 1 AND {condition} ORDER BY [{sortedBy.column}] {sortedBy.type}";
            }
            else
            {
                query = $"SELECT * FROM RECIPE WHERE SHOPPING_FLAG = 1 ORDER BY [{sortedBy.column}] {sortedBy.type}";
            }
           
            result = _dbFoodRecipe
                    .Database
                    .SqlQuery<Recipe>(query)
                    .ToList();

            return result;
        }
    }
}
