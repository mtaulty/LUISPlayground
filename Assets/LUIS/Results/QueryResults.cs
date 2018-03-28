using System;
using System.Linq;

namespace LUIS.Results
{
    [Serializable]
    public class QueryResultsIntent
    {
        public string intent;
        public float score;
    }
    [Serializable]
    public class QueryResultsResolution
    {
        public string[] values;

        public string FirstOrDefaultValue()
        {
            string value = string.Empty;
            
            if (this.values != null)
            {
                value = this.values.FirstOrDefault();
            }
            return (value);
        }
    }
    [Serializable]
    public class QueryResultsEntity
    {
        public string entity;
        public string type;
        public int startIndex;
        public int endIndex;
        public QueryResultsResolution resolution;

        public string FirstOrDefaultResolvedValue()
        {
            var value = string.Empty;

            if (this.resolution != null)
            {
                value = this.resolution.FirstOrDefaultValue();
            }

            return (value);
        }
        public string FirstOrDefaultResolvedValueOrEntity()
        {
            var value = this.FirstOrDefaultResolvedValue();

            if (string.IsNullOrEmpty(value))
            {
                value = this.entity;
            }
            return (value);
        }
    }
    [Serializable]
    public class QueryResults
    {
        public string query;
        public QueryResultsEntity[] entities;
        public QueryResultsIntent topScoringIntent;
    }
}
