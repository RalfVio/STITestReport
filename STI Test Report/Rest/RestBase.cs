using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using ADORest.BusinessObjects;

namespace ADORest
{
    public abstract class RestBase
    {
        protected string _collectionUrl = null;
        protected string _teamProject = null;
        protected string _personalAccessToken = null;

        const string _continuationTokenKeyName = "x-ms-continuationtoken";

        public RestBase(TeamProject teamProject)
        {
            _collectionUrl = teamProject.OrganizationUrl; _teamProject = teamProject.TeamProjectName; _personalAccessToken = teamProject.PAT;
        }

        protected string Credentials() { return Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _personalAccessToken))); }

        protected HttpClient GetClient()
        {
            HttpClient result = new HttpClient();
            result.Timeout = new TimeSpan(0, 1, 0);

            result.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            result.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Credentials());
            return result;
        }
        protected async Task<List<int>> GetFlatQueryResult(string wiql, HttpClient httpClient)
        {
            //https://www.visualstudio.com/en-us/docs/integrate/api/wit/wiql
            //https://stackoverflow.com/questions/11145053/cant-find-how-to-use-httpcontent

            string restUri = $"{_collectionUrl}/_apis/wit/wiql?api-version=6.0";

            if (wiql.Contains("@project"))
                wiql = wiql.Replace("@project", $"'{_teamProject}'");
            // Note: JsonConvert.ToString() adds double-quotes at beginning and end
            string restContent = $"{{\r\n\"query\":{Newtonsoft.Json.JsonConvert.ToString(wiql)}\r\n}}";

            List<int> result = null;
            try
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(
                            restUri, new StringContent(restContent, System.Text.Encoding.UTF8, "application/json")))  //(JsonConvert.SerializeObject(model)
                {
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();
                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic dataItemsJObject = resultJObject.workItems;
                    int count = dataItemsJObject.Count;
                    result = new List<int>();
                    for (int i = 0; i < count; i++)
                    {
                        int wiId = dataItemsJObject[i].id;
                        result.Add(wiId);
                    }
                }
            }

            catch
            {
                //MessageBox.Show(ex.ToString());
                throw;
            }

            return result;
        }
 

        #region Helper functions
        protected static string SqlIds(List<int> ids)
        {
            string result = null;
            foreach (int id in ids)
                result = (result == null ? "" : result + ",") + id.ToString();
            return result;
        }

        public static List<List<int>> SplitIdsIntoBatches(List<int> ids)
        {
            List<List<int>> result = new List<List<int>>();

            List<int> idsBatch = new List<int>();
            foreach (int id in ids)
            {
                idsBatch.Add(id);
                if (idsBatch.Count >= 25)
                {
                    result.Add(idsBatch);
                    idsBatch = new List<int>();
                }
            }
            if (idsBatch.Count > 0)
                result.Add(idsBatch);

            return result;
        }

        #endregion

        #region Service calls
        protected int _serviceCalls = 0;
        public int GetServiceCalls() => _serviceCalls;
        #endregion 

        #region Continuation Token
        protected string _continuationToken = null;
        public string ContinuationToken { get { return _continuationToken; } }

        protected void SetContinuationToken(System.Net.Http.Headers.HttpResponseHeaders headers)
        {
            _continuationToken = null;
            if (!headers.TryGetValues(_continuationTokenKeyName, out var continuationValues))
                return;

            _continuationToken = continuationValues.FirstOrDefault();
        }
        #endregion 

    }
}
