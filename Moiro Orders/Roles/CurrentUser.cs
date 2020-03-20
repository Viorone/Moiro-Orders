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
        //потом доработть
        EventsController eventsController = new EventsController();
        OrdersController ordersController = new OrdersController();
        PublicChatsController publicChatsController = new PublicChatsController();

        //EVENTS
        public async Task<HttpStatusCode> CreateEvent(Event @event)
        {
            var status = await eventsController.CreateEventAsync(@event);
            return status;
        }

        public async Task<List<Event>> GetEventsList(int count, int id)
        {
            var events = await eventsController.GetAllEventsAsync(count, id);
            return events;
        }

        public async Task<HttpStatusCode> EditEvent(Event @event)
        {
            var status = await eventsController.UpdateEvetntAsync(@event);
            return status;
        }

        public async Task<HttpStatusCode> DeleteEvent(int id)
        {
            var status = await eventsController.DeleteEvetntAsync(id);
            return status;
        }

        //ORDERS
        public async Task<HttpStatusCode> CreateOrder(Order order)
        {
            var status = await ordersController.CreateOrderAsync(order);
            return status;
        }

        public async Task<List<Order>> GetOrdersListOfDate(int userId, DateTime date)
        {
            var orders = await ordersController.GetOrdersListOfDateAsync(userId, date);
            return orders;
        }

        public async Task<List<Order>> GetAllOrdersToday(DateTime date)
        {
            var orders = await ordersController.GetAllOrdersTodayAsync(date);
            return orders;
        }

        public async Task<List<Order>> GetOrdersList(int count, int id)
        {
            var orders = await ordersController.GetAllOrdersAsync(count, id);
            return orders;
        }

        public async Task<HttpStatusCode> DeleteOrder(int id)
        {
            var status = await ordersController.DeleteOrderAsync(id);
            return status;
        }

        public async Task<HttpStatusCode> EditOrder(Order order)
        {
            var status = await ordersController.UpdateOrderAsync(order);
            return status;
        }

        //PUBLIC CHATS
        public async Task<HttpStatusCode> CreatePublicChatMessage(PublicChat publicChat)
        {
            var status = await publicChatsController.CreatePublicChatAsync(publicChat);
            return status;
        }

        public async Task<List<PublicChat>> GetPublicChatMessagesList(int count, int id)
        {
            var publicChats = await publicChatsController.GetAllPublicChatsAsync(count, id);
            return publicChats;
        }

        public async Task<HttpStatusCode> DeletePublicChatMessage(int id)
        {
            var status = await publicChatsController.DeletePublicChatAsync(id);
            return status;
        }

        public async Task<HttpStatusCode> EditPublicChatMessage(PublicChat publicChat)
        {
            var status = await publicChatsController.UpdatePublicChatAsync(publicChat);
            return status;
        }

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
    }
}