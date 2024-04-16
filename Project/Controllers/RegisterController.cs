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
               return View( model);
            }


            // Pois ehkä, ei turvallinen, voi selvittää databasessa olevia käyttäjiä
            //if (DatabaseManipulator.CheckIfUsernameExist(model.Username))
            //{
            //    ModelState.AddModelError("LoginError", "Username already exists");
            //    return View(model);
            //}


            var newSalt = PasswordHash.createSalt();
            var hashedPassword = PasswordHash.HashPassword(model.Password, newSalt);


            var registerModel = new User
            {
                Username = model.Username.ToLower(),
                Password = hashedPassword,
                Salt = newSalt,
                IsAdmin = false

            };

            try
            { DatabaseManipulator.Register(registerModel); }
            catch (Exception e) {
               Console.WriteLine(e);
               return View(model); 
            }
            

            return RedirectToAction("Index", "Home");
        }
    }
}
