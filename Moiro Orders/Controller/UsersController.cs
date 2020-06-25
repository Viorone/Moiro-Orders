using Moiro_Orders.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moiro_Orders.Controller
{
    class UsersController
    {
        public UsersController() { }

        public async Task<User> GetUserAsync(string login)
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

        public async Task<List<User>> GetAllUserNameAsync()
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync(
                $"api/UsersAPI");
            List<User> users = null;
            if (response.IsSuccessStatusCode)
            {
                users = await response.Content.ReadAsAsync<List<User>>();
            }
            return users;
        }

        public async Task<List<User>> GetAdminsListAsync()
        {
            bool admin = true;
            HttpResponseMessage response = await PublicResources.client.GetAsync(
                $"api/UsersAPI?admin={admin}");
            List<User> users = null;
            if (response.IsSuccessStatusCode)
            {
                users = await response.Content.ReadAsAsync<List<User>>();
            }
            return users;
        }

        public async Task<HttpStatusCode> UpdateUsersDbAsync(User user)
        {

            HttpResponseMessage response = await PublicResources.client.PostAsJsonAsync(
                "api/UsersAPI", user);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> UpdateUserAsync(User user)
        {
           
            HttpResponseMessage response = await PublicResources.client.PutAsJsonAsync(
                $"api/UsersAPI/{user.Id}", user);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
    }
}


