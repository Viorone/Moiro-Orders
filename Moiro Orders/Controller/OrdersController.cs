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
    class OrdersController
    {
        public OrdersController() { }

        //НАЧАЛО ИНТЕРФЕЙСНЫЕ МЕТОДЫ!!!!
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

        // get All orders by current user
        // count is number of orders per page
        public async Task GetOrders(int count)
        {
            await GetAllOrdersAsync(count);
        }

        public async Task DeleteOrder(int id)
        {
            await DeleteOrderAsync(id);
        }

        public async Task EditOrder(Order order)
        {
            await UpdateOrderAsync(order);
        }

        //КОНЕЦ ИНТЕРФЕЙСНЫЕ МЕТОДЫ!!!!




        async Task<Uri> CreateOrderAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/OrdersAPI", order);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
      
        async Task<List<Order>> GetAllOrdersAsync(int count)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?userId={PublicResources.Im.Id}&count={count}");
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
