using System;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class DictationSource : MonoBehaviour
{
    public event EventHandler DictationStopped;

    public float initialSilenceSeconds;
    public float autoSilenceSeconds;
    public DictationSink dictationSink;

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
        this.recognizer = null;

        // https://docs.microsoft.com/en-us/windows/mixed-reality/voice-input-in-unity
        // The challenge we have here is that we want to use both a KeywordRecognizer
        // and a DictationRecognizer at the same time or, at least, we want to stop
        // one, start the other and so on.
        // Unity does not like this. It seems that we have to shut down the 
        // PhraseRecognitionSystem that sits underneath them each time but the
        // challenge then is that this seems to stall the UI thread.
        // So far (following the doc link above) the best plan seems to be to
        // not call Stop() on the recognizer or Dispose() it but, instead, to
        // just tell the system to shutdown completely.
        PhraseRecognitionSystem.Shutdown();

        if (this.DictationStopped != null)
        {
            // And tell any friends that we are done.
            this.DictationStopped(this, EventArgs.Empty);
        }
    }
    DictationRecognizer recognizer;
}