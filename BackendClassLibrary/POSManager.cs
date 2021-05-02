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
            /*
            //this code his here to manually create an order if needed
            var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RYBHTZ2AMPQY1/order_types?access_token=8fe4215a-a338-4fbe-4f74-193919caa02c");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", 
                                    "{\"taxable\":\"false\",\"isDefault\":true,\"filterCategories\":\"false\",\"isHidden\":\"false\",\"isDeleted\":\"false\",\"id\":" + 
                                    "\"TestType\",\"labelKey\":\"TestType\",\"label\":\"TestType\",\"hoursAvailable\":\"ALL\"}", 
                                    ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            
            //This code is here to view order type information if needed
            var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RYBHTZ2AMPQY1/order_types?access_token=8fe4215a-a338-4fbe-4f74-193919caa02c");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            IRestResponse response = client.Execute(request);
            */

            var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RYBHTZ2AMPQY1/atomic_order/orders?access_token=8fe4215a-a338-4fbe-4f74-193919caa02c");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json",
                                    "{\"orderCart\":{\"lineItems\":[{\"item\":{\"id\":\"V341Y5136FFMP\"},\"printed\":\"false\",\"exchangedLineItem\":{},\"exchanged\":\"false\"," + 
                                    "\"modifications\":[{\"modifier\":{\"price\":\"0\",\"modifierGroup\":{}},\"id\":\"7\",\"name\":\"Test (1.5 oz)\"}],\"refunded\":\"false\"," + 
                                    "\"refund\":{\"orderRef\":{},\"device\":{},\"payment\":{},\"employee\":{},\"overrideMerchantTender\":{},\"serviceChargeAmount\":{},\"germanInfo\"" + 
                                    ":{},\"appTracking\":{},\"cardTransaction\":{\"extra\":{},\"vaultedCard\":{\"first6\":\"777777\",\"last4\":\"7777\"}},\"transactionInfo\":{" + 
                                    "\"identityDocument\":{\"payment\":{}},\"isTokenBasedTx\":\"false\",\"emergencyFlag\":\"false\",\"promotionalMessage\":{},\"sepaElvTransactionInfo\"" +
                                    ":{}},\"merchant\":{}},\"isRevenue\":\"false\",\"printGroup\":{},\"name\":\"TestDrink1\",\"quantitySold\":2}],\"orderType\":{\"taxable\":true," + 
                                    "\"isDefault\":true,\"filterCategories\":\"false\",\"isHidden\":\"false\",\"isDeleted\":\"false\",\"label\":\"Test\",\"id\":\"1Z288B5A2B0F8\"," + 
                                    "\"labelKey\":\"com.clover.order.type.dine_in\",\"hoursAvailable\":\"BUSINESS\",\"systemOrderTypeId\":\"DINE-IN-TYPE\"}," + 
                                    "\"groupLineItems\":\"false\",\"currency\":\"USD\",\"title\":\"test1\"}}", 
                                    ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JObject result = JsonConvert.DeserializeObject<JObject>(response.Content);
        }
    }
}
