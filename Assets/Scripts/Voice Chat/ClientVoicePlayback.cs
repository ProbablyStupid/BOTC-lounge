using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.Netcode;
using Unity.VisualScripting;

//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClientVoicePlayback : NetworkBehaviour
{
    // this is unnecessary!
    [SerializeField] AudioSource myAudioSource;

    [SerializeField] BotcPlayer myBotcPlayer;

    [SerializeField] int resampledLength = 0;

    [SerializeField] int bufferSize = 0;

    [SerializeField] int maxBufferSize = 48000;

    [SerializeField] bool test = false;

    // per-speaker FIFO buffer
    // private readonly Dictionary<ulong, Queue<float>> buffers = new();

    Queue<float> buffer = new Queue<float>();

    int chunkMs = 20;
    int sampleInRate = 16000;
    int sampleOutRate = 16000;
    float sampleRatio = 0;

    private void Start()
    {
        if (!IsOwner)
        {
            print("Registering " + OwnerClientId + " with ClientVoiceHandler!");
            FindFirstObjectByType<ClientVoiceHandler>().RegisterPlayerId(OwnerClientId, this);
        }

        //NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("VoiceReturn", OnVoiceDataReceived);

        sampleOutRate = AudioSettings.outputSampleRate;

        // 16000 / 48000 = 0.3...
        sampleRatio = (float)sampleInRate / (float)sampleOutRate;

        // 1 second of audio as maximum delay
        maxBufferSize = sampleOutRate;

        print("Audio playback at " + sampleOutRate + " samples per second (hz) with a ratio of " + sampleRatio);
    }

    

    public void EnqueueSamples(float[] samples)
    {
        foreach (float sample in samples)
        {
            buffer.Enqueue(sample);
        }
    }

    // I am very unfamiliar with how this works, so here is what I've concluded:
    //
    // Unity asks us for an output buffer.
    // that is `float[] data`
    // Instead of returning it, we just fill it in with data, because that's C# logic.
    // And so we fill it with the current samples we've received.
    // channels is ignored by us for now.
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (test)
        {
            print("Testing audio playback!");
            System.Random myRandom = new System.Random();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (float)myRandom.Next(100)/200f;
            }
            return;
        }

        if (buffer.Count > maxBufferSize)
        {
            print("I am " + OwnerClientId + " and my buffer is too large: " + buffer.Count);
        }
        while (buffer.Count > maxBufferSize)
        {
            buffer.Dequeue();
        }

        // this should not be necessary if we are filling in every data-slot with data from the buffer-Queue.
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = 0f;
        }

        //for (int i = 0; i< data.Length; i++)
        //{
        //    if (buffer.Count <= 0)
        //    {
        //        print("Not enough data in buffer on playback!!");
        //        break;
        //    }
        //    data[i] = buffer.Dequeue();
        //}

        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = buffer.Count > 0 ? buffer.Dequeue() : 0f;
            for (int channel = 0; channel < channels; channel++)
            {
                data[i + channel] = sample;
            }
        }

        for (int i = 0; i < data.Length; i++)
        {
            // Clamping to avoid clipping.
            data[i] = Mathf.Clamp(data[i], -1f, 1f);
        }
    }

    // Generates static to test Audio Device
    //private void OnAudioFilterRead(float[] data, int channels)
    //{
    //    System.Random myRandom = new System.Random();
    //    for (int i = 0; i < data.Length; i++)
    //    {
    //        data[i] = ((float)myRandom.Next(100)) - 50f / 100f;
    //    }
    //}
}
