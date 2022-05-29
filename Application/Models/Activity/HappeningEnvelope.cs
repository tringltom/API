using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class HappeningEnvelope
    {
        public List<HappeningReturn> Happenings { get; set; }
        public int HappeningCount { get; set; }
    }
}
