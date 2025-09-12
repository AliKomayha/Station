using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Station.Controllers
{
    [Authorize]
    public class DebtsController : Controller
    {
        private readonly string _connectionString;

        public DebtsController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }


        // GET: /Debts
        public IActionResult Index()
        {
            var debts = new List<Debt>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Debts.Id, Debts.Name, Price, Date, Users.Id, Users.Name FROM Debts JOIN Users ON Users.Id = Debts.UsersID", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    debts.Add(new Debt
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4),
                        User = new User {
                            Id = reader.GetInt32(4),
                            Name = reader.GetString(5),
                        }
                    });
                }
            }
            return View(debts);
        }

        // GET: /Debts/Create
        public IActionResult Create() => View();

        // POST: /Debts/Create
        [HttpPost]
        public IActionResult Create(Debt debt)
        {
            if (ModelState.IsValid)
            {

                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO Debts (Name, Price, Date, UsersID) VALUES (@Name, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Name", debt.Name);
                    command.Parameters.AddWithValue("@Price", debt.Price);
                    command.Parameters.AddWithValue("@Date", debt.Date);
                    command.Parameters.AddWithValue("@UserId", int.Parse(UserId));
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(debt);
        }

        // GET: /Debts/Details/5
        public IActionResult Details(int id)
        {
            var debt = GetDebtById(id);
            if (debt == null) return NotFound();
            return View(debt);
        }

        // GET: /Debts/Edit/5
        public IActionResult Edit(int id)
        {
            var debt = GetDebtById(id);
            if (debt == null) return NotFound();
            return View(debt);
        }

        // POST: /Debts/Edit/5
        [HttpPost]
        public IActionResult Edit(Debt debt)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Debts SET Name=@Name, Price=@Price, Date=@Date, UsersID=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Name", debt.Name);
                    command.Parameters.AddWithValue("@Price", debt.Price);
                    command.Parameters.AddWithValue("@Date", debt.Date);
                    command.Parameters.AddWithValue("@UserId", int.Parse(UserId));
                    command.Parameters.AddWithValue("@Id", debt.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(debt);
        }

        // GET: /Debts/Delete/5
        public IActionResult Delete(int id)
        {
            var debt = GetDebtById(id);
            if (debt == null) return NotFound();
            return View(debt);
        }

        // POST: /Debts/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Debts WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // Private helper: fetch debt by ID
        private Debt? GetDebtById(int id)
        {
            Debt? debt = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Debts.Id, Debts.Name, Price, Date, Users.Id, Users.Name FROM Debts JOIN Users ON Users.Id = Debts.UsersID WHERE Debts.Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    debt = new Debt
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4),
                        User = new User
                        {
                            Id = reader.GetInt32(4),
                            Name = reader.GetString(5)
                        }
                    };
                }
            }
            return debt;
        }
    }
}
