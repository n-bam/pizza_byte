using System;
using System.Net.Http;
using System.Web.Mvc;

namespace PizzaByteSite.Controllers
{
    public class BaseController : Controller
    {
        internal HttpClient client { get; set; }

        public BaseController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:55751/");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}