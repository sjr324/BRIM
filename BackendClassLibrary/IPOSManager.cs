using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BRIM.BackendClassLibrary
{
    public interface IPOSManager
    {
        //Takes in a DateTime and grabs all orders on or ater that time
        //Returns the JObject that comes from the the REST request to Clover
        JObject GetAllOrders(DateTime time);

        //This will take in information on an order in the future
        //Most likely will be used to create orders in clover to be used for testing purposes
        public void SendOrder();

        //This will take in information on the items in the future
        //Used to add the inventory items to the database so they can be ordered. Used for testing purposes
        public void CreateItem();
    }
}
