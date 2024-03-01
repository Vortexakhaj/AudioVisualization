using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyBandCube : MonoBehaviour
{
    public int _bandCount;
    public float _startScale, _maxScale;
    public bool _useBuffer;
    public float _emissiveIntensity;
    Material _material;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer)
        {
            transform.localScale = new Vector3(1, (AudioPeer._audioBandBuffer[_bandCount] * _maxScale) + _startScale, 1);
            Color _color = new Color(AudioPeer._amplitudeBuffer64, AudioPeer._amplitudeBuffer64, AudioPeer._amplitudeBuffer64);
            _material.SetColor("_EmissionColor", _color * _emissiveIntensity);          
        }
        else
            transform.localScale = new Vector3(1, (AudioPeer._audioBandBuffer[_bandCount] * _maxScale) + _startScale, 1);
    }
}
