using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public class Recipe
    {
        public int ID;
        public string Name;
        public string BaseLiquor;

        //A list of tuples that stores an item and the quantity of it used to make a recipie
        public List<RecipeItem> ItemList = new List<RecipeItem>();

        //had to explicitly make a default constructor to make a test recipe to mess with the Add and Updates
        public Recipe() {}

        //Data Conversion Constructor
        //takes an IEnumerable of DataRow objects(All assumed to be in the form of results from the query
        //in the getRecipes function in DatabaseManager and uses the data to create a recipe Object and 
        //populate it's ItemList
        //makes it less cluttered to convert results form database queries into objects
        public Recipe(int recipeID, string recipeName, string baseLiquor, IEnumerable<DataRow> itemListData) {
            ID = recipeID;
            Name = recipeName;
            BaseLiquor = baseLiquor;
            
            foreach(DataRow dr in itemListData) {
                //I kept the column for the drink's name as just "name" so I could piggyback off of the 
                //Constructor in Drink
                Drink tempDrink = new Drink(dr);
                double tempQuantity = dr.Field<double>("itemQuantity");
                ItemList.Add(new RecipeItem(tempDrink, tempQuantity));
            }
        }
    }
}
