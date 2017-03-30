using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerLightManager : MonoBehaviour {

	Dictionary<Light, float> _intensities = new Dictionary<Light, float>();

    [SerializeField] AnimationCurve _lightScaler;

    [SerializeField] List<Light> _ignoreList = new List<Light>();

    private void Awake() {
        var lights = FindObjectsOfType<Light>();
        foreach (var light in lights) {
            if (_ignoreList.Contains (light)) continue;

            _intensities.Add (light, light.intensity);

        }

        Application.targetFrameRate = 30;
    }

    private void Update() {
        ScaleLights (_lightScaler.Evaluate (Time.realtimeSinceStartup));
    }

    void ScaleLights (float value) {
        foreach (var light in _intensities.Keys) {
            light.intensity = _intensities[light] * value;
        }
    }
}
