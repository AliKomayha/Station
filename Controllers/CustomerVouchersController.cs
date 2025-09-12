using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Station.Controllers
{
    [Authorize]
    public class CustomerVouchersController : Controller
    {
        private readonly string _connectionString;

        public CustomerVouchersController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }



        public IActionResult Index()
        {
            var vouchers = new List<CustomerVoucher>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT Customer_Vouchers.Id, Customer_Vouchers.Name, Quantity, Price, Date, Users.Id, Users.Name FROM Customer_Vouchers JOIN Users ON Users.Id = Customer_Vouchers.UsersID", connection);
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
                        UserId = reader.GetInt32(5), 
                        User = new User
                        {
                            Id = reader.GetInt32(5),
                            Name = reader.GetString(6)
                        }
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
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO Customer_Vouchers (Name, Quantity, Price, Date, UsersId) VALUES (@Name, @Quantity, @Price, @Date, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@Name", voucher.Name);
                    command.Parameters.AddWithValue("@Quantity", voucher.Quantity);
                    command.Parameters.AddWithValue("@Price", voucher.Price);
                    command.Parameters.AddWithValue("@Date", voucher.Date);
                    command.Parameters.AddWithValue("@UserId",int.Parse(UserId));
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
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId == null) return RedirectToAction("Login", "Auth");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Customer_Vouchers SET Name=@Name, Quantity=@Quantity, Price=@Price, Date=@Date, UsersId=@UserId WHERE Id=@Id",
                        connection);
                    command.Parameters.AddWithValue("@Name", voucher.Name);
                    command.Parameters.AddWithValue("@Quantity", voucher.Quantity);
                    command.Parameters.AddWithValue("@Price", voucher.Price);
                    command.Parameters.AddWithValue("@Date", voucher.Date);
                    command.Parameters.AddWithValue("@UserId", int.Parse(UserId));
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
                var command = new SqlCommand("DELETE FROM Customer_Vouchers WHERE Id=@Id", connection);
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
                    "SELECT Customer_Vouchers.Id, Customer_Vouchers.Name, Quantity, Price, Date, Users.Id, Users.Name FROM Customer_Vouchers JOIN Users ON Users.Id = Customer_Vouchers.UsersID WHERE Customer_Vouchers.Id=@Id",
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
                        UserId = reader.GetInt32(5),
                        User = new User
                        {
                            Id = reader.GetInt32(5),
                            Name = reader.GetString(6)
                        }
                    };
                }
            }
            return voucher;
        }
    }
}
