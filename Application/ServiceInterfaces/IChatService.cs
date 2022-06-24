using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Comment;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IChatService
    {
        Task<Either<RestError, CommentReturn>> ApplyComment(CommentCreate comment);
    }
}
