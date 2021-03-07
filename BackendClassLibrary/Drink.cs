﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace BRIM.BackendClassLibrary
{
    public class Drink : Item
    {
        //interface properties
        public int ID { get; set; }
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
        public Boolean Vintage;

        public Drink() { }

        //Data Conversion Constructor
        //takes a DataRow object(assumed for a drink) and uses the data to create a Drink Object
        //makes it less cluttered to convert results form database queries into objects
        public Drink(DataRow dr)
        {
            try {
                ID = dr.Field<int>("drinkID");
                Name = dr.Field<string>("name");
                LowerEstimate = dr.Field<double>("lowerEstimate");
                UpperEstimate = dr.Field<double>("upperEstimate");
                Measurement = (unit) Enum.Parse(typeof(unit), dr.Field<string>("measurementUnit"));
                ParLevel = dr.Field<double>("parLevel");
                IdealLevel = dr.Field<double>("idealLevel");
                BottleSize = dr.Field<int>("bottleSize");
                Brand = dr.Field<string>("brand");
                UnitsPerCase = dr.Field<int>("bottlesPerCase");
                //TODO: Re-enable vintage
                //Vintage = Convert.ToBoolean(dr.Field<SByte>("vintage"));
                Vintage = true;
                Price = dr.Field<double>("price");
            } catch (IndexOutOfRangeException exp) {
                Console.WriteLine("The Datarow given does not contain one or more of the columns in a Drink Object");
                Console.WriteLine(exp.Message);
            }
            CalculateStatus();
        }

        //recalculates Item's status based on it's quantity range
        //returns True if the It's Status changes from what it was before the method was called, 
        //otherwise returns False
        public Boolean CalculateStatus()
        {
            //average value (should) provide a good balance between False Positive and False Negative Risk
            double averageQuantity = (LowerEstimate + UpperEstimate) / 2;
            status oldStatus = this.Status;
            if (averageQuantity > IdealLevel) {
                this.Status = status.aboveIdeal;
            } else if (averageQuantity <= IdealLevel && averageQuantity > ParLevel) {
                this.Status = status.belowIdeal;
            } else if (averageQuantity <= ParLevel && averageQuantity > 0) {
                this.Status = status.belowPar;
            } else {
                this.Status = status.empty;
            }

            if (oldStatus != this.Status) {
                return true;
            }
            return false;
        }
    }
}
