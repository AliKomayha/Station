using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Station.Models;

namespace Station.Controllers
{
    public class AuthController : Controller
    {
        private readonly string _connectionString;

        public AuthController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string name, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("Select Id, Name, Role, PasswordHash, IsActive From Users WHERE Name=@Name", connection);
            command.Parameters.AddWithValue("@Name", name);
            var reader = command.ExecuteReader();


            if (!reader.Read())
            {
                ModelState.AddModelError("", "Invalid name or password");
                return View();
            }

            var userId = reader.GetInt32(0);
            var userName = reader.GetString(1);
            var role = reader.GetString(2);
            var passwordHash = reader.GetString(3);
            var isActive = reader.GetBoolean(4);

            if (!isActive)
            {
                ModelState.AddModelError("", "User is inactive");
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
            {
                ModelState.AddModelError("", "Invalid name or password");
                return View();
            }



            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home"); 
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
