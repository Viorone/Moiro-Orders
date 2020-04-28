using Moiro_Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moiro_Orders.Controller
{
    class WebinarsController
    {
        public WebinarsController() { }

        public async Task<HttpStatusCode> CreateWebinarAsync(Webinar webinar)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/WebinarsAPI", webinar);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<List<Webinar>> GetWebinarsListOfDateAsync(int userId, DateTime date)
        {
            string d = date.ToString();
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/WebinarsAPI?userId={userId}&date={d}");
            List<Webinar> webinars = null;
            if (response.IsSuccessStatusCode)
            {
                webinars = await response.Content.ReadAsAsync<List<Webinar>>();
            }
            return webinars;
        }
        public async Task<List<Webinar>> GetAllWebinarsTodayAsync(DateTime date)
        {
            string d = date.ToString();
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/WebinarsAPI?date={d}");
            List<Webinar> webinars = null;
            if (response.IsSuccessStatusCode)
            {
                webinars = await response.Content.ReadAsAsync<List<Webinar>>();
            }
            return webinars;
        }

        public async Task<List<Webinar>> GetAllWebinarsAsync(int count, int id)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync(
                $"api/WebinarsAPI?userId={id}&count={count}");
            List<Webinar> webinars = null;
            if (response.IsSuccessStatusCode)
            {
                webinars = await response.Content.ReadAsAsync<List<Webinar>>();
            }
            return webinars;
        }

        public async Task<HttpStatusCode> UpdateWebinarAsync(Webinar webinar)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/WebinarsAPI/{webinar.Id}", webinar);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteWebinarAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/WebinarsAPI/{id}");
            return response.StatusCode;
        }
    }
}
