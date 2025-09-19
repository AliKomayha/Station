using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Station.Models
{
    public class Report
    {
        [Required(ErrorMessage = "From date is required")]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "To date is required")]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; } = DateTime.Today;

     
        [Display(Name = "Expenses")]
        public bool IncludeExpenses { get; set; }

        [Display(Name = "Debts")]
        public bool IncludeDebts { get; set; }

        [Display(Name = "Car Washes")]
        public bool IncludeCarWashes { get; set; }

        [Display(Name = "Debt Collections")]
        public bool IncludeDebtCollections { get; set; }

        [Display(Name = "Customer Vouchers")]
        public bool IncludeCustomerVouchers { get; set; }

        [Display(Name = "Government Vouchers")]
        public bool IncludeGovernmentVouchers { get; set; }

        [Display(Name = "Gas/Oil/Filters")]
        public bool IncludeGasOilFilters { get; set; }

        [Display(Name = "Liters Plus")]
        public bool IncludeLitersPlus { get; set; }

        // Data results
  
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        public List<Debt> Debts { get; set; } = new List<Debt>();
        public List<CarWash> CarWashes { get; set; } = new List<CarWash>();
        public List<DebtCollection> DebtCollections { get; set; } = new List<DebtCollection>();
        public List<CustomerVoucher> CustomerVouchers { get; set; } = new List<CustomerVoucher>();
        public List<GovernmentVoucher> GovernmentVouchers { get; set; } = new List<GovernmentVoucher>();
        public List<GasOilFilter> GasOilFilters { get; set; } = new List<GasOilFilter>();
        public List<LitersPlus> LitersPlus { get; set; } = new List<LitersPlus>();

        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public bool HasData =>
            Expenses.Any() || Debts.Any() || CarWashes.Any() ||
            DebtCollections.Any() || CustomerVouchers.Any() ||
            GovernmentVouchers.Any() || GasOilFilters.Any() || LitersPlus.Any();
    }


}
