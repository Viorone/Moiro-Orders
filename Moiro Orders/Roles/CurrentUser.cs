using Moiro_Orders.Controller;
using Moiro_Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moiro_Orders.Roles
{
    class CurrentUser
    {
        public CurrentUser() { }

        EventsController eventsController = new EventsController();
        OrdersController ordersController = new OrdersController();
        PublicChatsController publicChatsController = new PublicChatsController();

        //EVENTS
        public async Task CrateEvent(Event @event)
        {
            var status = await eventsController.CreateEventAsync(@event);
            MessageBox.Show(status.ToString());
        }

        // get All events by current user
        // count is number of events per page
        public async Task GetEvents()
        {
            var events = await eventsController.GetAllEventsAsync(20, PublicResources.Im.Id);
            MessageBox.Show(events[0].NameEvent, "Это название мероприятия");
        }

        public async Task EditEvent(Event @event)        //поставить таймер на возможность редактирования (возможно на час)
        {
            await eventsController.UpdateEvetntAsync(@event);
        }

        public async Task DeleteEvent(int id)            //поставить таймер на возможность удаления (возможно на час)
        {
            await eventsController.DeleteEvetntAsync(id);
        }

        //ORDERS
        public async Task CreateOrder(Order order)
        {
            var status = await ordersController.CreateOrderAsync(order);
            MessageBox.Show(status.ToString());
        }

        // get All orders by current user
        // count is number of orders per page
        public async Task GetOrders(int count)
        {
            var orders = await ordersController.GetAllOrdersAsync(count);
            MessageBox.Show(orders[0].Problem, "Это проблемма");
        }

        public async Task DeleteOrder(int id)        //поставить таймер на возможность редактирования (возможно на час)
        {
            await ordersController.DeleteOrderAsync(id);
        }

        public async Task EditOrder(Order order)     //поставить таймер на возможность удаления (возможно на час)
        {
            await ordersController.UpdateOrderAsync(order);
        }

        //PUBLIC CHATS
        public async Task CreatePublicChat(PublicChat publicChat)
        {
            var status = await publicChatsController.CreatePublicChatAsync(publicChat);
            MessageBox.Show(status.ToString());
        }

        // get All publicChats by current user
        // count is number of publicChats per page
        public async Task GetPublicChat(int count)
        {
            var publicChats = await publicChatsController.GetAllPublicChatsAsync(count);
            MessageBox.Show(publicChats[0].Message, "Это сообщение");
        }

        public async Task DeletePublicChat(int id)                         //поставить таймер на возможность редактирования (возможно на 10 минут)
        {
            await publicChatsController.DeletePublicChatAsync(id);
        }

        public async Task EditPublicChat(PublicChat publicChat)
        {
            await publicChatsController.UpdatePublicChatAsync(publicChat); //поставить таймер на возможность редактирования (возможно на 10 минут)
        }
    }
}
