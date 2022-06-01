using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class ChallengeAnswerEnvelope
    {
        public List<ChallengeAnswerReturn> ChallengeAnswers { get; set; }
        public int ChallengeAnswersCount { get; set; }
    }
}
