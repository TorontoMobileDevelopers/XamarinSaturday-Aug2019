using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using exercise_one.Models;
using Newtonsoft.Json;

namespace exercise_one.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;

        private static HttpClient _instance;

        private static HttpClient HttpClientInstance => _instance ?? (_instance = new HttpClient());


        public MockDataStore()
        {
            items = new List<Item>();

        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            var uri = "https://xamarinsatapi.azurewebsites.net/api/TodoItem";
            HttpResponseMessage response = await HttpClientInstance.GetAsync(uri)
                                                                   .ConfigureAwait(false);

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();
            var result = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Item>>(serialized))
                                       .ConfigureAwait(false);

            return result;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync()
                                            .ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception(content);
                }

                throw new HttpRequestException(content);
            }
        }
    }
}