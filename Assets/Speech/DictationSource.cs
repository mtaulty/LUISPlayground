using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class DictationSource : MonoBehaviour
{
    public float initialSilenceSeconds;
    public float autoSilenceSeconds;
    public LUISDictationSink dictationSink;
   
    // TODO: Think about whether this should be married with the notion of
    // a focused object rather than just some 'global' entity.

    void Start()
    {
        this.recognizer = new DictationRecognizer();
        this.recognizer.InitialSilenceTimeoutSeconds = 0;
        this.recognizer.AutoSilenceTimeoutSeconds = 0;
        this.recognizer.DictationResult += OnDictationResult;
        this.recognizer.Start();       
    }
    void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        if ((confidence == ConfidenceLevel.Medium) ||
            (confidence == ConfidenceLevel.High) &&
            (this.dictationSink != null))
        {
            this.dictationSink.OnDictatedText(text);
        }
    }
    DictationRecognizer recognizer;
}
