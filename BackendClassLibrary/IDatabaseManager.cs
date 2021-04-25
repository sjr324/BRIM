using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public interface IDatabaseManager
    {
        //Creates then runs a delete query
        bool deleteDrink(Drink drink);

        //Creates then runs an insert query
        bool addDrink(Drink drink);

        //Creates then runs an update query
        bool updateDrink(Drink drink);

        //Querys the database for all entries in the drinks table and returns them as a list
        List<Item> getDrinks();

        //Querys the database for all entries in the tags table and returns them as a list
        List<Tag> getTags();

        //Creates then runs a delete query for entry IN RECIPES TABLE ONLY
        //NOTE: DO NOT ATTEMPT TO USE THIS BEFORE DELETING ALL CONNECTED ENTRIES IN DRINKRECIPE TABLE 
        bool deleteRecipe(int recipeID);

        //Deletes all entries in the drinkRecipes Table with a certain recipeID
        bool deleteDrinkRecipesByRecipeID(int recipeID);

        //Delete all entries in the drinkTags Table with a certain drinkID
        bool deleteDrinkTagsByDrinkID(int drinkID);

        //Creates then runs an insert query FOR JUST THE RECIPES TABLE
        //returns the ID of the newly Inserted Recipe
        int addRecipe(string name, string baseLiquor);

        //Creates then runs an insert query FOR JUST THE DRINKRECIPES TABLE
        //ASSUMES THAT RECIPE AND DRINK IDS ARE VALID. AKA THAT THEY BELONG TO DRINKS AND RECIPES THAT EXIST 
        int addDrinkRecipe(int recipeID, int drinkID, double itemQuantity);

        //Creates and runs an insert query for just the DrinkTags table
        //Assumes that the drink and tag IDs are valid
        bool addDrinkTag(int drinkID, int tagID);

        //Creates and runs an insert query for just the tags table
        //returns the ID of the tag after it is added
        int addTag(string name);

        //Deletes a tag from the tags table by ID
        bool deleteTag(int ID);

        // Creates then runs an UPDATE query FOR ONLY THE ENTRY IN THE RECIPES TABLE
        // only sends the ID, name, and baseLiquor since RecipeObjects can get large from the itemList
        // even though Entries is Recipes Table itself only have column that will really be modified
        bool updateRecipe(int recipeID, string name, string baseLiquor);

        /* IMPORTANT!: DrinkRecipe Table Updates for am existing Recipe will be handled by Deleting and 
        all DrinkRecipe Entries related to that table and re-adding/replacing the entries with entire ItemList
        of the updated Recipe Object, for Simplicity's sake. */

        // Querys the database for all entries in the drinkrecipes, and the pertinent information  
        // of their respectively referenced Recipe and Drink entries
        List<Recipe> getRecipes();

        //Takes in the id, date, and amount for a drink and either updates an existing entry by that amount
        //or inserts a new entry into the drinkstats table
        void incrementDrinkStat(int id, string date, double amt);

        //Takes in the id, date, and amount for a recipe and either updates an existing entry by that amount
        //or inserts a new entry into the recipestats table
        void incrementRecipeStat(int id, string date, double amt);

        //Takes in an ID, a start date, and an end date. It returns a list of drink stats for that drink that are between those dates
        List<DrinkStat> getDrinkStatsByDateRange(int DrinkID, string StartDate, string EndDate);

        //Takes in an ID, a start date, and an end date. It returns a list of recipe stats for that recipe that are between those dates for that recipe
        List<RecipeStat> getRecipeStatsByDateRange(int RecipeID, string StartDate, string EndDate);

        //Takes in a start date, and an end date. It returns a list of drink stats from the drinkstats table that are between those dates
        List<DrinkStat> getAllDrinkStatsByDateRange(string StartDate, string EndDate);

        //Takes in a start date, and an end date. It returns a list of recipe stats from the recipestats table that are between those dates
        List<RecipeStat> getAllRecipeStatsByDateRange(string StartDate, string EndDate);

        //Takes in a drink id and gets a list of all entries from the DrinkStats table with that DrinkID
        //Returns that list of stats
        List<DrinkStat> getDrinkStats(int DrinkID);

        //Takes in a recipe id and gets a list of all entries from the RecipeStats table with that recipeID
        //returns that list of stats
        List<RecipeStat> getRecipeStats(int RecipeID);
    }
    
}