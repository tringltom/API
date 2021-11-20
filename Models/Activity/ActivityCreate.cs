using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Models.Activity
{
    public class ActivityCreate
    {
        public ActivityTypeId Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile[] Images { get; set; }
        public string Answer { get; set; }
    }
}
