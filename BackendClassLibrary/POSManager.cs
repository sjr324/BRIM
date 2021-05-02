using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BRIM.BackendClassLibrary
{
    public class POSManager : IPOSManager
    {
        public JObject GetAllOrders(DateTime time)
        {
            var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RYBHTZ2AMPQY1/orders?filter=createdTime>="
                                            + ((DateTimeOffset)time).ToUnixTimeSeconds()
                                            + "&access_token=8fe4215a-a338-4fbe-4f74-193919caa02c");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<JObject>(response.Content);
        }

        public void SendOrder() 
        {
            var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RYBHTZ2AMPQY1/atomic_order/orders?access_token=8fe4215a-a338-4fbe-4f74-193919caa02c");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json",
                                    "{\"orderCart\":{\"lineItems\":[{\"item\":{\"id\":\"01\"},\"printed\":\"false\",\"exchangedLineItem\":{},\"exchanged\":\"false\"," + 
                                    "\"modifications\":[{\"modifier\":{\"price\":\"0\",\"modifierGroup\":{}},\"id\":\"7\",\"name\":\"Test (1.5 oz)\"}],\"refunded\":\"false\"" + 
                                    ",\"refund\":{\"orderRef\":{},\"device\":{},\"payment\":{},\"employee\":{},\"overrideMerchantTender\":{},\"serviceChargeAmount\":{}," + 
                                    "\"germanInfo\":{},\"appTracking\":{},\"cardTransaction\":{\"extra\":{},\"vaultedCard\":{\"first6\":\"777777\",\"last4\":\"7777\"}},\"" + 
                                    "transactionInfo\":{\"identityDocument\":{\"payment\":{}},\"isTokenBasedTx\":\"false\",\"emergencyFlag\":\"false\",\"promotionalMessage\":{},\"" + 
                                    "sepaElvTransactionInfo\":{}},\"merchant\":{}},\"isRevenue\":\"false\",\"printGroup\":{},\"id\":\"01\",\"name\":\"TestDrink1\",\"quantitySold\":2}],\"" + 
                                    "orderType\":{\"taxable\":\"false\",\"isDefault\":\"false\",\"filterCategories\":\"false\",\"isHidden\":\"false\",\"isDeleted\":\"false\"},\"" + 
                                    "groupLineItems\":\"false\",\"id\":\"01\",\"currency\":\"USD\",\"title\":\"test1\"}}", 
                                    ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JObject result = JsonConvert.DeserializeObject<JObject>(response.Content);
        }

        //This will take in information on the items in the future
        //Used to add the inventory items to the database so they can be ordered. Used for testing purposes
        public void CreateItem()
        {
            var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RYBHTZ2AMPQY1/bulk_items?access_token=8fe4215a-a338-4fbe-4f74-193919caa02c");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", 
                                    "{\"items\":[{\"hidden\":\"false\",\"itemGroup\":{},\"defaultTaxRates\":\"true\",\"isRevenue\":\"false\",\"canonical\":{},\"itemStock\":{" + 
                                    "\"item\":{\"id\":\"01\"},\"quantity\":1000},\"id\":\"01\",\"name\":\"TestDrink1\",\"price\":0},{\"hidden\":\"false\",\"itemGroup\":{}," + 
                                    "\"defaultTaxRates\":\"true\",\"isRevenue\":\"false\",\"canonical\":{},\"itemStock\":{\"item\":{\"id\":\"02\"},\"quantity\":1000},\"id\":\"02\"," + 
                                    "\"name\":\"TestDrink2\",\"price\":0}]}", 
                                    ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JObject result = JsonConvert.DeserializeObject<JObject>(response.Content);
        }
    }
}
