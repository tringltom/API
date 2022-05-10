using System.Collections.Generic;
using Domain;

namespace DAL.Query
{
    public class ActivityQuery : QueryObject
    {
        public string Title { get; set; }
        public List<ActivityTypeId> ActivityTypes { get; set; }
    }
}
