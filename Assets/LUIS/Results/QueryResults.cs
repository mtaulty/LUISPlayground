using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    [Serializable]
    public class QueryResultsEntity
    {
        public string entity;
        public string type;
        public int startIndex;
        public int endIndex;
        public QueryResultsResolution resolution;
    }
    [Serializable]
    public class QueryResults
    {
        public string query;
        public QueryResultsEntity[] entities;
        public QueryResultsIntent topScoringIntent;
    }
}
