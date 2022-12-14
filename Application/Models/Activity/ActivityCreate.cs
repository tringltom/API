using System;
using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.Models.Activity
{
    public class ActivityCreate
    {
        public ActivityTypeId Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile[] Images { get; set; }
        public string Answer { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Location { get; set; }
        public string[] Urls { get; set; }
    }
}
