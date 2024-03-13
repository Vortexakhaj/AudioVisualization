using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Instantiate512 : MonoBehaviour
{

    public GameObject _sampleCube;
    public AudioPeer AudioPeer;
    public GameObject[] _sampleCubes;
    public float _maxScale = 10f;


    //Angles
    public float fDistance;
    public float startAngle;
    public float minAngle;
    public float maxAngle;

    // Start is called before the first frame update
    void Start()
    {
        _sampleCubes = new GameObject[512];
        for (int i = 0; i < _sampleCubes.Length; i++)
        {
            /*GameObject _instanceSampleCube = Instantiate(_sampleCube, Vector3.forward * 100, Quaternion.Euler(new Vector3(0, -0.703125f * i, 0)), this.transform);*/
            GameObject _instanceSampleCube = Instantiate(_sampleCube, this.transform);
            //_instanceSampleCube.transform.position = this.transform.position;

            //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            this.transform.rotation = Quaternion.Euler(-0.703125f * i, 0f, 0f);

            _instanceSampleCube.transform.position = Vector3.forward * 100;
            _instanceSampleCube.transform.rotation = Quaternion.LookRotation(new Vector3(0f, -0.703125f * (i == 0 ? 1 : i), 0f));

            _instanceSampleCube.name = nameof(_sampleCube) + $"{i}";
            //_instanceSampleCube.GetComponentInChildren<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            _sampleCubes[i] = _instanceSampleCube;
        }
        for (int i = 0; i < AudioPeer._audioBandBuffer.Length; i++)
        {
            int cubeRange = _sampleCubes.Length / AudioPeer._audioBandBuffer.Length;
            int cubeRangeMin = (cubeRange * i) + i;
            Color _color = new Color(Random.Range(0, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            for (int j = cubeRangeMin - i; j < cubeRange * (i + 1); j++)
            {
                Material mymat = _sampleCubes[j].GetComponentInChildren<MeshRenderer>().material;
                mymat.SetColor("_Color", _color);
                mymat.SetColor("_EmissionColor", _color * (j == cubeRangeMin - i ? 10f : j == (cubeRange * (i + 1)) - 1 ? 10f : 5f));
            }
        }
    }

    void CalculateRadial()
    {
        if (transform.childCount == 0)
            return;
        float fOffsetAngle = ((maxAngle - minAngle)) / transform.childCount;

        float fAngle = startAngle;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null)
            {
                Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
                child.position = Vector3.Cross(vPos, Vector3.forward).normalized * fDistance;
                child.rotation = Quaternion.Euler(Mathf.Asin(vPos.x) * 180f / Mathf.PI, Mathf.Asin(vPos.y) * 180f / Mathf.PI, vPos.z);
                fAngle += fOffsetAngle;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < _sampleCubes.Length; i++)
        //{
        //    if (_sampleCubes == null) return;

        //    //_sampleCubes[i].transform.LookAt(Camera.main.transform);
        //    //_sampleCubes[i].transform.localScale = new Vector3(1, (AudioPeer._audioBandBuffer[i] * _maxScale) + 2, 1);

        //}

        //for (int i = 0; i < AudioPeer._audioBandBuffer.Length; i++)
        //{
        //    int cubeRange = (int)Mathf.Pow(2, i + 1) * 2;
        //    int cubeRangeMin = (int)Mathf.Pow(2, i + 1);
        //    if (cubeRangeMin == 2) cubeRangeMin = 0;
        //    for (int j = cubeRangeMin; j < cubeRange; j++)
        //    {

        //        _sampleCubes[j].transform.localScale = new Vector3(1, (AudioPeer._audioBandBuffer[i] * _maxScale) + 2, 1);
        //    }
        //}
        //CalculateRadial();
        //if (Input.GetKey(KeyCode.LeftControl))
        for (int i = 0; i < AudioPeer._audioBandBuffer.Length; i++)
        {
            int cubeRange = _sampleCubes.Length / AudioPeer._audioBandBuffer.Length;
            int cubeRangeMin = (cubeRange * i) + i;

            for (int j = cubeRangeMin - i; j < cubeRange * (i + 1); j++)
            {
                _sampleCubes[j].transform.localScale = new Vector3(1, (AudioPeer._audioBandBuffer[i] * _maxScale) + 2, 1);
            }
        }
    }
}
