using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    //The main class of the program. 
    public class Inventory
    {
        public List<Item> ItemList = new List<Item>(); //holds all items registered to this BRIM instance
        public List<Recipe> RecipeList = new List<Recipe>(); //holds all of the recipes for this BRIM instance
        public List<Tag> TagList = new List<Tag>(); //holds all of the tags for this BRIM instance
        public string Country;
        public IDatabaseManager databaseManager;

        // Default Constructor, makes its own DatabaseManager
        public Inventory() {
            this.databaseManager = new DatabaseManager();
        }

        //  Overloaded Constructor, takes IDatabaseManager instance (For Mocking when Unit Testing)
        public Inventory(IDatabaseManager dbManager) {
            this.databaseManager = dbManager;
        }

        /// <summary>
        /// Runs an update command on the item based off the item that is sent in from the frontend
        /// </summary>
        /// <param name="info">An item that is created from the frontend</param>
        /// <returns>an integer return based on the exit status of the function</returns>
        public int UpdateItem(Item info)
        {
            Drink updateItem = info as Drink;
            //make sure status is recalculated to reflect any changes in Quantity
            updateItem.CalculateStatus(); 
            
            bool result = this.databaseManager.updateDrink(updateItem);

            if (!result)
            {
                Console.WriteLine("Error: Item Update Failed");

                string mes = updateItem.Name + " could not be updated";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            //Similar to Recipie updates, delete all Tag entries and readd them for simplicity
            if (!this.databaseManager.deleteDrinkTagsByDrinkID(updateItem.ID))
            {
                Console.WriteLine("Error: Drink Tag Entry Deletion Failed. Stopping here");

                string mes = updateItem.Name + " could not be updated";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            //After removing all of the tags for the drink, re-adds them
            foreach (Tag T in updateItem.Tags)
            {
                int tagID = T.ID;
                result = this.databaseManager.addDrinkTag(updateItem.ID, tagID);

                if (!result)
                {
                    Console.WriteLine("Error: Drink Tag Entry Addition Failed For Tag '"
                    + T.Name + "' on item '" + updateItem.Name + "'. Stopping here");

                    string mes = updateItem.Name + " could not be updated";
                    NotificationManager.AddNotification(mes);

                    return 1;
                }
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
            string mes = "";
            
            if (!result)
            {
                Console.WriteLine("Error: Item Addition Failed");

                mes = newItem.Name + " could not be added";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            //add in the tags associated with that drink if there are any
            foreach (Tag T in newItem.Tags)
            {
                if (!this.databaseManager.addDrinkTag(newItem.ID, T.ID))
                {
                    Console.WriteLine("Error: Tag could not be added");

                    mes = T.Name + " could not be added";
                    NotificationManager.AddNotification(mes);
                    
                    return 1;
                }
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

                string mes = removeItem.Name + " could not be removed";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            ItemList.Remove(removeItem);

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
            //double varianceMultiplier = 0.15;

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

                                //setup the total amount that should have been poured
                                updateAmt += pourAmt * quantitySold;
                            }
                        }
                        Drink updatedDrink = (Drink) orderItemUpdateProcedure(ItemList[drinkFound], updateAmt);

                        databaseManager.updateDrink(updatedDrink);
                        ItemList[drinkFound] = updatedDrink;
                        updateAmt = 0.0;
                    } else if (recipieFound != -1)
                    {
                        //same process as above, but for recipies
                        //recipies may or may not have modifications
                        Recipe orderedRecipe = RecipeList[recipieFound];
                        List<RecipeItem> parts = orderedRecipe.ItemList;
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
                                    int baseIndex = parts.FindIndex(x => x.Item.Name == orderedRecipe.BaseLiquor);
                                    double q = parts[baseIndex].Quantity;
                                    parts.RemoveAt(baseIndex);
                                    parts.Add(new RecipeItem(ItemList[modIndex] as Drink, q));
                                } else
                                {
                                    //Flag for the user because modification is unknown
                                    string mes = modName + " is unknown. Must be updated manually.";
                                    NotificationManager.AddNotification(mes);
                                }
                            }

                            //update every drink that was a part of the recipe
                            foreach (RecipeItem part in parts)
                            {
                                //calculate and update every item
                                updateAmt += part.Quantity * amtOrdered;

                                Drink updatedDrink = (Drink) orderItemUpdateProcedure(part.Item, updateAmt);

                                databaseManager.updateDrink(updatedDrink);
                                ItemList[drinkFound] = updatedDrink;
                                updateAmt = 0.0;
                            }
                        }
                    } else
                    {
                        //flag user for unknown items the user has to update
                        string mes = name + " is unknown. Please update manually.";
                        NotificationManager.AddNotification(mes);
                    }
                }
            }
        }
        
        /// <summary>
        /// For use by the "parseAPIPOSUpdate":
        /// Takes an Item Object and a Double amount, subtracts the amount from the Item's estimate
        /// recalculates the item's status, and creates a status change notification if neccesary
        /// </summary>
        /// <returns>The updated Item Object</returns>
        private Item orderItemUpdateProcedure(Item updatedItem, double amount) {
            //update Item amount
            updatedItem.Estimate -= amount;

            //update the user if any of these is true
            if (updatedItem.Estimate < 0.0)
            {
                updatedItem.Estimate = 0.0;
            }
            
            //Since an Interface can only be instantiated as one of its child classes, Then
            //Theoretically, whatever child class updatedItem is should already have their own
            //implementation of calculate Status 
            if (updatedItem.CalculateStatus())
            {
                if (updatedItem.Status == status.belowIdeal)
                {
                    string mes = updatedItem.Name + " is below ideal level";
                    NotificationManager.AddNotification(mes);
                } else if (updatedItem.Status == status.belowPar)
                {
                    string mes = updatedItem.Name + " is below par level";
                    NotificationManager.AddNotification(mes);
                } else if (updatedItem.Status == status.empty)
                {
                    string mes = updatedItem.Name + " is empty";
                    NotificationManager.AddNotification(mes);
                }
            }

            return updatedItem;
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

        //Gets the list of all tags in the tags table
        public int GetTagList()
        {
            List<Tag> tagList = new List<Tag>();
            tagList = this.databaseManager.getTags();
            TagList = tagList;

            return 0;
        }

        //Adds a tag to the tag table
        public int AddTag(string tagName)
        {
            int tagID = this.databaseManager.addTag(tagName);
            Tag newTag = new Tag(tagID, tagName);
            TagList.Add(newTag);

            return 0;
        }

        //Removes a tag from the tag table
        public int DeleteTag(int tagID)
        {
            int removeIndex = TagList.FindIndex(x => x.ID == tagID);
            Tag removeTag = TagList[removeIndex];
            bool result = this.databaseManager.deleteTag(tagID);

            if (!result)
            {
                Console.WriteLine("Error: Tag removal failed");

                string mes = removeTag.Name + " could not be removed";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            TagList.Remove(removeTag);

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
            //Validate Recipe ItemList
            if (!validateDrinkRecipeList(newRecipe.ItemList)) {
                Console.WriteLine("Error: Recipe Item List Invalid. Not Entering Data Into Table");
                string mes = newRecipe.Name + " could not be added." + 
                "\n Error: New Recipe Item List has one or more Invalid Entries. "+
                "Not Adding New Recipe Name, Base Liquor, Or Ingredients Into Database";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            //add entry into Recipe Table
            recipeID = this.databaseManager.addRecipe(newRecipe.Name, newRecipe.BaseLiquor);
            if (recipeID == -1)
            {
                Console.WriteLine("Error: Recipe Entry Addition Failed. Stopping here");

                string mes = newRecipe.Name + " could not be added";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            //NOTE: this relies on the recipe Object sent from the frontend is a DeepCopy and not a shallow one
            //i.e. that the Item Objects in item list are copied, and not just the references
            foreach(RecipeItem component in newRecipe.ItemList) {
                int itemID = component.Item.ID;
                double itemQuantity = component.Quantity;
                itemListResult = this.databaseManager.addDrinkRecipe(recipeID, itemID, itemQuantity);

                if (itemListResult == -1) {
                    Console.WriteLine("Error: DrinkRecipe Entry Addition Failed For Entry with '"
                    + itemID + "' ItemID and '" + itemQuantity + "' failed. Stopping here");

                    string mes = component.Item.Name + " could not be added to the recipie." +
                    "\n Recipe Ingredient Data Entry Stopped There.";
                    NotificationManager.AddNotification(mes);

                    return 1;
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
            //Validate Recipe ItemList
            if (!validateDrinkRecipeList(updatedRecipe.ItemList)) {
                Console.WriteLine("Error: Recipe Item List Invalid. Not Entering Data Into Table");
                string mes = updatedRecipe.Name + " could not be added." + 
                "\n Error: Updated Recipe Item List has one or more Invalid Entries. " +
                "Not Updating Recipe Name, Base Liqour, or Ingredients In Database.";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            //add entry into Recipe Table
            recipeID = updatedRecipe.ID;
            string updatedName = updatedRecipe.Name;
            string updatedBase = updatedRecipe.BaseLiquor;
            
            if (!this.databaseManager.updateRecipe(recipeID, updatedName, updatedBase))
            {
                Console.WriteLine("Error: Recipe Entry Update Failed. Stopping here");

                string mes = updatedName + " could not be updated";
                NotificationManager.AddNotification(mes);

                return 1;
            }
            
            //deleting old DrinkRecipe table entries for this Recipe
            if (!this.databaseManager.deleteDrinkRecipesByRecipeID(recipeID)) {
                Console.WriteLine("Error: DrinkRecipe Entry Deletion Failed. Stopping here");

                string mes = updatedName + " could not be updated";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            //NOTE: this relies on the recipe Object sent from the frontend is a DeepCopy and not a shallow one
            //i.e. that the Item Objects in item list are copied, and not just the references
            foreach(RecipeItem component in updatedRecipe.ItemList) {
                int itemID = component.Item.ID;
                double itemQuantity = component.Quantity;
                itemListResult = this.databaseManager.addDrinkRecipe(recipeID, itemID, itemQuantity);

                if (itemListResult == -1) {
                    Console.WriteLine("Error: DrinkRecipe Entry Addition Failed For Entry with '"
                    + itemID + "' ItemID and '" + itemQuantity + "' failed. Stopping here");

                    string mes = component.Item.Name + " could not be updated";
                    NotificationManager.AddNotification(mes);

                    return 1;
                }
            }
            
            return 0;
        }

        //Check to make Sure An Added or Updated Recipe's DrinkRecipe is valid before adding it in 
        private bool validateDrinkRecipeList(List<RecipeItem> itemList) {
            this.GetItemList();    //ItemList must be populated and uptoDate First                    
            foreach(RecipeItem component in itemList) {
                int itemID = component.Item.ID;
                double itemQuantity = component.Quantity;
                int drinkFound = this.ItemList.FindIndex(x => x.ID == itemID);

                if (drinkFound == -1 || itemQuantity < 0) {
                    return false;
                }
            }  
            
            return true;
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

                string mes = unwantedRecipe.Name + " could not be deleted";
                NotificationManager.AddNotification(mes);

                return 1;
            }

            // delete recip entry from recipes table 
            if (!this.databaseManager.deleteRecipe(recipeID)) {
                Console.WriteLine("Error: DrinkRecipe Entry Deletion Failed. Stopping here");

                string mes = unwantedRecipe.Name + " could not be deleted";
                NotificationManager.AddNotification(mes);

                return 1;
            }
            return 0;
        }
    }
}
