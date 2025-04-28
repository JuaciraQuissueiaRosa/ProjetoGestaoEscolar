using Escola.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Services
{
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
