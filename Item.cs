using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    interface Item
    {
        enum unit { oz, ml } //standard measurement units
        enum status { empty, belowPar, belowIdeal, aboveIdeal } //the status of an inventory item

        String Name { get; set; }
        double Price { get; set; }
        double LowerEstimate { get; set; }
        double UpperEstimate { get; set; }
        double ParLevel { get; set; }
        double IdealLevel { get; set; }
        unit Measurement { get; set; }
        status Status { get; set; }

        void CalculateStatus();
    }
}
