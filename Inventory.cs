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
        public DatabaseManager databaseManager = new DatabaseManager();

        /// <summary>
        /// Runs anh update command on the item based off the item that is sent in from the frontend
        /// </summary>
        /// <param name="info">An item that is created from the frontend</param>
        /// <returns>an integer return based on the exit status of the function</returns>
        public int UpdateItem(Item info)
        {
            Drink updateItem = info as Drink;
            bool result = this.databaseManager.updateDrink(updateItem);

            if (!result)
            {
                Console.WriteLine("Error: Item Update Failed");
            }

            return 0;
        }

        /// <summary>
        /// Takes the item information sent to it from the frontend and calls database to add an item.
        /// The database call returns true if the item is added and false otherwise.
        /// </summary>
        /// <param name="i">Item that the frontend sends to the backend</param>
        /// <returns>An integer is returned that corresponds to the exit status of the method</returns>
        public int AddItem(Item i)
        {
            Drink newItem = i as Drink;
            bool result = this.databaseManager.addDrink(newItem);
            
            if (!result)
            {
                Console.WriteLine("Error: Item Addition Failed");
            }

            return 0;
        }

        /// <summary>
        /// This item takes in an item to be removed and then makes a call to the database to delete that item
        /// The database call returns true if the item is removed and false otherwise.
        /// </summary>
        /// <param name="i">The item to be deleted</param>
        /// <returns>an integer value representing the exit status of the method</returns>
        public int RemoveItem(Item i)
        {
            Drink removeItem = i as Drink;
            bool result = this.databaseManager.deleteDrink(removeItem);

            if (!result)
            {
                Console.WriteLine("Error: Item removal failed");
            }

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
            List<Item> drinksList = new List<Item>();
            drinksList = this.databaseManager.getDrinks();
            ItemList = drinksList;

            return 0;
        }

        //what are these for again? Are the even still neccesary with the current plan, or are the vestigial?
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
            List<Recipe> recipeList = new List<Recipe>();
            recipeList = this.databaseManager.getRecipes();
            RecipeList = recipeList;

            return 0;
        }

        /// <summary>
        /// Takes the recipe information sent to it from the frontend and calls database to add a recipe.
        /// The database call returns true if the recipe is added and false otherwise.
        /// </summary>
        /// <param name="newRecipe">Recipe that the frontend sends to the backend</param>
        /// <returns>An integer is returned that corresponds to the exit status of the method</returns>
        public int AddRecipe(Recipe newRecipe)
        {
            int recipeID, itemListResult;
            //add entry into Recipe Table
            recipeID = this.databaseManager.addRecipe(newRecipe.Name);
            if (recipeID == -1)
            {
                Console.WriteLine("Error: Recipe Entry Addition Failed. Stopping here");
                return 0;
            }

            //NOTE: this relies on the recipe Object sent from the frontend is a DeepCopy and not a shallow one
            //i.e. that the Item Objects in item list are copied, and not just the references
            foreach((Item item, double quantity) component in newRecipe.ItemList) {
                int itemID = component.item.ID;
                double itemQuantity = component.quantity;
                itemListResult = this.databaseManager.addDrinkRecipe(recipeID, itemID, itemQuantity);

                if (itemListResult == -1) {
                    Console.WriteLine("Error: DrinkRecipe Entry Addition Failed For Entry with '"
                    + itemID + "' ItemID and '" + itemQuantity + "' failed. Stopping here");
                    break;
                }
            }
            
            
            // TODO: come up with and implement a better structure for return Codes
            // at the very Least, we should make them an Enum with Proper names for each Code;
            return 0; 
        }
        
        /// <summary>
        /// Takes the recipe information sent to it from the frontend and calls database to update a recipe.
        /// The database call returns true if the recipe is updated and false otherwise.
        /// </summary>
        /// <param name="updatedRecipe">Recipe that the frontend sends to the backend</param>
        /// <returns>An integer is returned that corresponds to the exit status of the method</returns>
        public int UpdateRecipe(Recipe updatedRecipe)
        {
            int recipeID, itemListResult;
            //add entry into Recipe Table
            recipeID = updatedRecipe.ID;
            string updatedName = updatedRecipe.Name;
            
            if (!this.databaseManager.updateRecipe(recipeID, updatedName))
            {
                Console.WriteLine("Error: Recipe Entry Update Failed. Stopping here");
                return 0;
            }
            
            //deleting old DrinkRecipe table entries for this Recipe
            if (!this.databaseManager.deleteDrinkRecipesByRecipeID(recipeID)) {
                Console.WriteLine("Error: DrinkRecipe Entry Deletion Failed. Stopping here");
                return 0;
            }

            //NOTE: this relies on the recipe Object sent from the frontend is a DeepCopy and not a shallow one
            //i.e. that the Item Objects in item list are copied, and not just the references
            foreach((Item item, double quantity) component in updatedRecipe.ItemList) {
                int itemID = component.item.ID;
                double itemQuantity = component.quantity;
                itemListResult = this.databaseManager.addDrinkRecipe(recipeID, itemID, itemQuantity);

                if (itemListResult == -1) {
                    Console.WriteLine("Error: DrinkRecipe Entry Addition Failed For Entry with '"
                    + itemID + "' ItemID and '" + itemQuantity + "' failed. Stopping here");
                    break;
                }
            }
            
            return 0;
        }

        
        /// <summary>
        /// This method takes in a recipe to be removed and then makes a call to the database to delete 
        /// that recipe from the recipes table, as well as any connected entries in the drinkRecipes table
        /// The database call returns true if the recipe is removed and false otherwise.
        /// </summary>
        /// <param name="unwantedRecipe">The item to be deleted</param>
        /// <returns>an integer value representing the exit status of the method</returns>
        public int RemoveRecipe(Recipe unwantedRecipe)
        {
            int recipeID = unwantedRecipe.ID;

            //deleting old DrinkRecipes table entries for this Recipe
            //Should be done first since drinkrecipe Table is the one with the constraints
            if (!this.databaseManager.deleteDrinkRecipesByRecipeID(recipeID)) {
                Console.WriteLine("Error: DrinkRecipe Entry Deletion Failed. Stopping here");
                return 0;
            }

            // delete recip entry from recipes table 
            if (!this.databaseManager.deleteRecipe(recipeID)) {
                Console.WriteLine("Error: DrinkRecipe Entry Deletion Failed. Stopping here");
                return 0;
            }
            return 0;
        }
    }
}
