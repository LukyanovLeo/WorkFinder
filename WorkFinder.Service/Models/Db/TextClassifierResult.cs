using System;
using System.Collections.Generic;
using System.Text;

namespace VkService.Models.Db
{
    class TextClassifierResult
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public string RawText { get; set; }
        public string Category { get; set; }
        public float Score { get; set; }
        public string Trivia { get; set; }
    }
}
