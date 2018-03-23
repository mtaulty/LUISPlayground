using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class KeywordDictationSwitch : MonoBehaviour
{
    public string keyword = "listen";
    public DictationSource dictationSource;

    void Start()
    {
        this.NewRecognizer();
        this.dictationSource.DictationStopped += this.OnDictationStopped;
    }
    void NewRecognizer()
    {
        if (recognizer == null)
        {
            this.recognizer = new KeywordRecognizer(new string[] { this.keyword });
            this.recognizer.OnPhraseRecognized += this.OnPhraseRecgonized;
        }
        
        if (!recognizer.IsRunning)
        {
            this.recognizer.Start();
        }
    }
    void OnDictationStopped(object sender, System.EventArgs e)
    {
        this.NewRecognizer();
    }
    void OnPhraseRecgonized(PhraseRecognizedEventArgs args)
    {
        if (((args.confidence == ConfidenceLevel.Medium) ||
            (args.confidence == ConfidenceLevel.High)) &&
            (args.text == this.keyword) &&
            (this.dictationSource != null))
        {
            // There's some dragons here in trying to not have a KeywordRecognizer
            // and a DictationRecognizer active at the same time. Unity doesn't
            // like that and this code is trying to avoid that after much
            // experimenting.
            if (this.recognizer != null)
            {
                this.recognizer.Stop();
                this.recognizer.OnPhraseRecognized -= this.OnPhraseRecgonized;
                this.recognizer.Dispose();
                this.recognizer = null;
            }

            // Stop listening for activation keyword 
            PhraseRecognitionSystem.Shutdown();

            this.dictationSource.Listen();
        }
    }
    void StartDictation()
    {
        this.dictationSource.Listen();
    }
    KeywordRecognizer recognizer;
}