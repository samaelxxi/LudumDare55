using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class LightIntensitySwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    float _nextIntensity = 0;

    [SerializeField] float _intensityChangeSpeed = 0.5f;

    Light2D _light;

    void Start()
    {
        _light = GetComponent<Light2D>();
        _nextIntensity = Random.Range(0.7f, 2f);
    }

    void Update()
    {
        if (Mathf.Abs(_light.intensity - _nextIntensity) < 0.01f)
        {
            _nextIntensity = Random.Range(0.7f,2f);
        }

        _light.intensity = Mathf.MoveTowards(_light.intensity, _nextIntensity, _intensityChangeSpeed * Time.deltaTime);
    }
}
