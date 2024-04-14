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


          
            var newSalt = Encryptor.createSalt();
            var hashedPassword = Encryptor.HashPassword(model.Password, newSalt);

            Console.WriteLine($"{Convert.ToHexString(newSalt)}");
            Console.WriteLine(hashedPassword);




            var registerModel = new User
            {
                Username = model.Username,
                Password = hashedPassword,
                Salt = newSalt

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
