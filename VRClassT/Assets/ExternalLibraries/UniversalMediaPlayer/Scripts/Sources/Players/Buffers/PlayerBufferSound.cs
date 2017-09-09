using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UMP.Wrappers;
using UMP;

/// <summary>
/// VLC sound formats
/// </summary>
internal enum SoundType
{
    /// <summary>
    /// 16 bits per sample
    /// </summary>
    S16N
}

internal class PlayerBufferSound
{
    Wrapper _wrapper;
    List<float> _samplesData;

    public static SoundType GetSoundType(string format)
    {
        switch (format)
        {
            case "S16N":
                return SoundType.S16N;

            default:
                return SoundType.S16N;
        }
    }

    /// <summary>
    /// Initializes new instance of SoundFormat class
    /// </summary>
    /// <param name="soundType"></param>
    /// <param name="rate"></param>
    /// <param name="channels"></param>
    public PlayerBufferSound(SoundType soundType, int rate, int channels)
    {
        _wrapper = Wrapper.Instance;
        SoundType = soundType;
        Format = soundType.ToString();
        Rate = rate;
        Channels = channels;
        Init();
        BlockSize = BitsPerSample / 8 * Channels;
        _samplesData = new List<float>();
    }

    private void Init()
    {
        switch (SoundType)
        {
            case SoundType.S16N:
                BitsPerSample = 16;
                break;
        }
    }

    public void UpdateSamplesData(IntPtr samples, uint count)
    {
        lock (_samplesData)
        {
            int audioFrameLength = BlockSize * (int)count;
            IntPtr point = _wrapper.NativeHelperGetAudioSamples(samples, audioFrameLength);
            float[] buffer = new float[audioFrameLength / 2];
            Marshal.Copy(point, buffer, 0, buffer.Length);

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Win)
                Marshal.FreeCoTaskMem(point);

            _samplesData.AddRange(buffer);
            buffer = null;
        }
    }

    /// <summary>
    /// Audio format
    /// </summary>
    public string Format { get; private set; }

    /// <summary>
    /// Sampling rate in Hz
    /// </summary>
    public int Rate { get; private set; }

    /// <summary>
    /// Number of channels used by audio sample
    /// </summary>
    public int Channels { get; private set; }

    /// <summary>
    /// Size of single audio sample in bytes
    /// </summary>
    public int BitsPerSample { get; private set; }

    /// <summary>
    /// Specifies sound sample format
    /// </summary>
    public SoundType SoundType { get; private set; }

    /// <summary>
    /// Size of audio block (BitsPerSample / 8 * Channels)
    /// </summary>
    public int BlockSize { get; private set; }

    public List<float> SamplesData
    {
        get
        {
            lock (_samplesData)
                return _samplesData;
        }
    }

    /// <summary>
    /// Playback time stamp in microseconds
    /// </summary>
    public long Pts { get; set; }
}
