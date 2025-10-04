using System;
using System.Collections.Generic;
using Unity.Netcode;
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

    // per-speaker FIFO buffer
    // private readonly Dictionary<ulong, Queue<float>> buffers = new();

    Queue<float> buffer = new Queue<float>();

    int chunkMs = 20;
    int sampleInRate = 16000;
    int sampleOutRate = 16000;
    float sampleRatio = 0;

    private void Start()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("VoiceData", OnVoiceDataReceived);

        sampleOutRate = AudioSettings.outputSampleRate;

        // 16000 / 48000 = 0.3...
        sampleRatio = (float)sampleInRate / (float)sampleOutRate;

        // 1 second of audio as maximum delay
        maxBufferSize = sampleOutRate;

        print("Audio playback at " + sampleOutRate + " samples per second (hz) with a ratio of " + sampleRatio);
    }

    void OnVoiceDataReceived(ulong senderid, FastBufferReader reader)
    {
        //print("(I am a client) -> receiving voice data!");

        //reader.ReadValueSafe(out ulong originalSenderId);

        // Here we need to figure out whether we are responsible for playing back this Audio.
        // This is a stupid way of doing this!
        //
        // Do note that we do NOT want to play this audio if the client we are on is our owner. Then we would be hearing ourselves.
        // if (originalSenderId != myBotcPlayer.myNetworkId.Value && originalSenderId != NetworkManager.LocalClientId) 
        //    return;

        reader.ReadValueSafe(out int byteCount);
        byte[] pcmBytes = new byte[byteCount];
        reader.ReadBytesSafe(ref pcmBytes, byteCount, 0);

        short[] pcmShorts = new short[byteCount / 2];
        Buffer.BlockCopy(pcmBytes, 0, pcmShorts, 0, byteCount);
        float[] samples = new float[pcmShorts.Length];
        
        // this seems like a lot of computation. Too bad!
        for (int i = 0; i < pcmShorts.Length; ++i)
            samples[i] = pcmShorts[i] / (float)short.MaxValue;

        // this is what "sends" the audio data to the speaker to be played back.
        // OnAudioFilterRead will write the data in `buffer` to the output buffer.
        //buffer = new Queue<float>();

        // that means resampled should be 3x the length of samples.
        float[] resampled = new float[(int)((float)((float)samples.Length * (float)(1/(float)sampleRatio)))];
        //float[] resampled = new float[outputChunkSamples];
        //float[] resampled = new float[samples.Length];

        resampledLength = resampled.Length;

        //float realIndex = 0;

        //for (int i = 0; i < (int)((float)((float)samples.Length * (1/sampleRatio))); i++)
        //{
        //    if (Mathf.RoundToInt(realIndex) < samples.Length) {
        //        resampled[i] = samples[Mathf.RoundToInt(realIndex)];
        //    }
        //    else
        //    {
        //        // The realIndex will reach 320 and that is out of bounds for the original array, which will only ever reach 319.
        //        // At least that is my current theory.

        //        //print("RESAMPLING INDEX WAS OUT OF BOUNDS! -> " + Mathf.RoundToInt(realIndex));
        //        resampled[i] = 0f;
        //    }
        //    realIndex += sampleRatio;
        //}

        //for (int i = 0; i < samples.Length; i+=3)
        //{
        //    resampled[i] = samples[i];
        //    resampled[i+1] = samples[i];
        //    resampled[i+2] = samples[i];
        //}


        // Linear Interpolation Algorithm by ChatGPT
        //for (int i = 0; i < resampled.Length; i++)
        //{
        //    realIndex = (float)i * sampleRatio;
        //    int i0 = (int)realIndex;
        //    int i1 = Mathf.Min(i0 + 1, samples.Length -1);
        //    float frac = realIndex - i0;
        //    resampled[i] = Mathf.Lerp(samples[i0], samples[i1], frac);
        //}


        //foreach (float sample in resampled)
        //{
        //buffer.Enqueue(sample);
        //}

        for (int i = 0; i < resampled.Length; i++)
        {
            buffer.Enqueue(resampled[i]);
        }

        //print("(I am a client) -> outputting my voice data!");

        bufferSize = buffer.Count;
        while (buffer.Count > maxBufferSize)
        {
            buffer.Dequeue();
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
