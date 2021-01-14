using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    //The main class of the program. 
    public class Inventory
    {
        public List<Item> ItemList = new List<Item>(); //holds all items registered to this BRIM instance
        public List<Recipe> RecipeList = new List<Recipe>(); //holds all of the recipes for this BRIM instance

    }
}
