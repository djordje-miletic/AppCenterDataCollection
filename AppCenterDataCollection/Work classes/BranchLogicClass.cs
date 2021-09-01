using AppCenterDataCollectionEntities.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace AppCenterDataCollection.Work_classes
{
    public class BranchLogicClass
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

        private string baseAddress;
        private string branchesAPI = "branches";
        private string buildAPI = "branches/{0}/builds";
        private string token;

        #region Constructor

        public BranchLogicClass()
        {
            string user = ConfigurationManager.AppSettings["user"];
            string application = ConfigurationManager.AppSettings["application"];
            string baseApi = ConfigurationManager.AppSettings["baseApi"];
            token = ConfigurationManager.AppSettings["token"];

            baseAddress = baseApi + user + "/" + application + "/";
        }

        #endregion Constructor

        #region Public methods

        /// <summary>
        /// Api call for getting all branches for application
        /// </summary>
        /// <returns></returns>
        public async Task<List<BranchEntity>> GetBranches()
        {
            List<BranchEntity> result = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);

                    client.DefaultRequestHeaders.Add("X-API-Token", token);

                    HttpResponseMessage responseMessage = await client.GetAsync(branchesAPI);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string contentJson = await responseMessage.Content.ReadAsStringAsync();

                        result = JsonConvert.DeserializeObject<List<BranchEntity>>(contentJson);
                    }
                    else
                    {
                        return null;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error on BranchLogicClass.GetBranches: ", ex);
                return null;
            }
        }

        /// <summary>
        /// API call for staring build of a branch
        /// </summary>
        /// <param name="branchData">Branch information</param>
        /// <returns></returns>
        public async Task<BranchBuildCompletedEntity> BuildBranch(BranchEntity branchData)
        {
            BranchBuildCompletedEntity result = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);

                    client.DefaultRequestHeaders.Add("X-API-Token", token);
                    client.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string formatedApiCall = buildAPI.Replace("{0}", branchData.branch.Name);
                    string apiCallForBuild = baseAddress + formatedApiCall;

                    BranchBuildData branchInformation = new BranchBuildData()
                    {
                        SourceVersion = branchData.branch.Commit.Sha,
                        Debug = true
                    };

                    var byteContent = CreateJsonRequestData(branchInformation);

                    HttpResponseMessage responseMessage = await client.PostAsync(apiCallForBuild, byteContent);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string contentJson = await responseMessage.Content.ReadAsStringAsync();

                        result = JsonConvert.DeserializeObject<BranchBuildCompletedEntity>(contentJson);

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error on BranchLogicClass.BuildBranch: ", ex);
                throw;
            }
        }

        public async Task<BranchBuildCompletedEntity> CheckBuildStatus(BranchBuildCompletedEntity startedBuildInfo)
        {
            List<BranchBuildCompletedEntity> builds = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);

                    client.DefaultRequestHeaders.Add("X-API-Token", token);
                    client.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string formatedApiCall = buildAPI.Replace("{0}", startedBuildInfo.SourceBranch);
                    string apiCallForBuild = baseAddress + formatedApiCall;

                    bool breakLoop = false;

                    do
                    {
                        HttpResponseMessage responseMessage = await client.GetAsync(apiCallForBuild);

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string contentJson = await responseMessage.Content.ReadAsStringAsync();

                            builds = JsonConvert.DeserializeObject<List<BranchBuildCompletedEntity>>(contentJson);

                            if(builds != null)
                            {
                                var currentBuild = builds.FirstOrDefault(x => x.Id == startedBuildInfo.Id);

                                if (currentBuild != null)
                                {
                                    return currentBuild;
                                }
                            }
                        }
                        else
                        {
                            return null;
                        }
                    } 
                    while (breakLoop == false);
                }

                return null;
            }
            catch (Exception ex)
            {
                log.Error("Error on BranchLogicClass.CheckBuildStatus: ", ex);
                throw;
            }
        }

        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Creating byte array for sending data to server
        /// </summary>
        /// <param name="data">Any object that can be serialized</param>
        /// <returns></returns>
        private ByteArrayContent CreateJsonRequestData(object data) 
        {
            var myContent = JsonConvert.SerializeObject(data);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }

        #endregion Private methods
    }
}
