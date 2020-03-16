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
    class EventsController
    {
        public EventsController() { }

        public async Task<HttpStatusCode> CreateEventAsync(Event @event)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/EventsAPI", @event);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
       
       public async Task<List<Event>> GetAllEventsAsync(int count, int id)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync(
                $"api/EventsAPI?userId={id}&count={count}");
            List<Event> events = null;
            if (response.IsSuccessStatusCode)
            {
                events = await response.Content.ReadAsAsync<List<Event>>();            
            }
            return events;
        }

        public async Task<HttpStatusCode> UpdateEvetntAsync(Event @event)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/EventsAPI/{@event.Id}", @event);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteEvetntAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/EventsAPI/{id}");
            return response.StatusCode;
        }
    }
}
