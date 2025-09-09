using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace Station.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        
        public string Name { get; set;}
        [Required]
    
        public string Role { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;

        [ValidateNever]
        public ICollection<Expense> Expenses { get; set; }
    }
}
