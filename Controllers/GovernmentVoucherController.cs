using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Station.Controllers
{
    public class GovernmentVouchersController : Controller
    {
        private readonly string _connectionString;

        public GovernmentVouchersController(string connectionString)
        {
            _connectionString = connectionString;
        }

        // GET: /GovernmentVouchers
        public IActionResult Index()
        {
            var vouchers = new List<GovernmentVoucher>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, Type, Quantity, Price, Date, UserId FROM GovernmentVouchers", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    vouchers.Add(new GovernmentVoucher
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Quantity = reader.GetDecimal(2),
                        Price = reader.GetDecimal(3),
                        Date = reader.GetDateTime(4),
                        UserId = reader.GetInt32(5)
                    });
                }
            }
            return View(vouchers);
        }

        // GET: /GovernmentVouchers/Create
        public IActionResult Create() => View();

        // POST: /GovernmentVouchers/Create
        [HttpPost]
        public IActionResult Create(GovernmentVoucher voucher)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO GovernmentVouchers (Type, Quantity, Price, Date, UserId) VALUES (@Type, @Quantity, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Type", voucher.Type);
                    command.Parameters.AddWithValue("@Quantity", voucher.Quantity);
                    command.Parameters.AddWithValue("@Price", voucher.Price);
                    command.Parameters.AddWithValue("@Date", voucher.Date);
                    command.Parameters.AddWithValue("@UserId", voucher.UserId);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: /GovernmentVouchers/Details/5
        public IActionResult Details(int id)
        {
            var voucher = GetVoucherById(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        // GET: /GovernmentVouchers/Edit/5
        public IActionResult Edit(int id)
        {
            var voucher = GetVoucherById(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        // POST: /GovernmentVouchers/Edit/5
        [HttpPost]
        public IActionResult Edit(GovernmentVoucher voucher)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE GovernmentVouchers SET Type=@Type, Quantity=@Quantity, Price=@Price, Date=@Date, UserId=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Type", voucher.Type);
                    command.Parameters.AddWithValue("@Quantity", voucher.Quantity);
                    command.Parameters.AddWithValue("@Price", voucher.Price);
                    command.Parameters.AddWithValue("@Date", voucher.Date);
                    command.Parameters.AddWithValue("@UserId", voucher.UserId);
                    command.Parameters.AddWithValue("@Id", voucher.Id);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: /GovernmentVouchers/Delete/5
        public IActionResult Delete(int id)
        {
            var voucher = GetVoucherById(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        // POST: /GovernmentVouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM GovernmentVouchers WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // Private helper: fetch voucher by ID
        private GovernmentVoucher? GetVoucherById(int id)
        {
            GovernmentVoucher? voucher = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, Type, Quantity, Price, Date, UserId FROM GovernmentVouchers WHERE Id=@Id",
                    connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    voucher = new GovernmentVoucher
                    {
                        Id = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Quantity = reader.GetDecimal(2),
                        Price = reader.GetDecimal(3),
                        Date = reader.GetDateTime(4),
                        UserId = reader.GetInt32(5)
                    };
                }
            }
            return voucher;
        }
    }
}
