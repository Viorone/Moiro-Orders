using Moiro_Orders.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Moiro_Orders.Roles
{
    interface IUser
    {
        Task<List<Event>> GetEventsList(int count, int id);
        Task<List<Event>> GetEventsListOfDate(int userId, DateTime date);
        Task<HttpStatusCode> CreateEvent(Event @event);
        Task<HttpStatusCode> EditEvent(Event @event);                      
        Task<HttpStatusCode> DeleteEvent(int id);

        Task<List<Webinar>> GetWebinarsListOfDate(int userId, DateTime date);
        Task<HttpStatusCode> CreateWebinar(Webinar webinar);
        Task<HttpStatusCode> EditWebinar(Webinar webinar);

        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetOrdersListOfDate(int userId, DateTime date);  //Get Orders list by current date
        Task<HttpStatusCode> CreateOrder(Order order);
        Task<HttpStatusCode> EditOrder(Order order);                      
        Task<HttpStatusCode> DeleteOrder(int id);                          

        Task<List<PublicChat>> GetPublicChatMessagesList(int count, int id);
        Task<HttpStatusCode> CreatePublicChatMessage(PublicChat publicChat);
        Task<HttpStatusCode> EditPublicChatMessage(PublicChat publicChat); 
        Task<HttpStatusCode> DeletePublicChatMessage(int id);              

        Task<HttpStatusCode> UpdateUser(User user);

    }
}
