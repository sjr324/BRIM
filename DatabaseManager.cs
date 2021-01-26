using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    public class DatabaseManager
    {
        //Should probably move this somewhere more secure eventually
        private static string connString = @"SERVER=68.84.78.85;PORT=3306;DATABASE=brim;UID=dev;PASSWORD=devpassword;";
        private MySqlConnection conn = new MySqlConnection(connString);

        //runs any given Select Query on the Database and returns the results in a DataTable
        //Should only be used by other public methods within the Helper (unless you guys thing other wise)
        private DataTable runSelectQuery(string query) 
        {
            DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            //Is there a reason we open and close for every query instead of just keeping it open?
            //beats me, but I'll ask the others later
            conn.Open();
            int rowsReturned = adapter.Fill(dt);
            conn.Close();
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("The database query returned no data");
            }
            return dt;
        }

        //Runs the given insert, update, or delete statement to add affect the database, returns the amount of rows affected
        public bool runSqlQuery(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);
            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if(rowsAffected <= 0)
            {
                Console.WriteLine("The database query could not insert the information");
                
                return false;
            }

            return true;
        }

        //Querys the database for all entries in the drinks table and returns them as a list
        public List<Item> getDrinks()
        {
            List<Item> newDrinkList = new List<Item>();
            string queryString = @"select * from drinks";

            DataTable dt = this.runSelectQuery(queryString);

            foreach(DataRow dr in dt.Rows) {
                Drink tempDrink = new Drink(dr);
                newDrinkList.Add(tempDrink);
            }
            return newDrinkList;
        }

        // Querys the database for all entries in the drinkrecipes, and the pertinent information  
        // of their respectively referenced Recipe and Drink entries
        public List<Recipe> getRecipes()
        {
            List<Recipe> newRecipeList = new List<Recipe>();
            //Want to talk to Alex about what he's expecting to send and get front end from recipes
            //because most of this drink information really shouldn't be neccesary for looking at, or updating, 
            //recipe components
            string queryString = @"SELECT brim.recipes.name AS recipeName, brim.recipes.recipeID, " 
            + "brim.drinks.drinkID, brim.drinks.name, brim.drinks.lowerEstimate, brim.drinks.upperEstimate, " 
            + "brim.drinkrecipes.itemQuantity, brim.drinks.measurementUnit, brim.drinks.parLevel, "
            + "brim.drinks.parLevel, brim.drinks.idealLevel, brim.drinks.bottleSize, brim.drinks.brand, "
            + "brim.drinks.bottlesPerCase, brim.drinks.vintage, brim.drinks.price "
            + "FROM brim.drinkrecipes "
            + "INNER JOIN recipes ON drinkrecipes.recipeID = recipes.recipeID " 
            + "INNER JOIN drinks ON drinkrecipes.drinkID = drinks.drinkID;";

            DataTable dt = this.runSelectQuery(queryString);
            //get list of recipe IDs
            var recipeIDs = dt.AsEnumerable()
                .Select(dr=>new { ID = dr.Field<int>("recipeID"), name = dr.Field<string>("recipeName") })
                .Distinct();
            foreach(var recipe in recipeIDs) {
                var recipeIngredients = dt.AsEnumerable()
                    .Select(dr=>dr)
                    .Where(dr=>dr.Field<int>("recipeID") == recipe.ID);
                Recipe tempDrink = new Recipe(recipe.ID, recipe.name, recipeIngredients);
                newRecipeList.Add(tempDrink);
            }

            return newRecipeList;
        }
    }
}