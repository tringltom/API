using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Comment
{
    public class CommentCreate
    {
        public int ActivityId { get; set; }
        public string Body { get; set; }
    }
}
