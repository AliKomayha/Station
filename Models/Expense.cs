using System.ComponentModel.DataAnnotations;

namespace Station.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime Date { get; set; }
        //foreign key
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
