using Escola.WPF.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Escola.WPF.Services
{
    public class ApiService
    {
        public async Task<Response> GetAsync<T>(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result,
                    };
                }

                var data = JsonConvert.DeserializeObject<List<T>>(result);

                return new Response
                {
                    IsSucess = true,
                    Result = data,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
