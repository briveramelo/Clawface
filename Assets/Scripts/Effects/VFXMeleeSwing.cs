using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXMeleeSwing : MonoBehaviour {

	[SerializeField] float _startUV = 1f;
    [SerializeField] float _endUV = 1f;

    [SerializeField] float _effectSpeed = 1f;

    bool _playing = false;

    Material _mat;

    private void Awake() {
        _mat = GetComponent<MeshRenderer>().material;
        PlayAnimation();
    }

    private void Update() {
        if (_playing) {
            var dUV = (_endUV - _startUV) * Time.deltaTime * _effectSpeed;
            SetUV (Mathf.Clamp(_mat.mainTextureOffset.x + dUV, _startUV, _endUV));
            if (_mat.mainTextureOffset.x == _endUV) {
                _playing = false;
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public void PlayAnimation () {
        GetComponent<MeshRenderer>().enabled = true;
        SetUV (_startUV);
        _playing = true;
    }

    void SetUV (float newUV) {
        var offset = _mat.mainTextureOffset;
        offset.x = newUV;
        _mat.mainTextureOffset = offset;
    }
}
