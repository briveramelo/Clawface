using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour {

	Light _light;

    [SerializeField] float _flickerIntensity = 0.5f;
    [SerializeField] float _flickerFrequency = 0.1f;

    float _min;
    float _max;

    private void Awake() {
        _light = GetComponent<Light>();
        _max = _light.intensity;
        _min = _max * _flickerIntensity;
        StartCoroutine (Flicker());
    }

    IEnumerator Flicker () {
        while (true) {
            while (_light == null) yield return null;

            _light.intensity = Random.Range (_min, _max);

            yield return new WaitForSeconds (_flickerFrequency);
        }
    }

    public void Play() {
        StartCoroutine (Flicker());
    }

    public void Stop () {
        StopCoroutine (Flicker());
    }
}
