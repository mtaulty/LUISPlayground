using System;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class DictationSource : MonoBehaviour
{
    public event EventHandler DictationStopped;

    public float initialSilenceSeconds;
    public float autoSilenceSeconds;
    public LUISDictationSink dictationSink;
   
    // TODO: Think about whether this should be married with the notion of
    // a focused object rather than just some 'global' entity.

    void NewRecognizer()
    {
        this.recognizer = new DictationRecognizer();
        this.recognizer.InitialSilenceTimeoutSeconds = this.initialSilenceSeconds;
        this.recognizer.AutoSilenceTimeoutSeconds = this.autoSilenceSeconds;
        this.recognizer.DictationResult += OnDictationResult;
        this.recognizer.DictationError += OnDictationError;
        this.recognizer.DictationComplete += OnDictationComplete;
        this.recognizer.Start();
    }
    public void Listen()
    {
        this.NewRecognizer();
    }
    void OnDictationComplete(DictationCompletionCause cause)
    {
        this.FireStopped();
    }
    void OnDictationError(string error, int hresult)
    {
        this.FireStopped();
    }
    void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        this.recognizer.Stop();

        if ((confidence == ConfidenceLevel.Medium) ||
            (confidence == ConfidenceLevel.High) &&
            (this.dictationSink != null))
        {
            this.dictationSink.OnDictatedText(text);
        }
    }
    void FireStopped()
    {
        this.recognizer.DictationComplete -= this.OnDictationComplete;
        this.recognizer.DictationError -= this.OnDictationError;
        this.recognizer.DictationResult -= this.OnDictationResult;
        this.recognizer.Dispose();
        this.recognizer = null;

        // There's some dragons here in trying to not have a KeywordRecognizer
        // and a DictationRecognizer active at the same time. Unity doesn't
        // like that and this code is trying to avoid that after much
        // experimenting. I still see Unity Assert on this line though :-(
        // in the editor.
        PhraseRecognitionSystem.Shutdown();

        if (this.DictationStopped != null)
        {
            this.DictationStopped(this, EventArgs.Empty);
        }
    }
    DictationRecognizer recognizer;
}