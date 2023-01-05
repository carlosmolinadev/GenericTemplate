using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Customer
    {
        
        public int Id { get; set; }
        [ForeignKey("HiThere")]
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        [NotMapped]
        public Address Address { get; set; }
    }
}
