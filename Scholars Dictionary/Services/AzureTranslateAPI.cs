using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Scholars_Dictionary.Services
{
    public static class AzureTranslateAPI
    {
        private static readonly string key = "f3f8e9960a444c45bef59bb842a1ef27";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";

        private static readonly string location = "westeurope";

        public static async Task<string> TranslateText(string text, string sourceLanguage, string targetLanguage)
        {
            string route = $"/translate?api-version=3.0&from={sourceLanguage}&to={targetLanguage}&to=zu";
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
