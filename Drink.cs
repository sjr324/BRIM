using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    public class Drink : Item
    {
        //interface properties
        public string Name { get; set; }
        public double Price { get; set; }
        public double LowerEstimate { get; set; }
        public double UpperEstimate { get; set; }
        public double ParLevel { get; set; }
        public double IdealLevel { get; set; }
        public unit Measurement { get; set; }
        public status Status { get; set; }

        //drink properties
        public int BottleSize;
        public string Brand;
        public int UnitsPerCase;
        public int Vintage;

        public void CalculateStatus()
        {
            
        }
    }
}
