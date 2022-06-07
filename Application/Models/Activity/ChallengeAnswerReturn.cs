using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class ChallengeAnswerReturn
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Confirmed { get; set; }
        public ICollection<Photo> ChallengePhotos { get; set; }
    }
}
