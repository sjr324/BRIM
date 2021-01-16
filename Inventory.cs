using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    //The main class of the program. 
    public class Inventory
    {
        public List<Item> ItemList = new List<Item>(); //holds all items registered to this BRIM instance
        public List<Recipe> RecipeList = new List<Recipe>(); //holds all of the recipes for this BRIM instance
        public string Country;

        public int UpdateItem(Item info)
        {
            return 0;
        }

        public int AddItem(Item i)
        {
            return 0;
        }

        public int RemoveItem(Item i)
        {
            return 0;
        }

        public int PurchseItem(Recipe r)
        {
            return 0;
        }

        /// <summary>
        /// Makes a call to the drinks table of the brim database to get the list of all drinks 
        /// that the user has registerd. 
        /// Creates a new item list and replaces the global ItemList with that when it is finished.
        /// In the future will also query for any other items a user can have in their inventory
        /// such as food.
        /// </summary>
        /// <returns>an integer value that represents the exit status of the method</returns>
        public int GetItemList()
        {
            List<Item> newList = new List<Item>();
            DataTable dt = new DataTable();
            int rowsReturned;
            string connString = @"SERVER=68.84.78.85;PORT=3306;DATABASE=brim;UID=dev;PASSWORD=devpassword;";
            //read from drink table and add to item list, can do food later down the line
            string query = @"select * from drinks";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            conn.Open();
            rowsReturned = adapter.Fill(dt);
            conn.Close();

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("The database query returned no data");
            } else
            {
                foreach(DataRow dr in dt.Rows)
                {
                    Drink tempDrink = new Drink();
                    tempDrink.Name = dr.Field<string>("name");
                    tempDrink.LowerEstimate = dr.Field<double>("lowerEstimate");
                    tempDrink.UpperEstimate = dr.Field<double>("upperEstimate");
                    tempDrink.Measurement = dr.Field<unit>("measurmentUnit");
                    tempDrink.ParLevel = dr.Field<double>("parLevel");
                    tempDrink.IdealLevel = dr.Field<double>("idealLevel");
                    tempDrink.BottleSize = dr.Field<int>("bottleSize");
                    tempDrink.Brand = dr.Field<string>("brand");
                    tempDrink.UnitsPerCase = dr.Field<int>("bottlesPerCase");
                    tempDrink.Vintage = dr.Field<int>("vintage");

                    newList.Add(tempDrink);
                }
            }

            ItemList = newList;

            return 0;
        }

        public void EmitEvent()
        {

        }

        public void ListenEvent()
        {

        }

        /// <summary>
        /// This method gets all of the recipes that have been registered in the dratabase and adds them to the 
        /// global recipeList variable. It overwrites the current global variable when it reads from the database.
        /// initial call to get all the stored recipes, then must call the drinkrecipe table to get the list of all
        /// registered ingredients, then must find the drink values from the drink table and make those values into a
        /// tuple list for each recipe entry
        /// </summary>
        /// <returns>It returns an int representing the status of the request</returns>
        public int GetRecipeList()
        {
            List<Recipe> newList = new List<Recipe>(); //return list
            DataTable recipeNameTable = new DataTable(); //list of all recipe names
            DataTable recipeIngredients = new DataTable(); //used to store the recipe ingredients

            int rowsReturned;
            string connString = @"SERVER=68.84.78.85;PORT=3306;DATABASE=brim;UID=dev;PASSWORD=devpassword;";
            //read from drink table and add to item list, can do food later down the line
            string query = @"select * from recipes";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //initial call for list of all recipes
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                rowsReturned = adapter.Fill(recipeNameTable);

                if (recipeNameTable.Rows.Count == 0)
                {
                    Console.WriteLine("The database query returned no data");
                }
                else
                {
                    foreach (DataRow dr in recipeNameTable.Rows)
                    {
                        Recipe tempRecipe = new Recipe();
                        tempRecipe.Name = dr.Field<string>("name");
                        int recipeID = dr.Field<int>("ID");
                        List<(Item, double)> ingredientList = new List<(Item, double)>(); //stores the list of ingredients for each recipe

                        //call the database to get the items that correspond to that recipe ID
                        cmd.CommandText = "select * from drinkrecipes where recipeID = " + recipeID;
                        adapter = new MySqlDataAdapter(cmd);
                        rowsReturned = adapter.Fill(recipeIngredients);

                        if (rowsReturned == 0)
                        {
                            //print an error somewhere
                            Console.WriteLine("There are no ingredients for this recipe");
                        } else
                        {
                            //use that list of ingredients to find their names and store their values into a tuple to add to the recipe list
                            foreach (DataRow ingredientRow in recipeIngredients.Rows)
                            {
                                double quantity = ingredientRow.Field<double>("itemQuantity");
                                int itemID = ingredientRow.Field<int>("drinkID");

                                cmd.CommandText = "select name from drinks where drinkID = " + itemID;
                                string itemName = (string)cmd.ExecuteScalar();

                                //need to search the itemList for the item with the same name, then add both.
                                int i = 0;
                                while (i < ItemList.Count)
                                {
                                    if (ItemList[i].Name == itemName)
                                    {
                                        break;
                                    }
                                    i++;
                                }

                                if (i >= ItemList.Count)
                                {
                                    //item was not found, error
                                    Console.WriteLine("Ingredient could not be found in item list");
                                }

                                //add the tuple for the ingredient to the tuple list
                                ingredientList.Add((ItemList[i], quantity));
                            }
                        }

                        //set the ingredient list for the tempRecipe and then add that recipe to the new recipeList
                        tempRecipe.ItemList = ingredientList;
                        newList.Add(tempRecipe);
                    }
                }

            }

            RecipeList = newList;

            return 0;
        }

        public int AddRecipe()
        {
            return 0;
        }

        public int UpdateRecipe()
        {
            return 0;
        }

        public int RemoveRecipe()
        {
            return 0;
        }
    }
}
