using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication4
{
    public class ServiceTranslator
    {
        private readonly IConfiguration _config;

        public ServiceTranslator(IConfiguration config)
        {;
            _config = config;
        }

        public async Task<string> TranslateText(string text, string to)
        {
            var key = _config["SubscriptionKey"];
            var endpoint = _config["Endpoint"];
            var location = _config["Region"];
            string route = $"/translate?api-version=3.0&to={to}";
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
