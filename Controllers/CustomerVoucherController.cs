using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Station.Controllers
{
    public class CustomerVouchersController : Controller
    {
        private readonly string _connectionString;

        public CustomerVouchersController(string connectionString)
        {
            _connectionString = connectionString;
        }

      
        public IActionResult Index()
        {
            var vouchers = new List<CustomerVoucher>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, Name, Quantity, Price, Date, UserId FROM CustomerVouchers", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    vouchers.Add(new CustomerVoucher
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Quantity = reader.GetDecimal(2),
                        Price = reader.GetDecimal(3),
                        Date = reader.GetDateTime(4),
                        UserId = reader.GetInt32(5)
                    });
                }
            }
            return View(vouchers);
        }


        public IActionResult Create() => View();

 
        [HttpPost]
        public IActionResult Create(CustomerVoucher voucher)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO CustomerVouchers (Name, Quantity, Price, Date, UserId) VALUES (@Name, @Quantity, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Name", voucher.Name);
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

        public IActionResult Details(int id)
        {
            var voucher = GetVoucherById(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

     
        public IActionResult Edit(int id)
        {
            var voucher = GetVoucherById(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }


        [HttpPost]
        public IActionResult Edit(CustomerVoucher voucher)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE CustomerVouchers SET Name=@Name, Quantity=@Quantity, Price=@Price, Date=@Date, UserId=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Name", voucher.Name);
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

        public IActionResult Delete(int id)
        {
            var voucher = GetVoucherById(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

     
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM CustomerVouchers WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // get by id
        private CustomerVoucher? GetVoucherById(int id)
        {
            CustomerVoucher? voucher = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Id, Name, Quantity, Price, Date, UserId FROM CustomerVouchers WHERE Id=@Id",
                    connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    voucher = new CustomerVoucher
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
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
