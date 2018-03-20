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

    public override void OnDictatedText(string text)
    {
        var query = new Query(
            "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/5cb7123d-1c67-40d3-ae54-dbcb4e1eb47c",
            "98d9738e0621478e9f25acd318345003");

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