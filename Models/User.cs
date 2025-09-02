using System.ComponentModel.DataAnnotations;


namespace Station.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]  
        public string Name { get; set;}
        [Required]
        public string Role { get; set; }

        public ICollection<Expense> Expenses { get; set; }
    }
}
