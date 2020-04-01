using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moiro_Orders.Models;

namespace Moiro_Orders.Controller
{
    class StatusController
    {
        public StatusController() { }

        public async Task<List<Status>> GetStatusesListAsync()
        {
            HttpResponseMessage response = await PublicResources.client.GetAsync($"api/StatusAPI");
            List<Status> statuses = null;
            if (response.IsSuccessStatusCode)
            {
                statuses = await response.Content.ReadAsAsync<List<Status>>();
            }
            return statuses;
        }
    }
}
