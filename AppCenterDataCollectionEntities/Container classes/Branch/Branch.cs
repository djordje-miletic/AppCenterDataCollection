using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCenterDataCollectionEntities.Container_classes.Branch
{
    public class Branch
    {
        public string Name { get; set; }
        public Commit Commit { get; set; }
        public bool Protected { get; set; }
    }
}
