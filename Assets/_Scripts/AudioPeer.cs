using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audio;
    public AudioClip _audioClip;

    //Microphone
    public bool isMic;
    public string _selectedMic;
    public AudioMixerGroup _microphoneGroup, _masterGroup;

    //FFT Values
    float[] _samplesLeft = new float[512];
    float[] _samplesRight = new float[512];

    float[] _freqBands = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];
    float[] _freqBandHighest = new float[8];

    float[] _freqBands64 = new float[64];
    float[] _bandBuffer64 = new float[64];
    float[] _bufferDecrease64 = new float[64];
    float[] _freqBandHighest64 = new float[64];

    //Audio Band Values
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];
    public static float[] _audioBand64 = new float[64];
    public static float[] _audioBandBuffer64 = new float[64];


    //Single value to unify All band - Amplitude
    public static float _amplitude, _amplitudeBuffer;
    float _amplitudeHighest;

    public static float _amplitude64, _amplitudeBuffer64;
    float _amplitudeHighest64;

    public float _audioProfile; //to fix freqband staring with 0;

    //Stereo Switch
    public enum Channel { Stereo, Left, Right };
    public Channel _channel = new Channel();

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);

        //Initialize Mic
        if (!isMic)
        {
            _audio.outputAudioMixerGroup = _masterGroup;
            _audio.clip = _audioClip;
            _audio.Play();
            return;
        }

        if (Microphone.devices.Length > 0)
        {
            StartCoroutine(StartMic());
        }
        else
        {
            isMic = false;
        }
    }

    IEnumerator StartMic()
    {
        yield return new WaitForEndOfFrame();
        _selectedMic = Microphone.devices[0].ToString();
        _audio.outputAudioMixerGroup = _microphoneGroup;
        _audio.clip = Microphone.Start(_selectedMic, true, 10, AudioSettings.outputSampleRate);
        _audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        MakeFrequencyBands64();
        BandBuffer();
        BandBuffer64();
        CreateAudioBands();
        CreateAudioBands64();
        GetAmplitude();
        GetAmplitude64();
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < _freqBandHighest.Length; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    void GetSpectrumAudioSource()
    {
        _audio.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        _audio.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        //Audio Frequency 8 Band Chart
        //https://www.cuidevices.com/blog/understanding-audio-frequency-range-in-audio-design

        //0 - 2 = 0 - 86 Hz
        //1 - 4 = 87 - 258 Hz
        //2 - 8 = 259 - 602 HZ
        //3 - 16 = 603 - 1290 Hz
        //4 - 32 = 1291 - 2666 Hz
        //5 - 64 = 2667 - 5418 Hz
        //6 - 128 = 5419 - 10922 Hz
        //7 - 256 = 10923 - 21930 Hz


        int count = 0;
        for (int i = 0; i < _freqBands.Length; i++)
        {
            float avgBands = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7) sampleCount += 2;
            for (int j = 0; j < sampleCount; j++)
            {
                if (_channel == Channel.Stereo)
                {
                    avgBands += (_samplesLeft[count] + _samplesRight[count]) * (count + 1);
                }
                if (_channel == Channel.Left)
                {
                    avgBands += _samplesLeft[count] * (count + 1);
                }
                if (_channel == Channel.Right)
                {
                    avgBands += _samplesRight[count] * (count + 1);
                }
                count++;
            }
            avgBands /= count;
            _freqBands[i] = avgBands * 10;
        }


    }
    void MakeFrequencyBands64()
    {
        //0 - 15 = 1   = 16
        //16 - 31 = 2  = 32
        //32 - 39 = 4  = 40
        //40 - 47 = 6  = 46 
        //48 - 55 = 16 = 128
        //56 - 63 = 32 = 256 = 512


        int count = 0;
        int sampleCount = 1;
        int power = 0;
        for (int i = 0; i < _freqBands64.Length; i++)
        {
            float avgBands = 0;

            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power == 3)
                    sampleCount -= 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                if (_channel == Channel.Stereo)
                {
                    avgBands += (_samplesLeft[count] + _samplesRight[count]) * (count + 1);
                }
                if (_channel == Channel.Left)
                {
                    avgBands += _samplesLeft[count] * (count + 1);
                }
                if (_channel == Channel.Right)
                {
                    avgBands += _samplesRight[count] * (count + 1);
                }
                count++;
            }
            avgBands /= count;
            _freqBands64[i] = avgBands * 80;
        }


    }

    void BandBuffer()
    {
        for (int i = 0; i < _bandBuffer.Length; i++)
        {
            if (_freqBands[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _freqBands[i];
                _bufferDecrease[i] = 0.005f;
            }

            if (_freqBands[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.25f;
            }
        }
    }
    void BandBuffer64()
    {
        for (int i = 0; i < _bandBuffer64.Length; i++)
        {
            if (_freqBands64[i] > _bandBuffer64[i])
            {
                _bandBuffer64[i] = _freqBands64[i];
                _bufferDecrease64[i] = 0.005f;
            }

            if (_freqBands64[i] < _bandBuffer64[i])
            {
                _bandBuffer64[i] -= _bufferDecrease64[i];
                _bufferDecrease64[i] *= 1.25f;
            }
        }
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < _freqBandHighest.Length; i++)
        {
            if (_freqBands[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBands[i];
            }
            if (_freqBandHighest[i] > 0f)
            {
                _audioBand[i] = _freqBands[i] / _freqBandHighest[i];
                _audioBandBuffer[i] = _bandBuffer[i] / _freqBandHighest[i];
            }
        }
    }

    void CreateAudioBands64()
    {
        for (int i = 0; i < _freqBandHighest64.Length; i++)
        {
            if (_freqBands64[i] > _freqBandHighest64[i])
            {
                _freqBandHighest64[i] = _freqBands64[i];
            }
            if (_freqBandHighest64[i] > 0f)
            {
                _audioBand64[i] = _freqBands64[i] / _freqBandHighest64[i];
                _audioBandBuffer64[i] = _bandBuffer64[i] / _freqBandHighest64[i];
            }
        }
    }

    void GetAmplitude()
    {
        float _tempAmplitude = 0;
        float _tempAmplitudeBuffer = 0;
        for (int i = 0; i < _audioBand.Length; i++)
        {
            _tempAmplitude += _audioBand[i];
            _tempAmplitudeBuffer += _audioBandBuffer[i];
        }

        if (_tempAmplitude > _amplitudeHighest)
        {
            _amplitudeHighest = _tempAmplitude;
        }
        if (_amplitudeHighest > 0)
        {
            _amplitude = _tempAmplitude / _amplitudeHighest;
            _amplitudeBuffer = _tempAmplitudeBuffer / _amplitudeHighest;
        }
    }
    void GetAmplitude64()
    {
        float _tempAmplitude = 0;
        float _tempAmplitudeBuffer = 0;
        for (int i = 0; i < _audioBand64.Length; i++)
        {
            _tempAmplitude += _audioBand64[i];
            _tempAmplitudeBuffer += _audioBandBuffer64[i];
        }

        if (_tempAmplitude > _amplitudeHighest64)
        {
            _amplitudeHighest64 = _tempAmplitude;
        }
        if (_amplitudeHighest64 > 0)
        {
            _amplitude64 = _tempAmplitude / _amplitudeHighest64;
            _amplitudeBuffer64 = _tempAmplitudeBuffer / _amplitudeHighest64;
        }
    }
}
