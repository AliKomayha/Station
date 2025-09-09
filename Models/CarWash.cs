using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Station.Models
{
    public class CarWash
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }

        //foreign key
        public int UserId { get; set; }
        [ValidateNever]
        public User User { get; set; }

    }
}
