using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using Newtonsoft.Json;

namespace Moiro_Orders.Controller
{
    class PublicChatsController
    {
        public PublicChatsController() { }

        public async Task<HttpStatusCode> CreatePublicChatAsync(PublicChat publicChat)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/PublicChatsAPI", publicChat);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<List<PublicChat>> GetAllPublicChatsAsync(int count, int id)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/PublicChatsAPI?userId={id}&count={count}");
            List<PublicChat> publicChats = null;
            if (response.IsSuccessStatusCode)
            {
                publicChats = await response.Content.ReadAsAsync<List<PublicChat>>();
            }
            return publicChats;
        }

        public async Task<HttpStatusCode> UpdatePublicChatAsync(PublicChat publicChat)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/PublicChatsAPI/{publicChat.Id}", publicChat);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeletePublicChatAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/PublicChatsAPI/{id}");
            return response.StatusCode;
        }
    }
}
