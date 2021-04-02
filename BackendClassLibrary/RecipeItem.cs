using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public class RecipeItem
    {
        public Drink Item;
        public double Quantity;

        public RecipeItem(Drink item, double quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
