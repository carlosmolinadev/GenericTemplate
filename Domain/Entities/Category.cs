using GloboTicket.TicketManagement.Domain.Common;

namespace GloboTicket.TicketManagement.Domain.Entities
{
    public class Category: AuditableEntity
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
