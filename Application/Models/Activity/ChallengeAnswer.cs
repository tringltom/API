using Microsoft.AspNetCore.Http;

namespace Application.Models.Activity
{
    public class ChallengeAnswer
    {
        public string Description { get; set; }
        public IFormFile[] Images { get; set; }
    }
}
