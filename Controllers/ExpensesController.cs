using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Station.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly string _connectionString;  

        public ExpensesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // get all expenses
        public IActionResult Index()
        {
            var expenses = new List<Expense>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Type, Price, Date, UsersID FROM Expenses", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    expenses.Add(new Expense
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4)
                    });
                }
            }
            return View(expenses);
        }

        
        public IActionResult Create() => View();

        // POST create user
        [HttpPost]
        public IActionResult Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");


                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("INSERT INTO Expenses (Type, Price, Date, UsersID) VALUES (@Type, @Price, @Date, @UsersID)",
                        connection);

                    command.Parameters.AddWithValue("@Type", expense.Type);
                    command.Parameters.AddWithValue("@Price", expense.Price);
                    command.Parameters.AddWithValue("@Date", expense.Date);
                    command.Parameters.AddWithValue("@UsersID", int.Parse(UserId));
                    command.ExecuteNonQuery();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        
        public IActionResult Details(int id)
        {
            var expense = GetExpenseById(id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        
        public IActionResult Edit(int id)
        {
            var expense = GetExpenseById(id);
            if (expense == null) return NotFound();
            return View(expense);
        }


        [HttpPost]
        public IActionResult Edit(Expense expense)
        {
            if (ModelState.IsValid)
            {

                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Expenses SET Type=@Type, Price=@Price, Date=@Date, UsersID=@UsersID WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Type", expense.Type);
                    command.Parameters.AddWithValue("@Price", expense.Price);
                    command.Parameters.AddWithValue("@Date", expense.Date);
                    command.Parameters.AddWithValue("@UsersID", int.Parse(UserId));
                    command.Parameters.AddWithValue("@Id", expense.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        //get   delete
        public IActionResult Delete(int id)
        {
            var expense = GetExpenseById(id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        // post
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Expenses WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        //  fetch expense by ID
        private Expense? GetExpenseById(int id)
        {
            Expense? expense = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Type, Price, Date, UsersID FROM Expenses WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    expense = new Expense
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4)
                    };
                }
            }
            return expense;
        }
    }
}
