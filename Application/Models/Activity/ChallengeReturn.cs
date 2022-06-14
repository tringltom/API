using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class ChallengeReturn : ActivityBase
    {
        public int ChallengeAnswerId { get; set; }
        public string ChallengeUserName { get; set; }
        public string ChallengeDesription { get; set; }
        public ICollection<Photo> ChallengePhotos { get; set; }
    }
}
