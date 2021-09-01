using AppCenterDataCollection.Work_classes;
using AppCenterDataCollectionEntities.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCenterDataCollection
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Collecting all branches...");

                BranchLogicClass branchLogicClass = new BranchLogicClass();

                List<BranchEntity> branches = await branchLogicClass.GetBranches();

                Console.WriteLine("All branches collected...");
                Console.WriteLine("Starting build of all branches");

                List<BranchBuildCompletedEntity> compleatedBuildsInformations = new List<BranchBuildCompletedEntity>();

                if (branches != null && branches.Count > 0)
                {
                    foreach (var branch in branches)
                    {
                        BranchBuildCompletedEntity buildInfo = await branchLogicClass.BuildBranch(branch);
                        
                        if(buildInfo != null)
                            Console.WriteLine("Branch: " + buildInfo.SourceBranch + " | build" + buildInfo.Result + " | ");
                    }
                }
                else
                {
                    Console.WriteLine("Didn't fatch any branches");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
