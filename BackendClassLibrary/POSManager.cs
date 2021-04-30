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
            /*var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RCTST0000008099/orders?filter=createdTime>="
                                            + ((DateTimeOffset)time).ToUnixTimeSeconds() 
                                            + "&access_token=8fe4215a-a338-4fbe-4f74-193919caa02c");*/
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
            var client = new RestClient("https://sandbox.dev.clover.com/v3/merchants/RYBHTZ2AMPQY1/atomic_order/orders?access_token=8fe4215a-a338-4fbe-4f74-193919caa02c%22");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", 
                                    "{\"orderCart\":{\"orderType\":{\"taxable\":\"false\",\"isDefault\":\"false\",\"filterCategories\":\"false\",\"isHidden\":\"false\",\"isDeleted\":\"false\"},\"groupLineItems\":\"false\"}}", 
                                    ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
    }
}
