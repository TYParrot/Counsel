using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;


using ApiConfig;

//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>
public class TTSManager: MonoBehaviour
{
    //public Button speakButton;
    public AudioSource audioSource;

    // Replace with your own subscription key and service region (e.g., "westus").
    private string SubscriptionKey;
    private string Region = "koreacentral";

    private const int SampleRate = 24000;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private bool audioSourceNeedStop;
    private string message;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    public void TTSStart(string aiMsg)
    {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        string newMessage = null;
        var startTime = DateTime.Now;

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesizer.StartSpeakingTextAsync(aiMsg).Result)
        {
            // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
            // Use the Unity API to play audio here as a short term solution.
            // Native playback support will be added in the future release.
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, // Can speak 10mins audio as maximum
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;
                    }
                });

            audioSource.clip = audioClip;
            audioSource.Play();
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }

            waitingForSpeak = false;
        }
    }

    void Start()
    {
        //API 할당
        SubscriptionKey = ApiCollection.ttsApiKey;

        // if (inputField == null)
        // {
        //     message = "inputField property is null! Assign a UI InputField element to it.";
        //     UnityEngine.Debug.LogError(message);
        // }
        // if (speakButton == null)
        // {
        //     message = "speakButton property is null! Assign a UI Button to it.";
        //     UnityEngine.Debug.LogError(message);
        // }
        // else
        // {
            // Continue with normal initialization, Text, InputField and Button objects are present.
            // inputField.text = "Enter text you wish spoken here.";
            message = "Click button to synthesize speech";
            //speakButton.onClick.AddListener(() => TTSStart("Hello, this is a test!"));


            // Creates an instance of a speech config with specified subscription key and service region.
            speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);

            /*  목소리 종류
                ko-KR-SunHiNeural(여성)
                ko-KR-InJoonNeural(남성)
                ko-KR-HyunsuMultilingualNeural 4(남성)
                ko-KR-BongJinNeural(남성)
                ko-KR-GookMinNeural(남성)
                ko-KR-HyunsuNeural(남성)
                ko-KR-JiMinNeural(여성)
                ko-KR-SeoHyeonNeural(여성)
                ko-KR-SoonBokNeural(여성)
                ko-KR-YuJinNeural(여성)
            */
            //목소리 설정
            speechConfig.SpeechSynthesisVoiceName = "ko-KR-SunHiNeural";

            // The default format is RIFF, which has a riff header.
            // We are playing the audio in memory as audio clip, which doesn't require riff header.
            // So we need to set the format to raw (24KHz for better quality).
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

            // Creates a speech synthesizer.
            // Make sure to dispose the synthesizer after use!
            synthesizer = new SpeechSynthesizer(speechConfig, null);

            synthesizer.SynthesisCanceled += (s, e) =>
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
                message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
            };
        // }
    }

    void Update()
    {
        lock (threadLocker)
        {
            // if (speakButton != null)
            // {
            //     speakButton.interactable = !waitingForSpeak;
            // }

            if (message != null)
            {
                Debug.Log(message);
                message = null;
            }

            if (audioSourceNeedStop)
            {
                audioSource.Stop();
                audioSourceNeedStop = false;
            }
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}

