﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    public class Recipe
    {
        public int ID;
        public string Name;

        //A list of tuples that stores an item and the quantity of it used to make a recipie
        public List<(Item, double)> ItemList = new List<(Item item, double quantity)>();

        //Data Conversion Constructor
        //takes an IEnumerable of DataRow objects(All assumed to be in the form of results from the query
        //in the getRecipes function in DatabaseManager and uses the data to create a recipe Object and 
        //populate it's ItemList
        //makes it less cluttered to convert results form database queries into objects
        public Recipe(int recipeID, string recipeName, IEnumerable<DataRow> itemListData) {
            ID = recipeID;
            Name = recipeName;
            
            foreach(DataRow dr in itemListData) {
                //I kept the column for the drink's name as just "name" so I could piggyback off of the 
                //Constructor in Drink
                Drink tempDrink = new Drink(dr);
                double tempQuantity = dr.Field<double>("itemQuantity");
                ItemList.Add((tempDrink, tempQuantity));
            }
        }
    }
}