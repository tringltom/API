using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ReviewType
    {
        public ReviewTypeId Id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public ReviewTypeId Name { get; set; }
    }
}
