using Domain;
using System;

namespace Application.Models.Activity
{
    public class ApprovedActivityReturn
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public ActivityTypeId Type { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DateApproved { get; set; }
        public int? NumberOfFavorites { get; set; }
        public int? NumberOfAwesomeReviews { get; set; }
        public int? NumberOfGoodReviews { get; set; }
        public int? NumberOfPoorReviews { get; set; }
        public int? NumberOfNoneReviews { get; set; }
    }
}
