

using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace singalR2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {         
            return View();
        }

        [HttpPost]
        public ActionResult Index(string device, string message)
        {
            //AndoridNotification pushNotificationObj = new AndoridNotification(); 
            try
            {
                pushMessage pushMessageObj = new pushMessage();
                pushMessageObj.SendNotification(device, message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Chat()
        {
            return View();
        }      
    }


    public class Test
    {
        public int Value
        {
            get;
            set;
        }

        public string String1
        {
            get;
            set;
        }

        public string String2
        {
            get;
            set;
        }
    }
}