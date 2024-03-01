using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate512 : MonoBehaviour
{

    public GameObject _sampleCube;
    public AudioPeer AudioPeer;
    public GameObject[] _sampleCubes = new GameObject[512];
    public float _maxScale = 10f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _sampleCubes.Length; i++)
        {
            /*GameObject _instanceSampleCube = Instantiate(_sampleCube, Vector3.forward * 100, Quaternion.Euler(new Vector3(0, -0.703125f * i, 0)), this.transform);*/
            GameObject _instanceSampleCube = Instantiate(_sampleCube, this.transform);
            _instanceSampleCube.transform.position = this.transform.position;
            this.transform.rotation = Quaternion.Euler(0f, -0.703125f * i, 0f);
            _instanceSampleCube.transform.position = Vector3.forward * 100;

            _instanceSampleCube.name = nameof(_sampleCube) + $"{i}";
            _sampleCubes[i] = _instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //for(int i = 0; i < _sampleCubes.Length; i++)
        //{
        //    if (_sampleCubes == null) return;

        //    _sampleCubes[i].transform.localScale = new Vector3(1, (AudioPeer._samples[i] * _maxScale) + 2, 1);

        //}

        for (int i = 0; i < AudioPeer._audioBandBuffer.Length; i++)
        {
            int cubeRange = (int)Mathf.Pow(2, i + 1) * 2;
            int cubeRangeMin = (int)Mathf.Pow(2, i + 1);
            if (cubeRangeMin == 2) cubeRangeMin = 0;
            for (int j = cubeRangeMin; j < cubeRange; j++)
            {
                _sampleCubes[j].transform.localScale = new Vector3(1, (AudioPeer._audioBandBuffer[i] * _maxScale) + 2, 1);
            }
        }
    }
}
