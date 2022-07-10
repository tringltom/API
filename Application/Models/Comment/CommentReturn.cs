using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Comment
{
    public class CommentReturn
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public Photo Image { get; set; }
    }
}
