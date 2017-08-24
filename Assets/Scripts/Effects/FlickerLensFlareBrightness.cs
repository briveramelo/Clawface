using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLensFlareBrightness : MonoBehaviour {

	LensFlare _flare;

    [SerializeField] float _flickerIntensity = 0.5f;
    [SerializeField] float _flickerFrequency = 0.1f;

    float _min;
    float _max;

    private void Awake() {
        _flare = GetComponent<LensFlare>();
        _max = _flare.brightness;
        _min = _max * _flickerIntensity;
        //StartCoroutine (Flicker());
    }

    IEnumerator Flicker () {
        while (true) {
            while (_flare == null) yield return null;

            _flare.brightness = Random.Range (_min, _max);

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
