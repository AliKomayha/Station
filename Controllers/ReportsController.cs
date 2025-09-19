using Microsoft.AspNetCore.Mvc;
using Station.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace Station.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly string _connectionString;

        public ReportsController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var model = new Report();
            return View(model);
        }

        [HttpPost]
        public IActionResult Generate(Report model)
        {
            if (model.FromDate > model.ToDate)
            {
                ModelState.AddModelError("", "From date cannot be greater than To date.");
                return View("Index", model);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();


                    if (model.IncludeExpenses)
                        model.Expenses = GetExpenses(connection, model.FromDate, model.ToDate);

                    if (model.IncludeDebts)
                        model.Debts = GetDebts(connection, model.FromDate, model.ToDate);

                    if (model.IncludeCarWashes)
                        model.CarWashes = GetCarWashes(connection, model.FromDate, model.ToDate);

                    if (model.IncludeDebtCollections)
                        model.DebtCollections = GetDebtCollections(connection, model.FromDate, model.ToDate);

                    if (model.IncludeCustomerVouchers)
                        model.CustomerVouchers = GetCustomerVouchers(connection, model.FromDate, model.ToDate);

                    if (model.IncludeGovernmentVouchers)
                        model.GovernmentVouchers = GetGovernmentVouchers(connection, model.FromDate, model.ToDate);

                    if (model.IncludeGasOilFilters)
                        model.GasOilFilters = GetGasOilFilters(connection, model.FromDate, model.ToDate);

                    if (model.IncludeLitersPlus)
                        model.LitersPlus = GetLitersPlus(connection, model.FromDate, model.ToDate);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while generating the report: {ex.Message}");
                return View("Index", model);
            }

            return View("Index", model);
        }


        private List<Expense> GetExpenses(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var expenses = new List<Expense>();
            var command = new SqlCommand(
                "SELECT e.Id, e.Type, e.Price, e.Date, e.UsersId, u.Name FROM Expenses e " +
                "JOIN Users u ON u.Id = e.UsersId " +
                "WHERE e.Date >= @FromDate AND e.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                expenses.Add(new Expense
                {
                    Id = reader.GetInt32(0),
                    Type = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Date = reader.GetDateTime(3),
                    UserId = reader.GetInt32(4),
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return expenses;
        }

        private List<Debt> GetDebts(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var debts = new List<Debt>();
            var command = new SqlCommand(
                "SELECT d.Id, d.Name, d.Price, d.Date, d.UsersID, u.Name FROM Debts d " +
                "JOIN Users u ON u.Id = d.UsersID " +
                "WHERE d.Date >= @FromDate AND d.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
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
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return debts;
        }

        private List<CarWash> GetCarWashes(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var carWashes = new List<CarWash>();
            var command = new SqlCommand(
                "SELECT c.Id, c.Type, c.Price, c.Date, c.UsersId, u.Name FROM Car_Washes c " +
                "JOIN Users u ON u.Id = c.UsersId " +
                "WHERE c.Date >= @FromDate AND c.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                carWashes.Add(new CarWash
                {
                    Id = reader.GetInt32(0),
                    Type = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Date = reader.GetDateTime(3),
                    UserId = reader.GetInt32(4),
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return carWashes;
        }

        private List<DebtCollection> GetDebtCollections(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var debtCollections = new List<DebtCollection>();
            var command = new SqlCommand(
                "SELECT dc.Id, dc.Name, dc.Price, dc.Date, dc.UsersId, u.Name FROM Debt_Collection dc " +
                "JOIN Users u ON u.Id = dc.UsersId " +
                "WHERE dc.Date >= @FromDate AND dc.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                debtCollections.Add(new DebtCollection
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Date = reader.GetDateTime(3),
                    UserId = reader.GetInt32(4),
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return debtCollections;
        }

        private List<CustomerVoucher> GetCustomerVouchers(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var customerVouchers = new List<CustomerVoucher>();
            var command = new SqlCommand(
                "SELECT cv.Id, cv.Name, cv.Price, cv.Date, cv.UsersId, u.Name FROM Customer_Vouchers cv " +
                "JOIN Users u ON u.Id = cv.UsersId " +
                "WHERE cv.Date >= @FromDate AND cv.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                customerVouchers.Add(new CustomerVoucher
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Date = reader.GetDateTime(3),
                    UserId = reader.GetInt32(4),
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return customerVouchers;
        }

        private List<GovernmentVoucher> GetGovernmentVouchers(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var governmentVouchers = new List<GovernmentVoucher>();
            var command = new SqlCommand(
                "SELECT gv.Id, gv.Name, gv.Price, gv.Date, gv.UsersId, u.Name FROM Government_Vouchers gv " +
                "JOIN Users u ON u.Id = gv.UsersId " +
                "WHERE gv.Date >= @FromDate AND gv.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                governmentVouchers.Add(new GovernmentVoucher
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Date = reader.GetDateTime(3),
                    UserId = reader.GetInt32(4),
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return governmentVouchers;
        }

        private List<GasOilFilter> GetGasOilFilters(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var gasOilFilters = new List<GasOilFilter>();
            var command = new SqlCommand(
                "SELECT gof.Id, gof.Type, gof.Price, gof.Date, gof.UsersId, u.Name FROM Gas_Oil_Filters gof " +
                "JOIN Users u ON u.Id = gof.UsersId " +
                "WHERE gof.Date >= @FromDate AND gof.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                gasOilFilters.Add(new GasOilFilter
                {
                    Id = reader.GetInt32(0),
                    Type = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Date = reader.GetDateTime(3),
                    UserId = reader.GetInt32(4),
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return gasOilFilters;
        }

        private List<LitersPlus> GetLitersPlus(SqlConnection connection, DateTime fromDate, DateTime toDate)
        {
            var litersPlus = new List<LitersPlus>();
            var command = new SqlCommand(
                "SELECT lp.Id, lp.Type, lp.Price, lp.Date, lp.UsersId, u.Name FROM Liters_Plus lp " +
                "JOIN Users u ON u.Id = lp.UsersId " +
                "WHERE lp.Date >= @FromDate AND lp.Date <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                litersPlus.Add(new LitersPlus
                {
                    Id = reader.GetInt32(0),
                    Type = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Date = reader.GetDateTime(3),
                    UserId = reader.GetInt32(4),
                    User = new User { Id = reader.GetInt32(4), Name = reader.GetString(5) }
                });
            }
            reader.Close();
            return litersPlus;
        }

        public IActionResult Print()
        {
            // This will be handled by JavaScript for client-side printing
            return Json(new { success = true });
        }
    }
}

