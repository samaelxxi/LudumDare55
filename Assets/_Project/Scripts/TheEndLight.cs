using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TheEndLight : MonoBehaviour
{
    Vector3 _startPos;

    Light2D light2D = null;

    float _offset1 = 0;
    float _offset2 = 0;
    float _offset3 = 0;
    void Start()
    {
        light2D = GetComponent<Light2D>();
        // _hue = Random.Range(0, 1);
        _startPos = transform.position;
        StartCoroutine(Moving());
        StartCoroutine(ChangeHue());
        _offset1 = Random.Range(0, 100000);
        _offset2 = Random.Range(0, 100000);
        _offset3 = Random.Range(0, 100000);
        // light2D.color = Color.HSVToRGB(_hue, 1, 1);

    }

    float _hue;

    void Update()
    {
        light2D.intensity = Mathf.PingPong(Time.time+_offset1, 1.5f) + 1f;
        light2D.pointLightOuterRadius = Mathf.PingPong(Time.time + _offset2, 2f) + 2.5f;


    }

    IEnumerator ChangeHue()
    {
        _hue = Random.Range(0, 1.0f);
        // Debug.Log("hue: " + _hue);
        while (true)
        {
            _hue += Random.Range(0.1f, 0.2f) * Time.deltaTime;
            if (_hue > 1)
                _hue = 0;
            light2D.color = Color.HSVToRGB(_hue, 1, 1);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Moving()
    {
        while (true)
        {
            var newPos = _startPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
            float time = Random.Range(2, 4);
            transform.DOMove(newPos, time).SetEase(Ease.Linear);
            yield return new WaitForSeconds(time);
            yield return new WaitForSeconds(Random.Range(2, 4));
        }

    }
}
