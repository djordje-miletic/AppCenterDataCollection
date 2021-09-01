using AppCenterDataCollectionEntities.Container_classes.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCenterDataCollectionEntities.Entities
{
    public class BranchEntity
    {
        public Branch branch { get; set; }
        public bool configured { get; set; }
        public LastBuild lastBuild { get; set; }
        public string trigger { get; set; }
    }
}
