using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using DAL.Query;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IChallengeService
    {
        Task<ChallengeEnvelope> GetChallengesForApprovalAsync(QueryObject queryObject);
        Task<Either<RestError, ChallengeAnswerEnvelope>> GetOwnerChallengeAnswersAsync(int id, QueryObject queryObject);
        Task<Either<RestError, Unit>> ConfirmChallengeAnswerAsync(int challengeAnswerId);
        Task<Either<RestError, Unit>> AnswerToChallengeAsync(int id, ChallengeAnswer challengeAnswer);
        Task<Either<RestError, Unit>> DisapproveChallengeAnswerAsync(int challengeAnswerId);
        Task<Either<RestError, Unit>> ApproveChallengeAnswerAsync(int challengeAnswerId);
    }
}
