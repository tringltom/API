using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class ChallengeAnswerReturn
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public ICollection<Photo> ChallengePhotos { get; set; }
    }
}
