using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class ChallengeEnvelope
    {
        public List<ChallengeReturn> Challenges { get; set; }
        public int ChallengesCount { get; set; }
    }
}
