namespace Station.Models
{
    public class Debt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }

        //foreign key
        public int UserId { get; set; }
        public User User { get; set; }


    }
}
