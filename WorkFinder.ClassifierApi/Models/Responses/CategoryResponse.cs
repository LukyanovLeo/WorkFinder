using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextsClassifierAPI.Models.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string InfraName { get; set; }
    }
}
