using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient; 
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Station.Controllers
{
    [Authorize]
    
    public class UsersController : Controller
    {
        private readonly string _connectionString;

       
        public UsersController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var command = new SqlCommand("SELECT Id, Name, Role, PasswordHash, IsActive FROM Users", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Role = reader.GetString(2),
                        // IsActive to be added properly
                    });
                }
            }
            return View(users);
        }
        
       // ceate user
        public IActionResult Create() => View();

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(User user)
        {

            if (ModelState.IsValid)
            {
                    var hashed = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        var command = new SqlCommand("INSERT INTO Users (Name, Role, PasswordHash, IsActive) VALUES (@Name, @Role, @PasswordHash, @IsActive)", connection);
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@Role", user.Role);
                        command.Parameters.AddWithValue("@PasswordHash", hashed);
                        command.Parameters.AddWithValue("@IsActive", 1);
                    command.ExecuteNonQuery();
                    }
                    return RedirectToAction(nameof(Index));
                
            }
            return View(user);
           
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var user = GetUserById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var user = GetUserById(id);
            if (user == null) return NotFound();
            return View(user);
        }

       
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                var command = new SqlCommand("SELECT Id, Name, Role, IsActive FROM Users WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Role = reader.GetString(2),
                        IsActive = reader.GetBoolean(3)
                    };
                }
            }
            return user;
        }


        // deactivate user instead of deleting 

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Deactivate(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Users SET IsActive = 0 WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Activate(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Users SET IsActive = 1 WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }



        [Authorize]
        public IActionResult Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = GetUserById(userId); 
            if (user == null) return NotFound();

            return View("Profile", user);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Profile(User model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId != model.Id) return Unauthorized(); // extra safety

            // Hash password if it changed
            if (!string.IsNullOrEmpty(model.PasswordHash))
            {
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
            }

            // Reuse your Edit logic to update name & password
            Edit(model); // your existing DB update method
            return RedirectToAction("Profile");
        }



        [HttpPost]
        [Authorize]
        public IActionResult UpdateProfile(User model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != model.Id) return Unauthorized();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE Users SET Name=@Name";
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    sql += ", PasswordHash=@PasswordHash";
                    model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
                }
                sql += " WHERE Id=@Id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@Id", model.Id);
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    command.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
                }

                command.ExecuteNonQuery();
            }

            return RedirectToAction("Profile");
        }




    }
}
