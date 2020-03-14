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

        public async Task<HttpStatusCode> CreateOrderAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/OrdersAPI", order);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
      
        public async Task<List<Order>> GetAllOrdersAsync(int count)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?userId={PublicResources.Im.Id}&count={count}");
            List<Order> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = await response.Content.ReadAsAsync<List<Order>>();                         
            }
            return orders;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/OrdersAPI/{order.Id}", order);
            response.EnsureSuccessStatusCode();
            order = await response.Content.ReadAsAsync<Order>();
            return order;
        }

        public async Task<HttpStatusCode> DeleteOrderAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/OrdersAPI/{id}");
            return response.StatusCode;
        }
    }
}
