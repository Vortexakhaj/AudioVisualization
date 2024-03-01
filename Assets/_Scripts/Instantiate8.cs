using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate8 : MonoBehaviour
{

    public GameObject _sampleCubeVariant;
    public AudioPeer AudioPeer;
    public GameObject[] _sampleCubesVariant = new GameObject[8];
    public int _offset = 5;

    // Start is called before the first frame update
    void Start()
    {
        int offsetCount = (int)(_sampleCubesVariant.Length / 2) * _offset;
        for (int i = 0; i < _sampleCubesVariant.Length; i++)
        {
            GameObject _instanceSampleCube = Instantiate(_sampleCubeVariant, this.transform);
            _instanceSampleCube.transform.position = this.transform.position + new Vector3(0, 0, offsetCount);
            offsetCount -= _offset;
            _instanceSampleCube.name = nameof(_sampleCubeVariant) + $"{i}";
            _instanceSampleCube.GetComponent<FrequencyBandCube>()._bandCount = i;
            _instanceSampleCube.GetComponentInChildren<Renderer>().material.color = Color.yellow;
            _sampleCubesVariant[i] = _instanceSampleCube;
        }
        offsetCount = (int)(_sampleCubesVariant.Length / 2) * _offset;
        for (int i = 0; i < _sampleCubesVariant.Length; i++)
        {
            GameObject _instanceSampleCube = Instantiate(_sampleCubeVariant, this.transform);
            _instanceSampleCube.transform.position = this.transform.position + new Vector3(-50, 0, offsetCount);
            offsetCount -= _offset;
            _instanceSampleCube.name = nameof(_sampleCubeVariant) + $"{i}_buffer";
            _instanceSampleCube.GetComponent<FrequencyBandCube>()._bandCount = i;
            _instanceSampleCube.GetComponent<FrequencyBandCube>()._useBuffer = true;
            _instanceSampleCube.GetComponent<FrequencyBandCube>()._emissiveIntensity = i;
        }
    }
}
