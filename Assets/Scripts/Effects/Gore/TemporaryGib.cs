using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryGib : MonoBehaviour {

    const float _BLOOD_SMEAR_Y_OFFSET = 0.01f;
    const int _MAX_SMEAR_POINTS = 10;
    static Quaternion _BLOOD_SMEAR_ROTATION = Quaternion.Euler(90f, 0f, 0f);

    [SerializeField]
    float _lifetime = 5f;
    [SerializeField]
    float _fadeTime = 2f;
    [SerializeField]
    float _bloodPaintDist = 1f;
    [SerializeField]
    bool _doSmear = false;

    int _groundLayerMask = 1 << (int)Layers.Ground;

    float _smearY;

    Vector3[] _smearPoints;


    float _t;

    Color _color;
    Color _originalColor;

    Material _material;

    Rigidbody _rb;

    LineRenderer _lineRenderer;
    Vector3 _lastSmearPoint;


    private void Awake() {
        _material = GetComponentInChildren<MeshRenderer>(true).material;
        _originalColor = _material.color;
        _color = _material.color;

        if (_doSmear) {
            _lineRenderer = GetComponentInChildren<LineRenderer>(true);

            _lineRenderer.enabled = false;
            _smearPoints = new Vector3[_MAX_SMEAR_POINTS];
            _lineRenderer.positionCount = 0;
            _lineRenderer.SetPositions(_smearPoints);
        }

        StartCoroutine(LiveAndFade());
    }

    private void OnEnable() {
        _color = _originalColor;
        _material.color = _color;
    }

    private void OnCollisionEnter(Collision collision) {
        if (_doSmear) {

            if (collision.collider.gameObject.layer == (int)Layers.Ground) {
                var point = collision.contacts[0].point;
                var smearPos = transform.position;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, _groundLayerMask)) {
                    _smearY = hit.point.y;
                }

                smearPos.y = _smearY + _BLOOD_SMEAR_Y_OFFSET;
                _lineRenderer.enabled = true;
                _lastSmearPoint = smearPos;
                _smearPoints[0] = _lastSmearPoint;
                _smearPoints[1] = _lastSmearPoint;
                _lineRenderer.positionCount = 2;
            }
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (_doSmear) {
            if (collision.collider.gameObject.layer == (int)Layers.Ground) {
                var point = collision.contacts[0].point;
                var smearPos = transform.position;
                smearPos.y = point.y + _BLOOD_SMEAR_Y_OFFSET;
                var dist = Vector3.Distance(smearPos, _lastSmearPoint);
                if (dist >= _bloodPaintDist) {

                    if (_lineRenderer.positionCount < _MAX_SMEAR_POINTS - 1)

                        _lineRenderer.positionCount++;

                    var start = Mathf.Min(_lineRenderer.positionCount, _MAX_SMEAR_POINTS) - 1;
                    for (int i = start; i > 1; i--) {
                        _smearPoints[i] = _smearPoints[i - 1];
                    }

                    _smearPoints[1] = smearPos;
                    _lastSmearPoint = smearPos;

                } else {
                    _smearPoints[0] = smearPos;
                }

                _lineRenderer.SetPositions(_smearPoints);

                _lineRenderer.transform.rotation = _BLOOD_SMEAR_ROTATION;
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        //if (collision.collider.gameObject.layer == (int)Layers.Ground)
        //_lineRenderer.enabled = false;
    }

    private IEnumerator LiveAndFade() {
        yield return new WaitForSecondsRealtime(_lifetime);

        _t = _fadeTime;
        while (_t > 0f) {

            _t -= Time.deltaTime;
            _color.a = _t / _fadeTime;
            _material.color = _color;
            if (_doSmear) {
                var color = _lineRenderer.material.color;
                color.a = _color.a;
                _lineRenderer.material.color = color;
            }
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }
}
