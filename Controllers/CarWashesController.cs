using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Station.Controllers
{
    [Authorize]
    public class CarWashesController : Controller
    {
        private readonly string _connectionString;

        public CarWashesController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }


        public IActionResult Index()
        {
            var washes = new List<CarWash>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Car_Washes.Id, Type,  Price, Date, Users.Id, Users.Name FROM Car_Washes JOIN Users ON Users.Id = Car_Washes.UsersID", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    washes.Add(new CarWash
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4),
                        User = new User
                        {
                            Id = reader.GetInt32(4),
                            Name = reader.GetString(5)

                        }
                    });
                }
            }
            return View(washes);
        }

   
        public IActionResult Create() => View();

     
        [HttpPost]
        public IActionResult Create(CarWash carWash)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("INSERT INTO Car_Washes (Type, Price, Date, UsersID) VALUES (@Type, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Type", carWash.Type);
                    command.Parameters.AddWithValue("@Price", carWash.Price);
                    command.Parameters.AddWithValue("@Date", carWash.Date);
                    command.Parameters.AddWithValue("@UserId", int.Parse(UserId));
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carWash);
        }

        public IActionResult Details(int id)
        {
            var carWash = GetCarWashById(id);
            if (carWash == null) return NotFound();
            return View(carWash);
        }

        public IActionResult Edit(int id)
        {
            var carWash = GetCarWashById(id);
            if (carWash == null) return NotFound();
            return View(carWash);
        }

        
        [HttpPost]
        public IActionResult Edit(CarWash carWash)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Car_Washes SET Type=@Type, Price=@Price, Date=@Date, UsersID=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Type", carWash.Type);
                    command.Parameters.AddWithValue("@Price", carWash.Price);
                    command.Parameters.AddWithValue("@Date", carWash.Date);
                    command.Parameters.AddWithValue("@UserId", int.Parse(UserId));
                    command.Parameters.AddWithValue("@Id", carWash.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carWash);
        }

    
        public IActionResult Delete(int id)
        {
            var carWash = GetCarWashById(id);
            if (carWash == null) return NotFound();
            return View(carWash);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Car_Washes WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // helper method to fetch by ID
        private CarWash? GetCarWashById(int id)
        {
            CarWash? carWash = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Car_Washes.Id, Type,  Price, Date, Users.Id, Users.Name FROM Car_Washes JOIN Users ON Users.Id = Car_Washes.UsersID WHERE Car_Washes.Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    carWash = new CarWash
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4),
                        User = new User
                        {
                            Id = reader.GetInt32(4),
                            Name = reader.GetString(5),
                        }
                    };
                }
            }
            return carWash;
        }
    }
}
