using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ReviewType
    {
        public ReviewType()
        {

        }

        public ReviewType(ReviewTypeId reviewType)
        {
            Id = reviewType;
            Name = reviewType;
        }

        public ReviewTypeId Id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public ReviewTypeId Name { get; set; }
    }
}
