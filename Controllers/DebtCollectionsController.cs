using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Station.Controllers
{
    [Authorize]
    public class DebtCollectionsController : Controller
    {
        private readonly string _connectionString;

        public DebtCollectionsController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }



        public IActionResult Index()
        {
            var collections = new List<DebtCollection>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, Name, Price, Date, UsersID FROM Debt_Collection", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    collections.Add(new DebtCollection
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4)
                    });
                }
            }
            return View(collections);
        }

       
        public IActionResult Create() => View();


        [HttpPost]
        public IActionResult Create(DebtCollection collection)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO Debt_Collection (Name, Price, Date, UsersID) VALUES (@Name, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Name", collection.Name);
                    command.Parameters.AddWithValue("@Price", collection.Price);
                    command.Parameters.AddWithValue("@Date", collection.Date);
                    command.Parameters.AddWithValue("@UserId",int.Parse(UserId));
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }

        // GET: /DebtCollection/Details/5
        public IActionResult Details(int id)
        {
            var collection = GetCollectionById(id);
            if (collection == null) return NotFound();
            return View(collection);
        }


        public IActionResult Edit(int id)
        {
            var collection = GetCollectionById(id);
            if (collection == null) return NotFound();
            return View(collection);
        }

     
        [HttpPost]
        public IActionResult Edit(DebtCollection collection)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Debt_Collection SET Name=@Name, Price=@Price, Date=@Date, UsersID=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Name", collection.Name);
                    command.Parameters.AddWithValue("@Price", collection.Price);
                    command.Parameters.AddWithValue("@Date", collection.Date);
                    command.Parameters.AddWithValue("@UserId",int.Parse(UserId));
                    command.Parameters.AddWithValue("@Id", collection.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }

  
        public IActionResult Delete(int id)
        {
            var collection = GetCollectionById(id);
            if (collection == null) return NotFound();
            return View(collection);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Debt_Collection WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

    
        private DebtCollection? GetCollectionById(int id)
        {
            DebtCollection? collection = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, Name, Price, Date, UsersID FROM Debt_Collection WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    collection = new DebtCollection
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Date = reader.GetDateTime(3),
                        UserId = reader.GetInt32(4)
                    };
                }
            }
            return collection;
        }
    }
}
