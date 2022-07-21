using System;

namespace TextsClassifierAPI.Models.Responses
{
    public class ClassificationResponse
    {
        public Guid? CategoryId { get; set; }
        public string Category { get; set; }
        public string RawText { get; set; }
        public float Score { get; set; }
    }
}
