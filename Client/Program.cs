using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace A06.Client
{
    public class Program
    {
        private static readonly List<string> users = new List<string>
        {
            "alice", "bob"
        };

        public static void Main(string[] args)
        {
            foreach (var u in users)
            {
                GetAsync(u).GetAwaiter().GetResult();
                PostAsync(u).GetAwaiter().GetResult();
            }
        }

        private static async Task GetAsync(string user)
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(user, "password", "api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");
            
            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/api/courses");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }

            var content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(JArray.Parse(content));

            Console.WriteLine("Let's see if this works...");
            var m = "{\"id\": 2, \"name\": \"Vefþjónustur\", \"semester\": \"20153\"}";
            var c = new StringContent(m, Encoding.UTF8);
            var r = await client.PostAsync("http://localhost:5001/api/courses", c);
            if (!r.IsSuccessStatusCode)
            {
                Console.WriteLine(r.StatusCode);
            }
            var ct = r.Content.ReadAsStringAsync().Result;
            Console.WriteLine(JArray.Parse(ct));
        }

        private static async Task PostAsync(string user)
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(user, "password", "api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
        }
    }
}
