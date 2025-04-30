using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Escola.WPF.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService(string baseUrl = "https://schoolapi.azurewebsites.net/")
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<T>> GetAsync<T>(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public async Task PostAsync<T>(string endpoint, T item)
        {
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutAsync<T>(string endpoint, T item)
        {
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string endpoint)
        {
            var response = await _client.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}
