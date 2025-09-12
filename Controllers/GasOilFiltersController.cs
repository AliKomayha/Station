using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Station.Controllers
{
    [Authorize]
    public class GasOilFiltersController : Controller
    {
        private readonly string _connectionString;

        public GasOilFiltersController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: /GasOilFilter
        public IActionResult Index()
        {
            var gasOilFilters = new List<GasOilFilter>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Gas_Oil_Filters.Id, Type, Quantity, Price, Date, Users.Id, Users.Name FROM Gas_Oil_Filters JOIN Users ON Users.Id = Gas_Oil_Filters.UsersID", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    gasOilFilters.Add(new GasOilFilter
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Quantity = reader.GetDecimal(2),
                        Price = reader.GetDecimal(3),
                        Date = reader.GetDateTime(4),
                        UserId = reader.GetInt32(5),
                        User = new User
                        {
                            Id = reader.GetInt32(5),
                            Name = reader.GetString(6),
                        }
                    });
                }
            }
            return View(gasOilFilters);
        }

        // GET: /GasOilFilter/Create
        public IActionResult Create() => View();

        // POST: /GasOilFilter/Create
        [HttpPost]
        public IActionResult Create(GasOilFilter gasOilFilter)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO Gas_Oil_Filters (Type, Quantity, Price, Date, UsersID) VALUES (@Type, @Quantity, @Price, @Date, @UsersID)",
                        connection);
                    command.Parameters.AddWithValue("@Type", gasOilFilter.Type);
                    command.Parameters.AddWithValue("@Quantity", gasOilFilter.Quantity);
                    command.Parameters.AddWithValue("@Price", gasOilFilter.Price);
                    command.Parameters.AddWithValue("@Date", gasOilFilter.Date);
                    command.Parameters.AddWithValue("@UsersID",int.Parse(UserId));
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gasOilFilter);
        }

        // GET: /GasOilFilter/Details/5
        public IActionResult Details(int id)
        {
            var gasOilFilter = GetGasOilFilterById(id);
            if (gasOilFilter == null) return NotFound();
            return View(gasOilFilter);
        }

        // GET: /GasOilFilter/Edit/5
        public IActionResult Edit(int id)
        {
            var gasOilFilter = GetGasOilFilterById(id);
            if (gasOilFilter == null) return NotFound();
            return View(gasOilFilter);
        }

        // POST: /GasOilFilter/Edit/5
        [HttpPost]
        public IActionResult Edit(GasOilFilter gasOilFilter)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Gas_Oil_Filters SET Type=@Type, Quantity=@Quantity, Price=@Price, Date=@Date, UsersID=@UsersID WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Type", gasOilFilter.Type);
                    command.Parameters.AddWithValue("@Quantity", gasOilFilter.Quantity);
                    command.Parameters.AddWithValue("@Price", gasOilFilter.Price);
                    command.Parameters.AddWithValue("@Date", gasOilFilter.Date);
                    command.Parameters.AddWithValue("@UsersID", int.Parse(UserId));
                    command.Parameters.AddWithValue("@Id", gasOilFilter.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gasOilFilter);
        }

        // GET: /GasOilFilter/Delete/5
        public IActionResult Delete(int id)
        {
            var gasOilFilter = GetGasOilFilterById(id);
            if (gasOilFilter == null) return NotFound();
            return View(gasOilFilter);
        }

        // POST: /GasOilFilter/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Gas_Oil_Filters WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // helper method
        private GasOilFilter? GetGasOilFilterById(int id)
        {
            GasOilFilter? gasOilFilter = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Gas_Oil_Filters.Id, Type, Quantity, Price, Date, Users.Id, Users.Name FROM Gas_Oil_Filters JOIN Users ON Users.Id = Gas_Oil_Filters.UsersID WHERE Gas_Oil_Filters.Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    gasOilFilter = new GasOilFilter
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Quantity = reader.GetDecimal(2),
                        Price = reader.GetDecimal(3),
                        Date = reader.GetDateTime(4),
                        UserId = reader.GetInt32(5),
                        User = new User
                        {
                            Id = reader.GetInt32(5),
                            Name = reader.GetString(6),
                        }
                    };
                }
            }
            return gasOilFilter;
        }
    }
}
