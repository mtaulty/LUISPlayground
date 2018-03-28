using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class KeywordDictationSwitch : MonoBehaviour
{
    public string[] keywords = { "ok", "now", "hey", "listen" };
    public DictationSource dictationSource;

    void Start()
    {
        this.NewRecognizer();
        this.dictationSource.DictationStopped += this.OnDictationStopped;
    }
    void NewRecognizer()
    {
        this.recognizer = new KeywordRecognizer(this.keywords);
        this.recognizer.OnPhraseRecognized += this.OnPhraseRecgonized;
        this.recognizer.Start();
    }
    void OnDictationStopped(object sender, System.EventArgs e)
    {
        this.NewRecognizer();
    }
    void OnPhraseRecgonized(PhraseRecognizedEventArgs args)
    {
        if (((args.confidence == ConfidenceLevel.Medium) ||
            (args.confidence == ConfidenceLevel.High)) &&
            this.keywords.Contains(args.text.ToLower()) &&
            (this.dictationSource != null))
        {
            this.recognizer.OnPhraseRecognized -= this.OnPhraseRecgonized;
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

            // And then start up the other system.
            this.dictationSource.Listen();
        }
        else
        {
            Debug.Log(string.Format("Dictation: Listening for keyword {0}, heard {1} with confidence {2}, ignored",
                this.keywords,
                args.text,
                args.confidence));
        }
    }
    void StartDictation()
    {
        this.dictationSource.Listen();
    }
    KeywordRecognizer recognizer;
}