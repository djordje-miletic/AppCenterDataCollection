using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCenterDataCollectionEntities.Entities
{
    public class BranchBuildCompletedEntity
    {
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public string QueueTime { get; set; }
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public string LastChangedDate { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string SourceBranch { get; set; }
        public string SourceVersion { get; set; }
    }
}
