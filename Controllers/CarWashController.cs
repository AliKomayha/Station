using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Station.Controllers
{
    public class CarWashController : Controller
    {
        private readonly string _connectionString;

        public CarWashController(string connectionString)
        {
            _connectionString = connectionString;
        }

        
        public IActionResult Index()
        {
            var washes = new List<CarWash>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Type, Price, Date, UserId FROM CarWash", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    washes.Add(new CarWash
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4)
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("INSERT INTO CarWash (Type, Price, Date, UserId) VALUES (@Type, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Type", carWash.Type);
                    command.Parameters.AddWithValue("@Price", carWash.Price);
                    command.Parameters.AddWithValue("@Date", carWash.Date);
                    command.Parameters.AddWithValue("@UserId", carWash.UserId);
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
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE CarWash SET Type=@Type, Price=@Price, Date=@Date, UserId=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Type", carWash.Type);
                    command.Parameters.AddWithValue("@Price", carWash.Price);
                    command.Parameters.AddWithValue("@Date", carWash.Date);
                    command.Parameters.AddWithValue("@UserId", carWash.UserId);
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
                var command = new SqlCommand("DELETE FROM CarWash WHERE Id=@Id", connection);
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
                var command = new SqlCommand("SELECT Id, Type, Price, Date, UserId FROM CarWash WHERE Id=@Id", connection);
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
                        UserId = reader.GetInt32(4)
                    };
                }
            }
            return carWash;
        }
    }
}
