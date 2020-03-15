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
                MessageBox.Show(PublicResources.Im.Id + " " + PublicResources.Im.FullName, "User");
            }

            return response.StatusCode;
        }
    }
}


