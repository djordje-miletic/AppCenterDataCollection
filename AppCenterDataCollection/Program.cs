using AppCenterDataCollection.Work_classes;
using AppCenterDataCollectionEntities.Entities;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AppCenterDataCollection
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            try
            {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

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
                            Console.WriteLine("Branch: " + buildInfo.SourceBranch + " | build: " + buildInfo.Result + " | ");
                        else
                            Console.WriteLine("Branch: " + buildInfo.SourceBranch + "can't start build");
                    }
                }
                else
                {
                    Console.WriteLine("Didn't fatch any branches");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in main method", ex);
                throw;
            }
        }
    }
}
