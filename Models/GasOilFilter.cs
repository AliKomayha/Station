namespace Station.Models
{
    public class GasOilFilter
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
      
        public DateTime Date { get; set; }

        //foreign key
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
