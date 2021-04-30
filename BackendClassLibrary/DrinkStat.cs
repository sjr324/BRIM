using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public class DrinkStat
    {
        public int ID;
        public string date;
        public double quantity;

        public DrinkStat() 
        {
            ID = 0;
            date = "";
            quantity = 0;
        }

        public DrinkStat(DataRow dr)
        {
            ID = dr.Field<int>("DrinkID");
            date = dr.Field<DateTime>("Date").ToString("yyyy-MM-dd");
            quantity = dr.Field<double>("Quantity");
        }
    }
}
