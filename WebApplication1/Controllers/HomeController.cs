using Microsoft.AspNet.SignalR.Client;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace WebAppWithSignalRAndWCF.Controllers
{
    public class HomeController : Controller
    {
        private readonly HubConnection _signalRConnection;
        private readonly IHubProxy _signalRHubProxy;

        public HomeController()
        {
            _signalRConnection = new HubConnection("https://localhost:7028/");
            _signalRHubProxy = _signalRConnection.CreateHubProxy("myHub");

            _signalRHubProxy.On<string>("ReceiveMessage", (message) =>
            {
                // Handle received messages here
                ViewBag.ReceivedMessage = message;
                Console.WriteLine("Message received from SignalR hub: " + message);
            });
        }

        public ActionResult Index()
        {
            try
            {
                _signalRConnection.Start();
                ViewBag.SignalRStatus = "Connected";
                Console.WriteLine("SignalR connection established.");
            }
            catch (Exception ex)
            {
                ViewBag.SignalRStatus = $"Connection failed: {ex.Message}";
                Console.WriteLine("SignalR connection failed: " + ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult SendMessage(string message)
        {
            try
            {
                _signalRHubProxy.Invoke("SendMessage", message).Wait();
                ViewBag.SentMessage = message;
                Console.WriteLine("Message sent to SignalR hub: " + message);
            }
            catch (Exception ex)
            {
                ViewBag.SentMessage = $"Failed to send message: {ex.Message}";
                Console.WriteLine("Failed to send message: " + ex.Message);
            }

            return RedirectToAction("Index");
        }
    }
}
