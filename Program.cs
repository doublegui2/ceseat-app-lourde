using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using System.Windows.Forms;


namespace AppLourde
{
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Roles
    {
        public string _id { get; set; }
        public string name { get; set; }
    }

    public class User
    {
        public List<Roles> roles { get; set; }
        public Boolean suspended { get; set; }
        public string _id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public List<Object> cart { get; set; }
        public int __v { get; set; }
        public string token { get; set; }
    }

    public class LoggedUser
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

    public class OrderStats
    {
        public float ordersCount { get; set; }
        public float summOrdersAmount { get; set; }
        public float AverageOrderAmount { get; set; }
    }
    public class Query
    {
        public string token { get; set; }
    }

    public class Request
    {
        public Query query { get; set; }
        public LoggedUser replacement { get; set; }
    }

    static class Program
    {
        static HttpClient client = new HttpClient();
        
      
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LogIn());
        }
    }

    public class Methods
    {
        public async Task<HttpResponseMessage> LogIn(UserInfo userinfo)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://84.7.195.152:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/v1/auth/signin", userinfo);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateUsername(string token, LoggedUser userUpdated)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://84.7.195.152:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            UserInfo userinfo = new UserInfo
            {
                username = "admin",
                password = "admin_ceseat"
            };
            var tmp = await LogIn(userinfo);
            var tmp2 = await tmp.Content.ReadFromJsonAsync<User>();
            var token2 = tmp2.token;
            //string tokenv2 = 
            client.DefaultRequestHeaders.Add("x-access-token", token2);
            /*client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));*/
            string userUpdatedJSON = JsonSerializer.Serialize(userUpdated);
            Query query = new Query
            {
                token = token2
            };
            Request request = new Request
            {
                query = query,
                replacement = userUpdated
            };
            //Debug.WriteLine(userUpdatedJSON);
            //string test = JsonSerializer.Serialize(request);
            //Debug.WriteLine(test);
            var stringContent = new StringContent(userUpdatedJSON, System.Text.Encoding.UTF8, "application/json-patch+json");
            Debug.WriteLine(stringContent);
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/v1/admin/update/user", request);
            return response;
        }

        public async Task<OrderStats> GetOrderStats(string token)
        {
            OrderStats orderStats;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://84.7.195.152:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("x-access-token", token);
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            orderStats = await client.GetFromJsonAsync<OrderStats>("api/v1/get/stats");
            return orderStats;
        }
        
    }

}
