using Microsoft.AspNetCore.Http;

namespace Models.Activity
{
    public class ActivityCreate
    {
        public int Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string Answer { get; set; }
    }
}
