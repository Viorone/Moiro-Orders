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
using Newtonsoft.Json;

namespace Moiro_Orders.Controller
{
    class PublicChatsController
    {
        public PublicChatsController() { }

        //НАЧАЛО ИНТЕРФЕЙСНЫЕ МЕТОДЫ!!!!
        public async Task CreatePublicChat()
        {
            // Update port # in the following line.
            try
            {
                // Create a new order
                PublicChat publicChat = new PublicChat
                {
                    UserId = 1,
                    Message = "Hello World"
                };
                var url = await CreatePublicChatAsync(publicChat);
                //MessageBox.Show(url.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!");
                //вывод сообщения
            }
        }

        // get All orders by current user
        // count is number of orders per page
        public async Task GetOrders(int count)
        {
            await GetAllPublicChatsAsync(count);
        }

        public async Task DeleteOrder(int id)
        {
            await DeletePublicChatAsync(id);
        }

        public async Task EditOrder(PublicChat publicChat)
        {
            await UpdatePublicChatAsync(publicChat);
        }

        //КОНЕЦ ИНТЕРФЕЙСНЫЕ МЕТОДЫ!!!!




        async Task<Uri> CreatePublicChatAsync(PublicChat publicChat)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/PublicChatsAPI", publicChat);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        async Task<List<PublicChat>> GetAllPublicChatsAsync(int count)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/PublicChatsAPI?userId={PublicResources.Im.Id}&count={count}");
            List<PublicChat> aa = null;
            if (response.IsSuccessStatusCode)
            {
                var a = await response.Content.ReadAsStringAsync();

                aa = JsonConvert.DeserializeObject<List<PublicChat>>(a);
                MessageBox.Show(aa[0].Message, "Это проблема");
            }
            return aa;
        }

        async Task<PublicChat> UpdatePublicChatAsync(PublicChat publicChat)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/PublicChatsAPI/{publicChat.Id}", publicChat);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated order from the response body.
            publicChat = await response.Content.ReadAsAsync<PublicChat>();
            return publicChat;
        }

        async Task<HttpStatusCode> DeletePublicChatAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/PublicChatsAPI/{id}");
            return response.StatusCode;
        }
    }
}
