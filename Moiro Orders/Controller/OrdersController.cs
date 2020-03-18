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
      
        public async Task<List<Order>> GetOrdersListOfDateAsync(int userId, DateTime date)
        {
            string d = date.ToString();
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?userId={userId}&date={d}");
            List<Order> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = await response.Content.ReadAsAsync<List<Order>>();
            }
            return orders;
        }
        // изменить контроллер на серваке
        public async Task<List<Order>> GetAllOrdersTodayAsync( DateTime date)
        {
            string d = date.ToString();
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?date={date}");
            List<Order> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = await response.Content.ReadAsAsync<List<Order>>();
            }
            return orders;
        }

        public async Task<List<Order>> GetAllOrdersAsync(int count, int id)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?userId={id}&count={count}");
            List<Order> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = await response.Content.ReadAsAsync<List<Order>>();
            }
            return orders;
        }

        public async Task<HttpStatusCode> UpdateOrderAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/OrdersAPI/{order.Id}", order);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteOrderAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/OrdersAPI/{id}");
            return response.StatusCode;
        }
    }
}
