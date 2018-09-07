using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using ReachMobi.Models;

namespace ReachMobi.Controllers
{
    public class HomeController : Controller
    {
        static Dictionary<string, int> Clicktracker = new Dictionary<string, int>();

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            string searchQuery = Request["search"];
            if (null == searchQuery)
            {
                searchQuery = (string)TempData["searchQuery"];
            }
            var request = WebRequest.Create("https://itunes.apple.com/search?term=" + searchQuery + "&limit=200");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream datastream = response.GetResponseStream();
            StreamReader reader = new StreamReader(datastream);
            string responseString = reader.ReadToEnd();
            var list = JsonConvert.DeserializeObject<RootObject>(responseString);
            RootObject model = new RootObject
            {
                RegisterClicks = Clicktracker,
                results = list.results
            };

            TempData["searchQuery"] = searchQuery;
            return View("Index", model);
            
        }

        public ActionResult test()
        {
            string viewUrlKey = Request["hidcollectionID"].ToString();
            Dictionary<string, int> Clicktracker = registerClick(viewUrlKey);
            return Redirect(viewUrlKey);

        }

        private static Dictionary<string, int> registerClick(string viewUrlKey)
        {

            if (!Clicktracker.ContainsKey(viewUrlKey))
                Clicktracker.Add(viewUrlKey, 1);
            else
            {

                Clicktracker[viewUrlKey]++;
            }
            return Clicktracker;
        }
    }
}


