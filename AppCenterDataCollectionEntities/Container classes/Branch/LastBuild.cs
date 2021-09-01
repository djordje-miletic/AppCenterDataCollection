using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCenterDataCollectionEntities.Container_classes.Branch
{
    public class LastBuild
    {
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public DateTime QueueTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public DateTime LastChangedDate { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string Reason { get; set; }
        public string SourceBranch { get; set; }
        public string SourceVersion { get; set; }
        public List<string> Tags { get; set; }
        public Properties Properties { get; set; }
    }
}
