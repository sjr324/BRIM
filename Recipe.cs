using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    public class Recipe
    {
        public string Name;

        //A list of tuples that stores an item and the quantity of it used to make a recipie
        public List<(Item, double)> ItemList = new List<(Item item, double quantity)>();
    }
}
