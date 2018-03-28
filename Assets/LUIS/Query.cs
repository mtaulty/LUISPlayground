using LUIS.Results;
using RESTClient;
using System;
using System.Collections;

namespace LUIS
{
    public class Query
    {
        string serviceBaseUrl;
        string serviceKey;

        public Query(string serviceBaseUrl,
            string serviceKey)
        {
            this.serviceBaseUrl = serviceBaseUrl;
            this.serviceKey = serviceKey;
        }
        public IEnumerator Get(Action<IRestResponse<QueryResults>> callback)
        {
            var request = new RestRequest(this.serviceBaseUrl, Method.GET);

            request.AddQueryParam("subscription-key", this.serviceKey);
            request.AddQueryParam("q", this.Utterance);
            request.AddQueryParam("verbose", this.Verbose.ToString());
            request.UpdateRequestUrl();

            yield return request.Send();

            request.ParseJson<QueryResults>(callback);
        }        
        public bool Verbose
        {
            get;set;
        }
        public string Utterance
        {
            get;set;
        }
    }
}