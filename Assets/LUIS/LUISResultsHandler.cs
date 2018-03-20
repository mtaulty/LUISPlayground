using LUIS.Results;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LUISResultsHandler : MonoBehaviour
{
    public float minimumConfidence;

    public void HandleResults(
        QueryResultsIntent topIntent,
        QueryResultsEntity[] entities)
    {
    }
}
