using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class ClientVoiceHandler : NetworkBehaviour
{
    [SerializeField] private Dictionary<ulong, ClientVoicePlayback> players = new Dictionary<ulong, ClientVoicePlayback>();

    [SerializeField] int resampledLength = 0;

    [SerializeField] int bufferSize = 0;

    [SerializeField] int maxBufferSize = 48000;

    int chunkMs = 20;
    int sampleInRate = 16000;
    int sampleOutRate = 48000;
    float sampleRatio = 0;

    private void Start()
    {
        sampleOutRate = AudioSettings.outputSampleRate;

        // 16000 / 48000 = 0.3...
        sampleRatio = (float)sampleInRate / (float)sampleOutRate;
    }

    override public void OnNetworkSpawn()
    {
        print("I am " + OwnerClientId + " and I am registering my voice handler!");
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("VoiceReturn", OnVoiceDataReceived);
    }

    // TODO: we also needs some way to un-register players
    public void RegisterPlayerId(ulong id, ClientVoicePlayback playback)
    {
        print("I am " + OwnerClientId + " and I am registering player " + id + " for voice playback!");
        players.Add(id, playback);
    }

    void OnVoiceDataReceived(ulong senderid, FastBufferReader reader)
    {
        print("(I am" + OwnerClientId + ") -> receiving voice data!");

        //reader.ReadValueSafe(out ulong originalSenderId);

        // Here we need to figure out whether we are responsible for playing back this Audio.
        // This is a stupid way of doing this!
        //
        // Do note that we do NOT want to play this audio if the client we are on is our owner. Then we would be hearing ourselves.
        // if (originalSenderId != myBotcPlayer.myNetworkId.Value && originalSenderId != NetworkManager.LocalClientId) 
        //    return;

        reader.ReadValueSafe(out ulong originalSenderId);
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

        //for (int i = 0; i < resampled.Length; i++)
        //{
            //buffer.Enqueue(resampled[i]);
        //}

        print("(I am" + OwnerClientId + ") -> outputting my voice data!");

        players[originalSenderId].EnqueueSamples(resampled);

        //bufferSize = buffer.Count;
        //if (buffer.Count > maxBufferSize)
        //{
        //    print("(I am " + myBotcPlayer.name + ") -> Voice playback buffer at size " + buffer.Count + " is above maximum BufferSize!");
        //}
        //while (buffer.Count > maxBufferSize)
        //{
        //    buffer.Dequeue();
        //}

    }
}
