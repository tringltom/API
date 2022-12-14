using System;
using System.Collections.Generic;

namespace Domain
{
    public class Activity
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public ActivityTypeId ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Answer { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? XpReward { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset DateApproved { get; set; }
        public virtual ICollection<ActivityMedia> ActivityMedias { get; set; }
        public virtual ICollection<UserAttendance> UserAttendances { get; set; }
        public virtual ICollection<HappeningMedia> HappeningMedias { get; set; }
        public virtual ICollection<UserFavoriteActivity> UserFavorites { get; set; }
        public virtual ICollection<UserChallengeAnswer> UserChallengeAnswers { get; set; }
        public virtual ICollection<UserReview> UserReviews { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
