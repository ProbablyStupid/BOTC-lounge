using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This script should be applied to a player respectively.
/// </summary>
public class ClientVoiceCapture : NetworkBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioClip micClip;
    [SerializeField] private string micDevice;
    // 16khz should be fine given bandwidth constraints.
    [SerializeField] private int sampleRate = 16000;
    [SerializeField] private int lastReadPosition = 0;

    [SerializeField] private int chunkMs = 20;
    [SerializeField] private int chunkSamples;

    [SerializeField] AudioSource myAudioSource;

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();

        chunkSamples = (sampleRate * chunkMs) / 1000;

        if (Microphone.devices.Length > 0)
        {
            // 0 ist the default system Microphone
            micDevice = Microphone.devices[0];
            print("Selecting default Microphone device: " + Microphone.devices[0]);

            micClip = Microphone.Start(micDevice, true, 1, sampleRate);
            print("Microphone frequency : " + micClip.samples);
            audioSource.loop = true;

            print("Microphone frequency : " + micClip.samples);


            // wait for the microphone to start. This is terrible!!
            while (Microphone.GetPosition(micDevice) <= 0) { }

            print("Microphone frequency : " + micClip.samples);

        } else
        {
            Debug.Log("NO MICROPHONE ON CLIENT --> CANNOT ENGAGE VOICE CHAT!");
            // TODO: implement a visual warning mechanic!
        }

        print("Microphone frequency : " + micClip.samples);
    }

    public float[] GetFloatPCM()
    {
        int micPos = Microphone.GetPosition(micDevice);
        float[] samples = new float[micClip.samples * micClip.channels];
        micClip.GetData(samples, 0);

        return samples;
    }

    public void SendSamples(float[] samples)
    {
        // "compression" to bytes to save bandwidth

        short[] pcmData = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            pcmData[i] = (short)(samples[i] * short.MaxValue);
        }

        byte[] bytePcmData = new byte[pcmData.Length * 2];
        System.Buffer.BlockCopy(pcmData, 0, bytePcmData, 0, bytePcmData.Length);

        FastBufferWriter writer = new FastBufferWriter(4 + bytePcmData.Length, Allocator.Temp);
        writer.WriteValueSafe<int>(bytePcmData.Length);
        writer.WriteBytes(bytePcmData, bytePcmData.Length, 0);

        // This is stupid. NetworkManager.ServerClientId is 0. Always.
        // Also, we use NetworkDelivery.UnreliableSequenced to ideally improve latency / performance. Haven't tested the actual impact this makes.
        // - this might also be dependant on the client network. We'll leave it like this for now.
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("VoiceData", NetworkManager.ServerClientId, writer, NetworkDelivery.UnreliableSequenced);
    }

    public short[] FloatPCMtoShortPCM(float[] samples)
    {
        short[] pcm = new short[samples.Length];
        for (int i = 0; i < samples.Length; ++i)
        {
            //pcm[i] = (short)Mathf.Clamp(Mathf.RoundToInt())
            pcm[i] = (short)(samples[i] * short.MaxValue);
        }
        return pcm;
    }

    public byte[] ShortPCMtoBytePCM(short[] samples)
    {
        byte[] bytes = new byte[samples.Length * 2];
        System.Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);

        return bytes;
    }

    public FastBufferWriter BytesPCMtoFastBufferWriter(byte[] bytes)
    {
        FastBufferWriter writer = new FastBufferWriter(4 + bytes.Length, Allocator.Temp);
        // Memory layout
        writer.TryBeginWrite(sizeof(int) + bytes.Length);
        writer.WriteValueSafe<int>(bytes.Length);
        writer.WriteBytes(bytes, bytes.Length, 0);

        return writer;
    }

    void Update()
    {
        // We shouldn't need this, but it's better to be safe than risk colliding voice data.
        if (!IsOwner) return;

        // Update() will sometimes run before everything is nicely initialized. This should prevent buggy behaviour.
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsClient) return;

        int micPos = Microphone.GetPosition(micDevice);
        int available = micPos - lastReadPosition;

        // WHY?
        if (available < 0)
        {
            available += micClip.samples;
        }

        while (available >= chunkSamples)
        {
            // fixed chunk size
            float[] samples = new float[chunkSamples];

            int readStart = lastReadPosition % micClip.samples;
            micClip.GetData(samples, readStart);
            lastReadPosition = (lastReadPosition + chunkSamples) % micClip.samples;
            available -= chunkSamples;

            FastBufferWriter writer = BytesPCMtoFastBufferWriter(ShortPCMtoBytePCM(FloatPCMtoShortPCM(samples)));
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("VoiceData", NetworkManager.ServerClientId, writer, NetworkDelivery.UnreliableSequenced);
        }
    }

    // this will NOT work because get_samples can only be executed from the main thread
    //private void OnAudioFilterRead(float[] data, int channels)
    //{
    //    if ((Microphone.GetPosition(micDevice) -  lastReadPosition) >= chunkSamples)
    //    {
    //        float[] samples = new float[chunkSamples];
    //        int readStart = lastReadPosition % micClip.samples;
    //        micClip.GetData(samples, readStart);
    //        lastReadPosition = (lastReadPosition + chunkSamples) % micClip.samples;

    //        for (int i = 0; i < samples.Length; i++)
    //        {
    //            data[i] = samples[i];
    //        }
    //    }
    //}
}
