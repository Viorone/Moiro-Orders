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
    class EventsController
    {
        public EventsController() { }


        //НАЧАЛО ИНТЕРФЕЙСНЫЕ МЕТОДЫ!!!!
        public async Task CreateEvent()
        {

            try
            {
                // Create a new order
                Event @event = new Event
                {
                    UserId = 1,
                    NameEvent = "Прикол",
                    Description = "100",
                    Status = "Widgets",
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now,
                    Place = "General place"
                };
                var url = await CreateEventAsync(@event);
                //MessageBox.Show(url.ToString());
                // path messege
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!");
                //вывод сообщения
            }
        }

        // get All events by current user
        // count is number of events per page
        public async Task GetEvents(int count) 
        {
            await GetAllEventsAsync(count);
        }    
        
        public async Task DeleteEvent(int id)
        {
            await DeleteEvetntAsync(id);
        }

        public async Task EditEvent(Order order)
        {
            await UpdateEvetntAsync(order);
        }
        //КОНЕЦ ИНТЕРФЕЙСНЫЕ МЕТОДЫ!!!!




        async Task<Uri> CreateEventAsync(Event @event)
        {
            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/EventsAPI", @event);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
       
        async Task<List<Event>> GetAllEventsAsync(int count)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/EventsAPI?userId={PublicResources.Im.Id}&count={count}");
            List<Event> aa = null;
            if (response.IsSuccessStatusCode)
            {
                var a = await response.Content.ReadAsStringAsync();

                aa = JsonConvert.DeserializeObject<List<Event>>(a);
                MessageBox.Show(aa[0].NameEvent, "Это название мероприятия");
            }
            return aa;
        }

        async Task<Order> UpdateEvetntAsync(Order order)
        {
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/EventsAPI/{order.Id}", order);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated order from the response body.
            order = await response.Content.ReadAsAsync<Order>();
            return order;
        }

        async Task<HttpStatusCode> DeleteEvetntAsync(int id)
        {
            HttpResponseMessage response = await PublicResources.client.DeleteAsync(
                $"api/EventsAPI/{id}");
            return response.StatusCode;
        }
    }
}
