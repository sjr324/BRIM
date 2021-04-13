using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public enum unit { milliliters, ounces } //standard measurement units
    public enum status { empty, belowPar, belowIdeal, aboveIdeal } //the status of an inventory item

    public interface Item
    {
        int ID { get; set; }
        string Name { get; set; }
        double Price { get; set; }
        double Estimate { get; set; }
        double ParLevel { get; set; }
        double IdealLevel { get; set; }
        unit Measurement { get; set; }
        status Status { get; set; }
        List<Tag> Tags { get; set; }

        Boolean CalculateStatus();
    }
}
