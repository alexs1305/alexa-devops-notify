using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurrentPrs
{
    class Notifier
    {
        public static async Task Notify(string message, string messageTitle = null, string token=null)
        {
            var httpClient = new HttpClient();
            await httpClient.PostAsJsonAsync("https://api.notifymyecho.com/v1/NotifyMe",
                new
                {
                    notification = message,
                    accessCode = token,
                    title = messageTitle
                });
        }
    }
}
