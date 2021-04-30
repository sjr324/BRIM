using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public class RecipeStat
    {
        public int ID;
        public string date;
        public int quantity;

        public RecipeStat()
        {
            ID = 0;
            date = "";
            quantity = 0;
        }

        public RecipeStat(DataRow dr)
        {
            ID = dr.Field<int>("DrinkID");
            date = dr.Field<DateTime>("Date").ToString("yyyy-MM-dd");
            quantity = dr.Field<int>("Quantity");
        }
    }
}
