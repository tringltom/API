using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class HappeningReturn : ActivityBase
    {
        public ICollection<Photo> HappeningPhotos { get; set; }
    }
}
