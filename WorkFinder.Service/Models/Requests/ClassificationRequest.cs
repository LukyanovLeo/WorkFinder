namespace VkService.Models.Request
{
    public class ClassificationRequest
    {
        public string Text { get; set; }

        public ClassificationRequest(string text)
        {
            Text = text;
        }
    }
}
