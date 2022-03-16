using Microsoft.Extensions.Configuration;
using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpDeletingDataClient : IHttpDeletingDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpDeletingDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task DeleteUserData(SingleStringClass obj)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(obj.Username),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{_configuration["DeletingDataService"]}",
                httpContent);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Data of User: {obj.Username} was deleted");
            }
            else
            {
                Console.WriteLine($"Data of User: {obj.Username} was not deleted");
            }
        }
    }
}
