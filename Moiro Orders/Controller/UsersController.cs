using Moiro_Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Moiro_Orders.Controller
{
    class UsersController
    {
        public UsersController(){}

        public async Task<HttpStatusCode> GetUserAsync(string login)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync(
                $"api/UsersAPI?login={login}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsAsync<User>();

                PublicResources.Im = user;               
            }

            return response.StatusCode;
        }

        public async Task<User> GetUserNameAsync(string login)
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync(
                $"api/UsersAPI?login={login}");
            var user = new User();
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadAsAsync<User>();
                PublicResources.Im = user;
            }
            return user;
        }
    }
}


