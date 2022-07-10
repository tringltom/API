using Domain;
using System;

namespace Application.Models.Activity
{
    public class FavoritedActivityReturn
    {
        public int Id { get; set; }
        public ActivityTypeId Type { get; set; }
        public string Title { get; set; }        
        public int? NumberOfFavorites { get; set; }
        public int? NumberOfAwesomeReviews { get; set; }
        public int? NumberOfGoodReviews { get; set; }
        public int? NumberOfPoorReviews { get; set; }
        public int? NumberOfNoneReviews { get; set; }
    }
}
