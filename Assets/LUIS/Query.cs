using LUIS.Results;
using RESTClient;
using System;
using System.Collections;
using UnityEngine.Windows.Speech;

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
        /// <summary>
        /// In my version of Unity, this doesn't work. Unity seems to struggle with
        /// doing an HTTP POST in that I don't think it sends any of the body data
        /// to the server which means that we end up with a null looking response.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator Post(Action<IRestResponse<QueryResults>> callback)
        {
            var request = new RestRequest(this.serviceBaseUrl, Method.POST);
            request.Request.chunkedTransfer = false;

            request.AddQueryParam("subscription-key", this.serviceKey);
            request.AddQueryParam("verbose", this.Verbose.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.UpdateRequestUrl();

            request.AddBody(this.Utterance, "application/json");

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