using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moiro_Orders.Models;

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

        public async Task<List<Order>> GetNotConfirmedOrdersListAsync(int userId, int statusId)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?userId={userId}&statusId={statusId}");
            List<Order> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = await response.Content.ReadAsAsync<List<Order>>();
            }
            return orders;
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(int statusId, DateTime tmpDateStart, DateTime tmpDateEnd)
        {
            string dateStart = tmpDateStart.ToString();
            string dateEnd = tmpDateEnd.ToString();
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?statusId={statusId}&dateStart={dateStart}&dateEnd={dateEnd}");
            List<Order> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = await response.Content.ReadAsAsync<List<Order>>();
            }
            return orders;
        }

        public async Task<List<Order>> GetAllOrdersTodayAsync(DateTime date)
        {
            string d = date.ToString();
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?date={d}");
            List<Order> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = await response.Content.ReadAsAsync<List<Order>>();
            }
            return orders;
        }

        public async Task<int> GetCountOrdersByStatusAsync(int statusId)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?statusId={statusId}");
            int status = 0;
            if (response.IsSuccessStatusCode)
            {
                status = await response.Content.ReadAsAsync<int>();
            }
            return status;
        }

        public async Task<int> GetCountNotConfirmedOrdersAsync(int stId, int usId)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/OrdersAPI?stId={stId}&usId={usId}");
            int status = 0;
            if (response.IsSuccessStatusCode)
            {
                status = await response.Content.ReadAsAsync<int>();
            }
            return status;
        }

        public async Task<HttpStatusCode> UpdateOrderAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/OrdersAPI/{order.Id}", order);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync(
                $"api/OrdersAPI/{id}");
            response.EnsureSuccessStatusCode();
            Order order = null;
            if (response.IsSuccessStatusCode)
            {
                order = await response.Content.ReadAsAsync<Order>();
            }
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
