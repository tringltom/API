using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Models.Activity
{
    public class ActivityGet
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public ActivityTypeId Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Answer { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? XpReward { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}
