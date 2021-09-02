using AppCenterDataCollection.Work_classes;
using AppCenterDataCollectionEntities.Entities;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
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

                List<BranchBuildCompletedEntity> startedBuildsInformations = new List<BranchBuildCompletedEntity>();

                if (branches != null && branches.Count > 0)
                {
                    foreach (var branch in branches)
                    {
                        BranchBuildCompletedEntity buildInfo = await branchLogicClass.BuildBranch(branch);

                        if (buildInfo != null)
                        {
                            //Saving branches that build is started
                            Console.WriteLine("Branch: " + buildInfo.SourceBranch + " | build started");
                            startedBuildsInformations.Add(buildInfo);
                        }
                        else
                            Console.WriteLine("Branch: " + branch.branch.Name + "can't start build");
                    }

                    Console.WriteLine("All possible builds started");

                    var progressIndicator = new Progress<BranchBuildCompletedEntity>(branchLogicClass.ReportFinishedBuildData);

                    if (startedBuildsInformations != null && startedBuildsInformations.Count > 0)
                    {
                        CancellationTokenSource source = new CancellationTokenSource();
                        CancellationToken token = source.Token;
                        TaskFactory factory = new TaskFactory(token);

                        List<Task<BranchBuildCompletedEntity>> tasks = new List<Task<BranchBuildCompletedEntity>>();

                        foreach (var build in startedBuildsInformations)
                        {
                            tasks.Add(Task.Factory.StartNew(async () =>
                            {
                                BranchBuildCompletedEntity buildInfo = await branchLogicClass.CheckBuildStatus(build, progressIndicator);

                                return buildInfo;

                            }).Unwrap());
                        }

                        try
                        {
                            Task kolektor = factory.ContinueWhenAll(tasks.ToArray(), (results) =>
                            {
                                
                            }, token);

                            kolektor.Wait();
                        }
                        catch (AggregateException ae)
                        {
                            foreach (Exception e in ae.InnerExceptions)
                            {
                                if (e is TaskCanceledException)
                                    Console.WriteLine("Threre was an error while executing task: {0}",
                                                      ((TaskCanceledException)e).Message);
                                else
                                    Console.WriteLine("Exception: " + e.GetType().Name);
                            }

                            log.Error("Error in gahtering threads", ae);
                        }
                        catch (Exception ex)
                        {
                            log.Error("Error in thread part of code", ex);
                        }
                        finally
                        {
                            //Na kraju uništavamo token koji smo kreirali
                            source.Dispose();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Didn't fatch any branches");
                }

                Console.WriteLine("All done!");
            }
            catch (Exception ex)
            {
                log.Error("Error in main method", ex);
                throw;
            }
        }
    }
}
