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
        Task<HttpStatusCode> EditEvent(Event @event);                      //поставить таймер на возможность редактирования (возможно на час)
        Task<HttpStatusCode> DeleteEvent(int id);                          //поставить таймер на возможность удаления (возможно на час)

        Task<List<Order>> GetOrdersList(int count, int id);
        Task<List<Order>> GetOrdersListOfDate(int userId, DateTime date);  //Get Orders list by current date
        Task<HttpStatusCode> CreateOrder(Order order);
        Task<HttpStatusCode> EditOrder(Order order);                      //поставить таймер на возможность редактирования (возможно на час)
        Task<HttpStatusCode> DeleteOrder(int id);                          //поставить таймер на возможность удаления (возможно на час)

        Task<List<PublicChat>> GetPublicChatMessagesList(int count, int id);
        Task<HttpStatusCode> CreatePublicChatMessage(PublicChat publicChat);
        Task<HttpStatusCode> EditPublicChatMessage(PublicChat publicChat); //поставить таймер на возможность редактирования (возможно на 10 минут)
        Task<HttpStatusCode> DeletePublicChatMessage(int id);              //поставить таймер на возможность удаления (возможно на 10 минут)

        Task<HttpStatusCode> UpdateUser(User user);

    }
}
