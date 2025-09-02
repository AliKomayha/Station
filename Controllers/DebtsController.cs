using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Station.Controllers
{
    public class DebtsController : Controller
    {
        private readonly string _connectionString;

        public DebtsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        // GET: /Debts
        public IActionResult Index()
        {
            var debts = new List<Debt>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, Name, Price, Date, UserId FROM Debts", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    debts.Add(new Debt
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4)
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO Debts (Name, Price, Date, UserId) VALUES (@Name, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Name", debt.Name);
                    command.Parameters.AddWithValue("@Price", debt.Price);
                    command.Parameters.AddWithValue("@Date", debt.Date);
                    command.Parameters.AddWithValue("@UserId", debt.UserId);
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Debts SET Name=@Name, Price=@Price, Date=@Date, UserId=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Name", debt.Name);
                    command.Parameters.AddWithValue("@Price", debt.Price);
                    command.Parameters.AddWithValue("@Date", debt.Date);
                    command.Parameters.AddWithValue("@UserId", debt.UserId);
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
                    "SELECT Id, Name, Price, Date, UserId FROM Debts WHERE Id=@Id", connection);
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
                        UserId = reader.GetInt32(4)
                    };
                }
            }
            return debt;
        }
    }
}
