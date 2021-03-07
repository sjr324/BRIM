using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public class DatabaseManager : IDatabaseManager
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
            int rowsReturned;

            //Is there a reason we open and close for every query instead of just keeping it open?
            //beats me, but I'll ask the others later
            try {
                conn.Open();
                rowsReturned = adapter.Fill(dt);
                conn.Close();
            } catch (MySqlException ex) {
                Console.WriteLine("MYSQL ERROR OCCURED! ERROR MESSAGE: " + ex.Message);
                rowsReturned = 0;
            }

            if (rowsReturned == 0)
            {
                Console.WriteLine("The database query returned no data");
            }
            return dt;
        }

        // Runs the given insert, update, or delete statement to add affect the database, 
        //returns true if Command successful executes, otherwise returns false
        private bool runSqlInsertUpdateOrDeleteCommand(string query)
        {
            int rowsAffected;
            try {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();
            } catch (MySqlException ex) {
                Console.WriteLine("MYSQL ERROR OCCURED! ERROR MESSAGE: " + ex.Message);
                return false;
            }

            if (rowsAffected == 0) {
                Console.WriteLine("The Command was Valid, but no Rows where affected: ");
            } else if (rowsAffected < 0){
                Console.WriteLine("The Given Query was not for an Update, or Delete Command: ");
                return false;
            }
            return true;
        }

        // Runs the given insert statement to affect the database, 
        // returns the value given to the Auto-incremented Colmun of the last insert made
        // assuming to multi-threading shenanigans happen, that should be the ID of the entry
        // this method just inserted
        // IF the command fails, returns -1 
        // NOTE: while this code CAN technically Run DELETE and UPDATE Commands, the return value will only
        // be relevant for Insert Commands
        private int runSqlInsertCommandReturnID(string query)
        {
            int rowsAffected, newValueID;
            try {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
                //WARNING, This method of getting the ID is the t but is NOT entirely thread safe
                newValueID = (int) (cmd.LastInsertedId);
                conn.Close();
            } catch (MySqlException ex) {
                Console.WriteLine("MYSQL ERROR OCCURED! ERROR MESSAGE: " + ex.Message);
                return -1;
            }
            
            if (rowsAffected < 0){
                Console.WriteLine("The Given Query was not for an Insert, Update, or Delete Command: ");
                return -1;
            }
            return newValueID;
        }


        //Creates then runs a delete query
        public bool deleteDrink(Drink drink)
        {
            string query = @"delete from brim.drinks where drinkID = '" + drink.ID + "'";
            bool result = this.runSqlInsertUpdateOrDeleteCommand(query);

            if (!result)
            {
                Console.WriteLine("Error: Drink could not be deleted");

                return false;
            }

            return true;
        }

        //Creates then runs an insert query
        public bool addDrink(Drink drink)
        {
            //TODO: Re-add vintage
            string query = @"insert into brim.drinks (name, lowerEstimate, upperEstimate, measurementUnit, parLevel, idealLevel, bottleSize, brand, bottlesPerCase, vintage) values ('"
                + drink.Name + "', '" + drink.LowerEstimate + "', '" + drink.UpperEstimate + "', '" + drink.Measurement + "', '" + drink.ParLevel + "', '" + drink.IdealLevel
                + "', '" + drink.BottleSize + "', '" + drink.Brand + "', '" + drink.UnitsPerCase + "', '" + 2000 + "')";
            int newDrinkID = this.runSqlInsertCommandReturnID(query);

            if (newDrinkID == -1)
            {
                Console.WriteLine("Error: Drink could not be added");
                return false;
            }

            return true;
        }

        //Creates then runs an update query
        public bool updateDrink(Drink drink)
        {
            //TODO: Re-add vintage
            string query = @"update brim.drinks set name = '" + drink.Name + "', lowerEstimate = '" + drink.LowerEstimate + "', upperEstimate = '" + drink.UpperEstimate
                + "', measurementUnit = '" + drink.Measurement + "', parLevel = '" + drink.ParLevel + "', idealLevel = '" + drink.IdealLevel + "', bottleSize = '"
                + drink.BottleSize + "', brand = '" + drink.Brand + "', bottlesPerCase = '" + drink.UnitsPerCase + "', vintage = '" + 2000 + "' where "
                + "drinkID = '" + drink.ID + "'";
            bool result = this.runSqlInsertUpdateOrDeleteCommand(query);

            if (!result)
            {
                Console.WriteLine("Error: Drink could not be updated");
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

        //Creates then runs a delete query for entry IN RECIPES TABLE ONLY
        //NOTE: DO NOT ATTEMPT TO USE THIS BEFORE DELETING ALL CONNECTED ENTRIES IN DRINKRECIPE TABLE 
        public bool deleteRecipe(int recipeID)
        {
            string query = @"delete from brim.recipes where recipeID = '" + recipeID + "';";
            bool result = this.runSqlInsertUpdateOrDeleteCommand(query);

            if (!result)
            {
                Console.WriteLine("Error: Recipe Entry could not be deleted");
                return false;
            }

            return true;
        }

        //Deletes all entries in the drinkRecipes Table with a certain recipeID
        public bool deleteDrinkRecipesByRecipeID(int recipeID)
        {
            string query = @"delete from brim.drinkrecipes where recipeID = '" + recipeID + "';";
            bool result = this.runSqlInsertUpdateOrDeleteCommand(query);

            if (!result)
            {
                Console.WriteLine("Error: DrinkRecipe Entries could not be deleted");
                return false;
            }

            return true;
        }

        //Creates then runs an insert query FOR JUST THE RECIPES TABLE
        //returns the ID of the newly Inserted Recipe
        public int addRecipe(string name, string baseLiquor)
        {
            string query = @"INSERT INTO brim.recipes (name, baseLiquor) VALUES ('" + name + "', '" + baseLiquor + "');";
            int newRecipeID = this.runSqlInsertCommandReturnID(query);

            if (newRecipeID == -1)
            {
                Console.WriteLine("Error: Recipe Entry Could not be Added");
            }
            return newRecipeID;
        }

        //Creates then runs an insert query FOR JUST THE DRINKRECIPES TABLE
        //ASSUMES THAT RECIPE AND DRINK IDS ARE VALID. AKA THAT THEY BELONG TO DRINKS AND RECIPES THAT EXIST 
        public int addDrinkRecipe(int recipeID, int drinkID, double itemQuantity){
            string query = @"INSERT INTO brim.drinkrecipes (itemQuantity, recipeID, drinkID) "
                + "VALUES ('" + itemQuantity + "', '" + recipeID + "', '" + drinkID + "');";
            int newDrinkRecipeID = this.runSqlInsertCommandReturnID(query);

            if (newDrinkRecipeID == -1)
            {
                Console.WriteLine("Error: DrinkRecipe Entry Could not be Added");
            }
            return newDrinkRecipeID;
        }

        // Creates then runs an UPDATE query FOR ONLY THE ENTRY IN THE RECIPES TABLE
        // only sends the ID, name, and baseLiquor since RecipeObjects can get large from the itemList
        // even though Entries is Recipes Table itself only have column that will really be modified
        public bool updateRecipe(int recipeID, string name, string baseLiquor)
        {
            string query = @"UPDATE brim.recipes SET name = '" + name + "', baseLiquor = '" + baseLiquor + "'"
                + " WHERE recipeID = '" + recipeID + "';";
            bool result = this.runSqlInsertUpdateOrDeleteCommand(query);

            if (!result)
            {
                Console.WriteLine("Error: Recipe Entry Could not be Updated");
            }
            return result;
        }

        /* IMPORTANT!: DrinkRecipe Table Updates for am existing Recipe will be handled by Deleting and 
        all DrinkRecipe Entries related to that table and re-adding/replacing the entries with entire ItemList
        of the updated Recipe Object, for Simplicity's sake. */

        // Querys the database for all entries in the drinkrecipes, and the pertinent information  
        // of their respectively referenced Recipe and Drink entries
        public List<Recipe> getRecipes()
        {
            List<Recipe> newRecipeList = new List<Recipe>();
            //Want to talk to Alex about what he's expecting to send and get front end from recipes
            //because most of this drink information really shouldn't be neccesary for looking at, or updating, 
            //recipe components
            string queryString = @"SELECT brim.recipes.name AS recipeName, brim.recipes.baseLiquor, brim.recipes.recipeID, " 
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
                .Select(dr=>new { ID = dr.Field<int>("recipeID"), name = dr.Field<string>("recipeName"), baseLiquor = dr.Field<string>("baseLiquor") })
                .Distinct();
            foreach(var recipe in recipeIDs) {
                var recipeIngredients = dt.AsEnumerable()
                    .Select(dr=>dr)
                    .Where(dr=>dr.Field<int>("recipeID") == recipe.ID);
                Recipe tempDrink = new Recipe(recipe.ID, recipe.name, recipe.baseLiquor, recipeIngredients);
                newRecipeList.Add(tempDrink);
            }

            return newRecipeList;
        }
    }
}