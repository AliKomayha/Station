using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Station.Models
{
    public class GovernmentVoucher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public DateTime Date { get; set; }

        //foreign key
        public int UserId { get; set; }
        [ValidateNever]
        public User User { get; set; }
    }
}
