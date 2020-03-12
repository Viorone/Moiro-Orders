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
    public class OrderController
    {
        public OrderController() { }

        public async Task CreateOrder()
        {
            // Update port # in the following line.

            try
            {
                // Create a new order
                Order order = new Order
                {
                    UserId = 1,
                    Problem = "Gizmo",
                    Description = "100",
                    Status = "Widgets",
                    // Date = DateTime.Now Генерится автоматически
                };
                var url = await CreateOrderAsync(order);

                MessageBox.Show(url.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!");
                //вывод сообщения
            }

        }
        async Task ChangeOrder()
        {
            // мб допилить авывод куда нитьб типо логов



            // Get the order
            Order order = await GetOrderAsync(PublicResources.client.BaseAddress.PathAndQuery);
          

            // Update the order

            order.Description = "dlfkbhdfjkhbvdfkg";
            await UpdateOrderAsync(order);

            // Get the updated order
            order = await GetOrderAsync(PublicResources.client.BaseAddress.PathAndQuery);
         

            // Delete the order


            // Get all orders

            order = null;
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/orders/");
            if (response.IsSuccessStatusCode)
            {
                var a = await response.Content.ReadAsStringAsync();
                List<Order> aa;

                aa = JsonConvert.DeserializeObject<List<Order>>(a);
                int j = 0;
                foreach (var i in aa)
                {
                
                    j++;
                }


            }

        }
        async Task DeleteOrder(Order order)
        {
            var statusCode = await DeleteOrderAsync(order.Id);
            Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");
        }

        async Task<Uri> CreateOrderAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/OrdersAPI", order);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        public async Task<Order> GetOrderAsync(string path)
        {
            Order order = null;
            HttpResponseMessage response = await PublicResources.client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                order = await response.Content.ReadAsAsync<Order>();
            }
            return order;
        }
        public async Task<List<Order>> GetAllOrderAsync()
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?userId={PublicResources.Im.Id}");
            List<Order> aa = null;
            if (response.IsSuccessStatusCode)
            {
                var a = await response.Content.ReadAsStringAsync();
              
                aa = JsonConvert.DeserializeObject<List<Order>>(a);
                MessageBox.Show(aa[0].Problem, "Это проблема");
            }
            return aa;
        }

        async Task<Order> UpdateOrderAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/OrdersAPI/{order.Id}", order);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated order from the response body.
            order = await response.Content.ReadAsAsync<Order>();
            return order;
        }

        async Task<HttpStatusCode> DeleteOrderAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/OrdersAPI/{id}");
            return response.StatusCode;
        }
    }
}
