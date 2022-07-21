using System;

namespace Dwh.Models
{
    public class UserCategory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid TariffId { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
