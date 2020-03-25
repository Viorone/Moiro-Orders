using Moiro_Orders.Controller;
using Moiro_Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moiro_Orders.Roles
{
    class CurrentUser : IUser, IAdmin
    {
        public CurrentUser() { }
    

        #region Events
        //EVENTS
        public async Task<HttpStatusCode> CreateEvent(Event @event)
        {
            EventsController eventsController = new EventsController();
            var status = await eventsController.CreateEventAsync(@event);
            return status;
        }

        public async Task<List<Event>> GetEventsList(int count, int id)
        {
            EventsController eventsController = new EventsController();
            var events = await eventsController.GetAllEventsAsync(count, id);
            return events;
        }

        public async Task<List<Event>> GetEventsListOfDate(int userId, DateTime date)
        {
            EventsController eventsController = new EventsController();
            var events = await eventsController.GetEventsListOfDateAsync(userId, date);
            return events;
        }

        public async Task<List<Event>> GetAllEventsToday(DateTime date)
        {
            EventsController eventsController = new EventsController();
            var events = await eventsController.GetAllEventsTodayAsync(date);
            return events;
        }

        public async Task<HttpStatusCode> EditEvent(Event @event)
        {
            EventsController eventsController = new EventsController();
            var status = await eventsController.UpdateEvetntAsync(@event);
            return status;
        }

        public async Task<HttpStatusCode> DeleteEvent(int id)
        {
            EventsController eventsController = new EventsController();
            var status = await eventsController.DeleteEvetntAsync(id);
            return status;
        }
        #endregion

        #region Orders
        //ORDERS
        public async Task<HttpStatusCode> CreateOrder(Order order)
        {
            OrdersController ordersController = new OrdersController();
            var status = await ordersController.CreateOrderAsync(order);
            return status;
        }

        public async Task<List<Order>> GetOrdersListOfDate(int userId, DateTime date)
        {
            OrdersController ordersController = new OrdersController();
            var orders = await ordersController.GetOrdersListOfDateAsync(userId, date);
            return orders;
        }

        public async Task<List<Order>> GetAllOrdersToday(DateTime date)
        {
            OrdersController ordersController = new OrdersController();
            var orders = await ordersController.GetAllOrdersTodayAsync(date);
            return orders;
        }

        public async Task<List<Order>> GetOrdersList(int count, int id)
        {
            OrdersController ordersController = new OrdersController();
            var orders = await ordersController.GetAllOrdersAsync(count, id);
            return orders;
        }

        public async Task<HttpStatusCode> DeleteOrder(int id)
        {
            OrdersController ordersController = new OrdersController();
            var status = await ordersController.DeleteOrderAsync(id);
            return status;
        }

        public async Task<HttpStatusCode> EditOrder(Order order)
        {
            OrdersController ordersController = new OrdersController();
            var status = await ordersController.UpdateOrderAsync(order);
            return status;
        }
        #endregion

        #region PublicChat
        //PUBLIC CHAT
        public async Task<HttpStatusCode> CreatePublicChatMessage(PublicChat publicChat)
        {
            PublicChatsController publicChatsController = new PublicChatsController();
            var status = await publicChatsController.CreatePublicChatAsync(publicChat);
            return status;
        }

        public async Task<List<PublicChat>> GetPublicChatMessagesList(int count, int id)
        {
            PublicChatsController publicChatsController = new PublicChatsController();
            var publicChats = await publicChatsController.GetAllPublicChatsAsync(count, id);
            return publicChats;
        }

        public async Task<HttpStatusCode> DeletePublicChatMessage(int id)
        {
            PublicChatsController publicChatsController = new PublicChatsController();
            var status = await publicChatsController.DeletePublicChatAsync(id);
            return status;
        }

        public async Task<HttpStatusCode> EditPublicChatMessage(PublicChat publicChat)
        {
            PublicChatsController publicChatsController = new PublicChatsController();
            var status = await publicChatsController.UpdatePublicChatAsync(publicChat);
            return status;
        }
        #endregion

        #region Users
        //Users
        public List<User> GetNewADUsersList()
        {
            ActiveDyrectoryController activeDyrectoryController = new ActiveDyrectoryController();
            var users = activeDyrectoryController.GetNewADUsers();
            return users;
        }

        public async Task<List<User>> GetAllUserName()
        {
            UsersController usersController = new UsersController();
            var users = await usersController.GetAllUserNameAsync();
            return users;
        }

        public async Task<HttpStatusCode> UpdateUsersDb(User user)
        {
            UsersController usersController = new UsersController();
            var status = await usersController.UpdateUsersDbAsync(user);
            return status;
        }
        #endregion

    }
}