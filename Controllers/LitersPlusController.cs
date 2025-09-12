using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Station.Controllers
{
    [Authorize]
    public class LitersPlusController : Controller
    {
        private readonly string _connectionString;

        public LitersPlusController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }


        public IActionResult Index()
        {
            var litersPlusList = new List<LitersPlus>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Liters_Plus.Id, Type, Quantity, Price, Date, Users.Id, Users.Name FROM Liters_Plus JOIN Users ON Users.Id = Liters_Plus.UsersID", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    litersPlusList.Add(new LitersPlus
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Quantity = reader.GetDecimal(2),
                        Price = reader.GetDecimal(3),
                        Date = reader.GetDateTime(4),
                        UserId = reader.GetInt32(5),
                        User= new User
                        {
                            Id = reader.GetInt32(5),
                            Name = reader.GetString(6)
                        }
                    });
                }
            }
            return View(litersPlusList);
        }

    
        public IActionResult Create() => View();

        
        [HttpPost]
        public IActionResult Create(LitersPlus litersPlus)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO Liters_Plus (Type, Quantity, Price, Date, UsersID) VALUES (@Type, @Quantity, @Price, @Date, @UsersID)",
                        connection);
                    command.Parameters.AddWithValue("@Type", litersPlus.Type);
                    command.Parameters.AddWithValue("@Quantity", litersPlus.Quantity);
                    command.Parameters.AddWithValue("@Price", litersPlus.Price);
                    command.Parameters.AddWithValue("@Date", litersPlus.Date);
                    command.Parameters.AddWithValue("@UsersID", int.Parse(UserId));
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(litersPlus);
        }

  
        public IActionResult Details(int id)
        {
            var litersPlus = GetLitersPlusById(id);
            if (litersPlus == null) return NotFound();
            return View(litersPlus);
        }

       
        public IActionResult Edit(int id)
        {
            var litersPlus = GetLitersPlusById(id);
            if (litersPlus == null) return NotFound();
            return View(litersPlus);
        }

        
        [HttpPost]
        public IActionResult Edit(LitersPlus litersPlus)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Liters_Plus SET Type=@Type, Quantity=@Quantity, Price=@Price, Date=@Date, UsersID=@UsersID WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Type", litersPlus.Type);
                    command.Parameters.AddWithValue("@Quantity", litersPlus.Quantity);
                    command.Parameters.AddWithValue("@Price", litersPlus.Price);
                    command.Parameters.AddWithValue("@Date", litersPlus.Date);
                    command.Parameters.AddWithValue("@UsersID", int.Parse(UserId));
                    command.Parameters.AddWithValue("@Id", litersPlus.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(litersPlus);
        }

        // GET: /LitersPlus/Delete/5
        public IActionResult Delete(int id)
        {
            var litersPlus = GetLitersPlusById(id);
            if (litersPlus == null) return NotFound();
            return View(litersPlus);
        }

        // POST: /LitersPlus/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Liters_Plus WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // fetch LitersPlus by ID
        private LitersPlus? GetLitersPlusById(int id)
        {
            LitersPlus? litersPlus = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Liters_Plus.Id, Type, Quantity, Price, Date, Users.Id, Users.Name FROM Liters_Plus JOIN Users ON Users.Id = Liters_Plus.UsersID WHERE Liters_Plus.Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    litersPlus = new LitersPlus
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Quantity = reader.GetDecimal(2),
                        Price = reader.GetDecimal(3),
                        Date = reader.GetDateTime(4),
                        UserId = reader.GetInt32(5),
                        User = new User
                        {
                            Id= reader.GetInt32(5),
                            Name=reader.GetString(6)
                        }
                    };
                }
            }
            return litersPlus;
        }
    }
}
