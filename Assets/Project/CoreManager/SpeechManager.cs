using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

using ApiConfig;

public class SpeechManager : MonoBehaviour
{
    private string sttResult;
    private string recognizedString = "";
    private string errorString = "";
    private string languageCode = "ko-KR";
    private string languageSignal = "korean";
    private SpeechRecognizer recognizer;

    //TextMeshProUGUI는 메인스레드에서만 실행이 가능함.
    public Text recognizedText;

    private System.Object threadLocker = new System.Object();

    //private TranslationRecognizer translator; //번역

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        lock(threadLocker){
            recognizedText.text = recognizedString;
        }
    }

    void OnDisable(){
        StopRecognition();
    }

    public async void StopRecognition(){
        if (recognizer != null)
        {
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            recognizer.Recognizing -= RecognizingHandler;
            recognizer.Recognized -= RecognizedHandler;
            recognizer.SpeechStartDetected -= SpeechStartDetectedHandler;
            recognizer.SpeechEndDetected -= SpeechEndDetectedHandler;
            recognizer.Canceled -= CanceledHandler;
            recognizer.SessionStarted -= SessionStartedHandler;
            recognizer.SessionStopped -= SessionStoppedHandler;
            recognizer.Dispose();
            recognizer = null;
            Debug.Log("Speech Recognizer is now stopped.");
        }
    }

    public void StartContinuous(){
        
        errorString = "";

        StartContinuousRecognition();
    }

    private async void StartContinuousRecognition(){
        
        Debug.Log("Starting Continuous Speech Recognition.");
        CreateSpeechRecognizer();

        if(recognizer != null){
            
            Debug.Log("Starting speech Recognizer.");
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        }
        Debug.Log("Start ContinuousRecognition exit");
    }

    void CreateSpeechRecognizer(){
        
        if(recognizer == null){
            
            SpeechConfig speechConfig = SpeechConfig.FromSubscription(ApiCollection.speechApiKey, ApiCollection.speechRegion);
            speechConfig.SpeechRecognitionLanguage = languageCode;
            recognizer = new SpeechRecognizer(speechConfig);

            if(recognizer != null){
                recognizer.Recognizing += RecognizingHandler;
                recognizer.Recognized += RecognizedHandler;
                recognizer.SpeechStartDetected += SpeechStartDetectedHandler;
                recognizer.SpeechEndDetected += SpeechEndDetectedHandler;
                recognizer.Canceled += CanceledHandler;
                recognizer.SessionStarted += SessionStartedHandler;
                recognizer.SessionStopped += SessionStoppedHandler;
            }
        }

        Debug.Log("Create Speech Recognizer Exit");
    }

    
#region Speech Recognition event handlers
    private void SessionStartedHandler(object sender, SessionEventArgs e)
    {
        Debug.Log($"\n    Session started event. Event: {e.ToString()}.");
    }

    private void SessionStoppedHandler(object sender, SessionEventArgs e)
    {
        Debug.Log($"\n    Session event. Event: {e.ToString()}.");
        Debug.Log($"Session Stop detected. Stop the recognition.");
    }

    private void SpeechStartDetectedHandler(object sender, RecognitionEventArgs e)
    {
        Debug.Log($"SpeechStartDetected received: offset: {e.Offset}.");
    }

    private void SpeechEndDetectedHandler(object sender, RecognitionEventArgs e)
    {
        Debug.Log($"SpeechEndDetected received: offset: {e.Offset}.");
        Debug.Log($"Speech end detected.");
    }

    // "Recognizing" events are fired every time we receive interim results during recognition (i.e. hypotheses)
    private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        if (e.Result.Reason == ResultReason.RecognizingSpeech)
        {
            Debug.Log($"HYPOTHESIS: Text={e.Result.Text}");
            lock (threadLocker)
            {
                //recognizedString = $"HYPOTHESIS: {Environment.NewLine}{e.Result.Text}";
            }
        }
    }

    // "Recognized" events are fired when the utterance end was detected by the server
    private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
    {
        if (e.Result.Reason == ResultReason.RecognizedSpeech)
        {
            Debug.Log($"RECOGNIZED: Text={e.Result.Text}");
            lock (threadLocker)
            {
                recognizedString = $"{Environment.NewLine}{e.Result.Text}";
            }
        }
        else if (e.Result.Reason == ResultReason.NoMatch)
        {
            Debug.Log($"NOMATCH: Speech could not be recognized.");
        }
    }

    // "Canceled" events are fired if the server encounters some kind of error.
    // This is often caused by invalid subscription credentials.
    private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
    {
        Debug.Log($"CANCELED: Reason={e.Reason}");

        errorString = e.ToString();
        if (e.Reason == CancellationReason.Error)
        {
            Debug.LogError($"CANCELED: ErrorDetails={e.ErrorDetails}");
            Debug.LogError("CANCELED: Did you update the subscription info?");
        }
    }
    #endregion
}
