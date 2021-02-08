using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
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
        /// Runs an update command on the item based off the item that is sent in from the frontend
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
        /// Takes in a JObject message from the controller. This message is the response from an API update call.
        /// This method loops through this message and parses each lineitem that was ordered, then updates those items
        /// in the database based off the quantity of them that is ordered.
        /// Looks up the item in the drinks to see if it is a saved drink, if not then it looks up the recipe and updates
        /// that. 
        /// </summary>
        /// <param name="message"></param>
        public void parseAPIPOSUpdate(JObject message)
        {
            JToken msg = message;

            foreach (JObject order in msg)
            {
                foreach(JObject lineitem in order["lineItems"])
                {
                    string name = lineitem["name"].ToString();
                    double updateAmt = 0.0;

                    //check if the lineItem ordered is a base drink and update
                    //then check recipies
                    //Modifications always come in for ordering a specific drink item, 
                    //but in the case of cocktails(recipies) there is a possibility of there 
                    //being no modification
                    //TODO: If not in recipies or drinks flag it for manual update
                    int drinkFound = ItemList.FindIndex(x => x.Name == name);
                    int recipieFound = RecipeList.FindIndex(x => x.Name == name);
                    if (drinkFound != -1)
                    {
                        Drink updatedDrink = ItemList[drinkFound] as Drink;

                        //TODO: Add in code for parsing modifications
                        JArray modifications = (JArray)lineitem["modifications"];

                        //should only be one, but maybe there is something im not thinking of,
                        //can change from a loop later
                        foreach (JObject mod in modifications)
                        {
                            string modName = mod["name"].ToString();

                            //if it has parenthasis then assume it is the modification that tells us the portion size
                            if (modName.Contains("("))
                            {
                                string[] temppour = modName.Split('(');
                                temppour = temppour[1].Split(')');
                                string[] pour = temppour[0].Split(' ');

                                double pourAmt = Convert.ToDouble(pour[0]);
                                string pourMeasurement = pour[1];
                                int quantitySold = (int)lineitem["quantitySold"];

                                if (pourMeasurement == "oz")
                                {
                                    pourAmt = pourAmt * 29.5735; //conversion for fluid oz to ml
                                }

                                //naive. does not account for spillage or over/under pouring
                                updateAmt += pourAmt * quantitySold;
                            }
                        }

                        updatedDrink.LowerEstimate -= updateAmt;
                        updatedDrink.UpperEstimate -= updateAmt;

                        //TODO: should update the user if any of these is true
                        if (updatedDrink.LowerEstimate < 0.0)
                        {
                            updatedDrink.LowerEstimate = 0.0;
                        }

                        if (updatedDrink.UpperEstimate < 0.0)
                        {
                            updatedDrink.UpperEstimate = 0.0;
                        }

                        databaseManager.updateDrink(updatedDrink);
                        ItemList[drinkFound] = updatedDrink;
                        updateAmt = 0.0;
                    } else if (recipieFound != -1)
                    {
                        //same process as above, but for recipies
                        //recipies may or may not have modifications
                        Recipe orderedRecipe = RecipeList[recipieFound];
                        List<(Item item, double quantity)> parts = orderedRecipe.ItemList;
                        int amtOrdered = (int)lineitem["quantitySold"];

                        JArray modifications = (JArray)lineitem["modifications"];
                        if (modifications.Count > 0)
                        {
                            string modName = modifications[0]["name"].ToString();

                            if (modName != orderedRecipe.BaseLiquor)
                            {
                                //process the modification
                                int modIndex = RecipeList.FindIndex(x => x.Name == modName);

                                if (modIndex != -1)
                                {
                                    int baseIndex = parts.FindIndex(x => x.item.Name == orderedRecipe.BaseLiquor);
                                    double q = parts[baseIndex].quantity;
                                    parts.RemoveAt(baseIndex);
                                    parts.Add((ItemList[modIndex], q));
                                } else
                                {
                                    //TODO: Flag for the user because modification is unknown
                                }
                            }

                            //update every drink that was a part of the recipe
                            foreach ((Item item, double quantity) part in parts)
                            {
                                //calcualte and update every item
                                Drink updatedDrink = part.item as Drink;

                                updateAmt += part.quantity * amtOrdered;

                                updatedDrink.LowerEstimate -= updateAmt;
                                updatedDrink.UpperEstimate -= updateAmt;

                                //TODO: should update the user if any of these is true
                                if (updatedDrink.LowerEstimate < 0.0)
                                {
                                    updatedDrink.LowerEstimate = 0.0;
                                }

                                if (updatedDrink.UpperEstimate < 0.0)
                                {
                                    updatedDrink.UpperEstimate = 0.0;
                                }

                                databaseManager.updateDrink(updatedDrink);
                                ItemList[drinkFound] = updatedDrink;
                                updateAmt = 0.0;
                            }
                        }
                    } else
                    {
                        //TODO: Add flagging for unknown items the user has to update
                    }
                }
            }
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
            recipeID = this.databaseManager.addRecipe(newRecipe.Name, newRecipe.BaseLiquor);
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
            string updatedBase = updatedRecipe.BaseLiquor;
            
            if (!this.databaseManager.updateRecipe(recipeID, updatedName, updatedBase))
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
