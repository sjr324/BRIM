using System;
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
        public double Estimate { get; set; }
        public double ParLevel { get; set; }
        public double IdealLevel { get; set; }
        public unit Measurement { get; set; }
        public status Status { get; set; }

        //drink properties
        public int BottleSize {get; set ;}
        public string Brand {get; set;}
        public int UnitsPerCase {get; set;}
        public int? Vintage{get;set;}

        public Drink() { }

        //Data Conversion Constructor
        //takes a DataRow object(assumed for a drink) and uses the data to create a Drink Object
        //makes it less cluttered to convert results form database queries into objects
        public Drink(DataRow dr)
        {
            try {
                ID = dr.Field<int>("drinkID");
                Name = dr.Field<string>("name");
                Estimate = dr.Field<double>("estimate");
                Measurement = (unit) Enum.Parse(typeof(unit), dr.Field<string>("measurementUnit"));
                ParLevel = dr.Field<double>("parLevel");
                IdealLevel = dr.Field<double>("idealLevel");
                BottleSize = dr.Field<int>("bottleSize");
                Brand = dr.Field<string>("brand");
                UnitsPerCase = dr.Field<int>("bottlesPerCase");
                Vintage = dr.Field<int?>("vintage");
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
            status oldStatus = this.Status;
            if (this.Estimate > IdealLevel) {
                this.Status = status.aboveIdeal;
            } else if (this.Estimate <= IdealLevel && this.Estimate > ParLevel) {
                this.Status = status.belowIdeal;
            } else if (this.Estimate <= ParLevel && this.Estimate > 0) {
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
