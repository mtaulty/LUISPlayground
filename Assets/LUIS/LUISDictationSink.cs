using LUIS;
using LUIS.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class QueryResultsEventType : UnityEvent<QueryResultsEntity[]>
{
}

[Serializable]
public class DictationSinkHandler
{
    public string intentName;
    public QueryResultsEventType intentHandler;
}

public class LUISDictationSink : DictationSink
{
    public float minimumConfidenceScore = 0.5f;
    public DictationSinkHandler[] intentHandlers;
    public string luisApiEndpoint;
    public string luisApiKey;

    public override void OnDictatedText(string text)
    {
        var query = new Query(this.luisApiEndpoint, this.luisApiKey);

        query.Utterance = text;

        StartCoroutine(query.Get(
            results =>
            {
                if (!results.IsError)
                {
                    var data = results.Data;

                    if ((data.topScoringIntent != null) &&
                        (data.topScoringIntent.score > this.minimumConfidenceScore))
                    {
                        var handler = this.intentHandlers.FirstOrDefault(
                            h => h.intentName == data.topScoringIntent.intent);

                        if (handler != null)
                        {
                            handler.intentHandler.Invoke(data.entities);
                        }
                    }
                }
            }
        ));
    }
}