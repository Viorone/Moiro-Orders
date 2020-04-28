using Moiro_Orders.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Moiro_Orders.Roles
{
    interface IAdmin
    {
        Task<List<Event>> GetEventsList(int count, int id);  //Get Events list by current user
        Task<HttpStatusCode> EditEvent(Event @event); //Change Event Status
        Task<List<Event>> GetAllEventsToday(DateTime date);
        Task<List<Event>> GetEventsListOfDate(int userId, DateTime date); 

        Task<HttpStatusCode> EditWebinar(Webinar webinar); 
        Task<List<Webinar>> GetAllWebinarsToday(DateTime date);
        Task<List<Webinar>> GetWebinarsListOfDate(int userId, DateTime date); 

        Task<List<Order>> GetOrdersList(int count, int id); //Get Orders list by current user
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetAllOrdersToday(DateTime date);
        Task<List<Order>> GetOrdersListOfDate(int userId, DateTime date); //Get Orders list by current date
        Task<HttpStatusCode> EditOrder(Order order); //Change Order Status

        Task<List<PublicChat>> GetPublicChatMessagesList(int count, int id);
        Task<HttpStatusCode> CreatePublicChatMessage(PublicChat publicChat);
        Task<HttpStatusCode> EditPublicChatMessage(PublicChat publicChat);
        Task<HttpStatusCode> DeletePublicChatMessage(int id);

        List<User> GetNewADUsersList();
        Task<List<User>> GetAllUserName();
        Task<HttpStatusCode> UpdateUsersDb(User user);

        Task<List<Status>> GetStatuses();
    }
}
