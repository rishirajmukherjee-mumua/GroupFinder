using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MVC_EF_Start.Models
{
    public class SpotifyDataAccess
    {

        public string ClientSecret { get; set; }



        public string ClientId { get; set; }



        public SpotifyDataAccess(string clientId, string clientSecret)

        {

            ClientId = clientId;

            ClientSecret = clientSecret;

        }


        public async Task<Token> GetToken()

        {

            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret));



            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()

            {

                new KeyValuePair<string, string>("grant_type", "client_credentials")

            };


            
            HttpClient client = new HttpClient();

            
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

            HttpContent content = new FormUrlEncodedContent(args);



            HttpResponseMessage resp = await client.PostAsync("https://accounts.spotify.com/api/token", content);

            string msg = await resp.Content.ReadAsStringAsync();



            return JsonConvert.DeserializeObject<Token>(msg);

        }

        public async Task<String> SearchArtistAndTrack(String token, String search)
        {
            HttpClient client = new HttpClient();
            
            //UriBuilder builder = new UriBuilder("https://api.spotify.com/v1/search");
            //builder.Query = "name='abc'&password='cde'";
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            //HttpContent content = new FormUrlEncodedContent(args);

            //client.BaseAddress = new Uri("https://api.spotify.com/v1/search");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue
                ("application/json"));

            var builder = new UriBuilder("https://api.spotify.com/v1/search");
            //builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["q"] = $"{search}";
            query["type"] = "track,artist";
            query["market"] = "US";
            query["limit"] = "10";
            query["offset"] = "0";
            builder.Query = query.ToString();
            string url = builder.ToString();


            HttpResponseMessage resp = await client.GetAsync(url);
            string msg = await resp.Content.ReadAsStringAsync();
            //JObject jObject = JObject.Parse(msg);
            return msg;
        }
        



    }
}
