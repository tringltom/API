using Application.Models;
using Application.Models.Comment;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentReturn>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.User.Id))
                .ForMember(d => d.Image, o =>
                {
                    o.PreCondition(s => s.User.ImageApproved);
                    o.MapFrom(s => new Photo() { Url = s.User.ImageUrl });
                });
        }
    }
}
