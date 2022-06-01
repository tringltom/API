using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class ChallengeReturn : ActivityBase
    {
        public string ChallengeDesription { get; set; }
        public ICollection<Photo> ChallengePhotos { get; set; }
    }
}
