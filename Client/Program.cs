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
            GetAllAsync(null).GetAwaiter().GetResult();
            GetAllAsync(users[0]).GetAwaiter().GetResult();
            GetOneAsync(null).GetAwaiter().GetResult();
            GetOneAsync(users[1]).GetAwaiter().GetResult();
            PostAsync(null).GetAwaiter().GetResult(); // Should fail
            foreach (var u in users)
            {
                PostAsync(u).GetAwaiter().GetResult(); // One should fail, one succeed
            }
        }

        private static async Task GetAllAsync(string user)
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            TokenResponse tokenResponse;
            if (user == null) // To check if unauthenticated/anonymous user can access the /api/courses get method
            {
                tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            }
            else
            {
                tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(user, "password", "api1");
            }
                
            if (user != null && tokenResponse.IsError)
            {
                Console.WriteLine("ERROR getting token for user: " + user + " - " + tokenResponse.Error);
                return;
            }
            
            // call api
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5001");

            if (user != null)
            {
                client.SetBearerToken(tokenResponse.AccessToken);
            }

            var response = await client.GetAsync("/api/courses");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("ERROR getting courses for user: " + (user ?? "null") + " - " + response.StatusCode);
                return;
            }

            var content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("SUCCESS getting courses for user: " + (user ?? "null") + " - content:");
            Console.WriteLine(JArray.Parse(content));
        }

        private static async Task GetOneAsync(string user)
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            TokenResponse tokenResponse;
            if (user == null) // To check if unauthenticated/anonymous user can access the /api/courses get method
            {
                tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            }
            else
            {
                tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(user, "password", "api1");
            }

            if (user != null && tokenResponse.IsError)
            {
                Console.WriteLine("ERROR getting token for user: " + user + " - " + tokenResponse.Error);
                return;
            }

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5001");

            if (user != null)
            {
                client.SetBearerToken(tokenResponse.AccessToken);
            }
            
            var response = await client.GetAsync("/api/courses/1");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("ERROR getting course nr. 1 for user: " + (user ?? "null") + " - " + response.StatusCode);
                return;
            }

            var content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("SUCCESS getting course nr. 1 for user: " + (user ?? "null") + " - content:");
            Console.WriteLine(JObject.Parse(content));
        }

        private static async Task PostAsync(string user)
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            TokenResponse tokenResponse;
            if (user == null) // To check if unauthenticated/anonymous user can access the /api/courses get method
            {
                tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            }
            else
            {
                tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(user, "password", "api1");
            }

            if (user != null && tokenResponse.IsError)
            {
                Console.WriteLine("ERROR getting token for user: " + user + " - " + tokenResponse.Error);
                return;
            }

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            client.BaseAddress = new Uri("http://localhost:5001");

            var response = await client.PostAsync("/api/courses", new StringContent("", Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("ERROR posting as user: " + (user ?? "null") + " - " + response.StatusCode);
                return;
            }
            Console.WriteLine("SUCCESS posting as user: " + (user ?? "null"));
        }
    }
}
