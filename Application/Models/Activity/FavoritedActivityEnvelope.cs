using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class FavoritedActivityEnvelope
    {
        public List<FavoritedActivityReturn> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
