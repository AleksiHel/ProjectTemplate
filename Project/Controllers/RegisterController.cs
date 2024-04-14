using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Project.Models;

namespace Project.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string testi = "661bccf4e2de7e7519ec782c";

            var id = ObjectId.Parse(testi);

            var registerModel = new User
            {
                Username = model.Username,
                Password = model.Password
            };

            try
            { DatabaseManipulator.Register(registerModel); }
            catch (Exception e) {
                Console.WriteLine(e);
                return View(model); }
            

            return RedirectToAction("Index", "Home");
        }
    }
}
