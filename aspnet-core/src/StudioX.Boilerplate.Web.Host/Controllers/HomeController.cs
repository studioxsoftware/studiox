using System.Threading.Tasks;
using StudioX;
using StudioX.Notifications;
using StudioX.Timing;
using StudioX.Boilerplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using StudioX.Extensions;

namespace StudioX.Boilerplate.Web.Host.Controllers
{
    public class HomeController : BoilerplateControllerBase
    {
        private readonly INotificationPublisher notificationPublisher;

        public HomeController(INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;
        }

        public IActionResult Index()
        {
            return Redirect("/swagger");
        }

        /// <summary>
        /// This is a demo code to demonstrate sending notification to default tenant admin and host admin uers.
        /// Don't use this code in production !!!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ActionResult> TestNotification(string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            var defaultTenantAdmin = new UserIdentifier(1, 2);
            var hostAdmin = new UserIdentifier(null, 1);

            await notificationPublisher.PublishAsync(
                    "App.SimpleMessage",
                    new MessageNotificationData(message),
                    severity: NotificationSeverity.Info,
                    userIds: new[] { defaultTenantAdmin, hostAdmin }
                 );

            return Content("Sent notification: " + message);
        }
    }
}