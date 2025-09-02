using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient; 
using System.Collections.Generic;

namespace Station.Controllers
{
    public class UserController : Controller
    {
        private readonly string _connectionString;

       
        public UserController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET
        public IActionResult Index()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Console.WriteLine("POST hit: Name=");
                var command = new SqlCommand("SELECT Id, Name, Role FROM Users", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Role = reader.GetString(2)
                    });
                }
            }
            return View(users);
        }
        
       // ceate user
        public IActionResult Create() => View();

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            Console.WriteLine("request Name=" + user.Name + " Role=" + user.Role);

            if (ModelState.IsValid)
            {
               
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        var command = new SqlCommand("INSERT INTO Users (Name, Role) VALUES (@Name, @Role)", connection);
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@Role", user.Role);
                        command.ExecuteNonQuery();
                    }
                    return RedirectToAction(nameof(Index));
                
            }
            return View(user);
            //return Content($"request {user.Name}, {user.Role}");
        }

        
        public IActionResult Details(int id)
        {
            var user = GetUserById(id);
            if (user == null) return NotFound();
            return View(user);
        }


        public IActionResult Edit(int id)
        {
            var user = GetUserById(id);
            if (user == null) return NotFound();
            return View(user);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("UPDATE Users SET Name=@Name, Role=@Role WHERE Id=@Id", connection);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

   
        public IActionResult Delete(int id)
        {
            var user = GetUserById(id);
            if (user == null) return NotFound();
            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Users WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // get user by id
        private User? GetUserById(int id)
        {
            User? user = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Name, Role FROM Users WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Role = reader.GetString(2)
                    };
                }
            }
            return user;
        }
    }
}
