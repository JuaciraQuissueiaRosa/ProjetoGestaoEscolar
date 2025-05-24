using Escola.WPF.Models;
using System.Net.Http;

namespace Escola.WPF.Services
{
    /// <summary>
    /// not used in the current version, but can be used to check network connectivity.
    /// </summary>
    public class NetworkService
    {
        private readonly HttpClient _client;

        public NetworkService()
        {
            _client = new HttpClient();
        }

        public async Task<Response> CheckConnectionAsync()
        {
            try
            {
                var response = await _client.GetAsync("http://clients3.google.com/generate_204");

                return new Response
                {
                    IsSucess = response.IsSuccessStatusCode
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
