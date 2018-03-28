using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RESTClient;
using UnityEngine.Windows.Speech;

namespace UnityBingSpeechRecognizer
{
    [Serializable]
    public class SimpleRecognitionResult
    {
        public string RecognitionStatus;
        public string DisplayText;
        public int Offset;
        public int Duration;
    }

    //[RequireComponent(typeof(AudioSource))]
    public class BingSpeechRecognizer : MonoBehaviour
    {
        public delegate void DictationCompletedDelegate(DictationCompletionCause cause);
        public delegate void DictationHypothesisDelegate(string text);
        public delegate void DictationResultDelegate(string text, ConfidenceLevel level);
        public delegate void DictationErrorHandler(string error, int hresult);

        public event DictationHypothesisDelegate DictationHypothesis;
        public event DictationResultDelegate DictationResult;
        public event DictationCompletedDelegate DictationComplete;
        public event DictationErrorHandler DictationError;

        private string apiUrl = "https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1";
        [SerializeField]
        private string key = ""; // Requires Bing Speech API Key
        [SerializeField]
        private string language = "en-GB";
        private const int MAX_DURATION = 15; // NB: 15 seconds is max duration for Bing Speech REST API

        [SerializeField,Range(1, 15)]
        private int maxRecordTime = 6;

        private string mic;
        private const int SAMPLE_RATE = 16000;
        private AudioClip _audioClip;
        private bool isRecording = false;
        private float recordingTime = 0;

        //private AudioSource _audioSource;        

        // Use this for initialization
        private void Awake()
        {
            status = SpeechSystemStatus.Stopped;
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Requires Bing Speech API Key");
                this.enabled = false;
                return;
            }

            // Test
            //if (DictationResult != null) DictationResult("create cube!", ConfidenceLevel.High);

            //_audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isRecording)
            {
                return;
            }
            recordingTime += Time.deltaTime;
            if (recordingTime > maxRecordTime)
            {
                Stop();
                return;
            }
        }

        //
        // Summary:
        // Starts the dictation recognization session. 
        public void StartRecording()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.Log("No microphone found.");
                return;
            }
            
            if (isRecording)
            {
                Debug.LogWarning("Already recording");
                return;
            }

            if (mic == null)
            {
                mic = Microphone.devices[0];
            }
            _audioClip = Microphone.Start(mic, false, maxRecordTime, SAMPLE_RATE);
            isRecording = true;
            status = SpeechSystemStatus.Running;
            Debug.Log("Start Recording: " + mic);
        }


        //
        // Summary:
        // Stops the dictation recognization session and posts recorded wav bytes to Bing Speech API.
        public void Stop()
        {
            Microphone.End(mic);

            recordingTime = 0;
            isRecording = false;
            status = SpeechSystemStatus.Stopped;

            if (_audioClip == null)
            {
                Debug.LogWarning("Empty audio clip");
                return;
            }

            byte[] bytes = WavUtility.FromAudioClip(_audioClip);
            Debug.Log("Recorded ended. AudioClip bytes: " + bytes.Length);

            _audioClip = null;

            PostWavBytes(bytes);
        }

        public void PostWavBytes(byte[] wavBytes)
        {
            StartCoroutine(Post(wavBytes, PostCompleted));
        }

        private IEnumerator Post(byte[] wavBytes, Action<IRestResponse<SimpleRecognitionResult>> callback = null)
        {
            RestRequest request = new RestRequest(apiUrl, Method.POST);
            request.AddHeader("Accept", "application/json;text/xml");
            request.AddHeader("Content-Type", "audio/wav; codec=audio/pcm; samplerate=16000");
            request.AddHeader("Ocp-Apim-Subscription-Key", key);

            request.AddQueryParam("language", language);
            request.AddQueryParam("format", "simple", true);

            request.AddBody(wavBytes, "audio/wav");
            request.Request.chunkedTransfer = false;

            yield return request.Request.SendWebRequest();
            request.ParseJson<SimpleRecognitionResult>(callback);
        }

        private void PostCompleted(IRestResponse<SimpleRecognitionResult> response)
        {
            if (response.IsError)
            {
                Debug.LogError("Request error: " + response.StatusCode);
                return;
            }
            string text = response.Data.DisplayText;
            Debug.Log("Completed: " + text + " status:" + response.Data.RecognitionStatus); // response.Content

            if (response.Data == null || !response.Data.RecognitionStatus.Equals("Success"))
            {
                // Handle failure
                Debug.LogWarning("Failed to recognize recording");
                status = SpeechSystemStatus.Failed;
                // Raise error event
                if (DictationError != null)
                    DictationError("Failed to recognize recording", 0);
                return;
            }

            // Speech dictation status is finished
            status = SpeechSystemStatus.Stopped;

            // Raise completion events
            if (DictationHypothesis != null)
                DictationHypothesis(text);
            if (DictationResult != null)
                DictationResult(text, ConfidenceLevel.High);
            if (DictationComplete != null)
                DictationComplete(DictationCompletionCause.Complete);
        }

        // TODO: Not implemented
        [SerializeField]
        private float initialSilenceTimeoutSeconds = 1;
        public float InitialSilenceTimeoutSeconds
        {
            get
            {
                return initialSilenceTimeoutSeconds;
            }
            set
            {
                if (value > maxRecordTime)
                {
                    this.initialSilenceTimeoutSeconds = maxRecordTime;
                }
                this.initialSilenceTimeoutSeconds = value;
            }
        }

        // TODO: Not implemented
        [SerializeField]
        private float autoSilenceTimeoutSeconds = 1;
        public float AutoSilenceTimeoutSeconds
        {
            get
            {
                return autoSilenceTimeoutSeconds;
            }
            set
            {
                if (value > maxRecordTime)
                {
                    this.autoSilenceTimeoutSeconds = maxRecordTime;
                }
                this.autoSilenceTimeoutSeconds = value;
            }
        }

        // Indicates the status of dictation recognizer.
        private SpeechSystemStatus status;
        public SpeechSystemStatus Status
        {
            get
            {
                return status;
            }
        }

        public void Dispose()
        {
            // clear resources
            _audioClip = null;
            Microphone.End(mic);
            mic = null;
        }

    }

}