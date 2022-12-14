using System.Threading.Tasks;

namespace Application.InfrastructureInterfaces
{
    public interface IEmailManager
    {
        Task SendConfirmationEmailAsync(string verifyUrl, string email);
        Task SendPasswordRecoveryEmailAsync(string verifyUrl, string email);
        Task SendActivityApprovalEmailAsync(string activityTitle, string userEmail, bool approved);
        Task SendProfileImageApprovalEmailAsync(string userName, bool approved);
        Task SendPuzzleAnsweredAsync(string puzzleTitle, string creatorEmail, string answererUsername);
        Task SendChallengeAnsweredEmailAsync(string challengeTitle, string creatorEmail, string answererUsername);
        Task SendChallengeAnswerAcceptedEmailAsync(string challengeTitle, string answererEmail);
        string DecodeVerificationToken(string token);
    }
}
