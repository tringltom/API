using System;
using Domain;

namespace Application.Models.Activity
{
    public class ApprovedActivityReturn /*: ActivityBase*/
    {
        public int Id { get; set; }
        public ActivityTypeId Type { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DateApproved { get; set; }
        //public int NumberOfAttendees { get; set; }
        //public bool IsUserAttending { get; set; }
        //public bool IsHeld { get; set; }
        //public bool IsHost { get; set; }
        public int? NumberOfFavorites { get; set; }
        public int? NumberOfAwesomeReviews { get; set; }
        public int? NumberOfGoodReviews { get; set; }
        public int? NumberOfPoorReviews { get; set; }
        public int? NumberOfNoneReviews { get; set; }
    }
}
