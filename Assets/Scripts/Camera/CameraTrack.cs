using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraTrack : MonoBehaviour {

    const float _GIZMO_SPHERE_RADIUS = 0.1f;

    Color _editorColor = Color.blue;
    Color _idleColor = Color.white;
    Color _playingColor = Color.red;

    [SerializeField] Camera _cameraToMove;

    [SerializeField] bool _playOnAwake = false;

    [SerializeField] AnimationCurve _speed;

	[SerializeField] List<PositionInfo> _positions = new List<PositionInfo>();

    [SerializeField] public UnityEvent onCompleteTrack = new UnityEvent();

    [SerializeField] int _nameCount = 0;

    float _t = 0f;
    float _progress = 0f;

    bool _playing = false;

    [SerializeField]
    List<CameraEvent> _events = new List<CameraEvent>();

    List<CameraEvent> _eventsToDo = new List<CameraEvent>();

    private void Awake() {
        if (_playOnAwake) PlayFromBeginning();
    }

    private void Start() {
        foreach (var camEvent in _events)
            _eventsToDo.Add (camEvent);
    }

    private void Update() {
        if (_playing) {
            //_t += _speed.Evaluate (_t) * Time.deltaTime;
            _t += Time.deltaTime;

            _progress = Mathf.Clamp(_speed.Evaluate (_t), 0f, _positions.Count - 1);

            int backPos = Mathf.FloorToInt (_progress);
            int forwardPos = Mathf.CeilToInt (_progress);
            var position = Vector3.Lerp (_positions [backPos].transform.position, _positions[forwardPos].transform.position, _progress - backPos);
            var rotation = Quaternion.Lerp (_positions [backPos].transform.rotation, _positions[forwardPos].transform.rotation, _progress - backPos);
            _cameraToMove.transform.position = position;
            _cameraToMove.transform.rotation = rotation;
            _cameraToMove.fieldOfView = Mathf.Lerp (_positions[backPos].fov, _positions[forwardPos].fov, _progress - backPos);

            for (int i = 0; i < _events.Count; i++) {
                var camEvent = _events[i];
                if (camEvent.time <= _t) {
                    camEvent.onTrigger.Invoke();
                    _events.RemoveAt (i);
                }
            }

            if (_progress == _positions.Count - 1) {
                onCompleteTrack.Invoke();
                Pause();
            }
        }
    }

    private void OnDrawGizmos() {
        if (_playing) Gizmos.color = _playingColor;
        else {
            if (Application.isPlaying) Gizmos.color = _idleColor;
            else Gizmos.color = _editorColor;
        }

        for (int i = 0; i < _positions.Count; i++) {
            if (_positions[i].transform == null) {
                _positions.RemoveAt (i);
                continue;
            }

            var positionTransform = _positions[i].transform;
            Gizmos.matrix = positionTransform.localToWorldMatrix;
            if (_cameraToMove != null)
                Gizmos.DrawFrustum (positionTransform.position, _positions[i].fov, 1f, 0.1f, _cameraToMove.aspect);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawSphere (positionTransform.position, _GIZMO_SPHERE_RADIUS);
            Gizmos.DrawRay (positionTransform.position, positionTransform.rotation * Vector3.forward);
            if (i > 0) Gizmos.DrawLine (positionTransform.position, _positions[i-1].transform.position);
            
        }

        Gizmos.color = Color.white;
        if (_cameraToMove != null) {
            Gizmos.matrix = _cameraToMove.transform.localToWorldMatrix;
            Gizmos.DrawFrustum (_cameraToMove.transform.position, _cameraToMove.fieldOfView, 1f, 0.1f, _cameraToMove.aspect);
        }
    }

    public List<PositionInfo> Positions { get { return _positions; } }

    public Camera CameraToMove {
        get { return _cameraToMove; }
        set { _cameraToMove = value; }
    }

    public float Progress { get { return _progress; } }

    public float PlaybackTime { get { return _t; } }

    public bool IsPlaying { get { return _playing; } }

    public void PlayFromBeginning () {
        _t = 0f;
        _progress = 0f;
        Play();
    }

    public void Play () {
        if (_cameraToMove == null)
            throw new MissingReferenceException ("Missing camera reference!");

        else if (_positions == null)
            throw new MissingReferenceException ("No positions specified!");

        else if (_positions.Count == 0)
            throw new MissingReferenceException ("No positions specified!");

        else {
            _playing = true;
        }
    } 

    public void Pause () {
        _playing = false;
    }

    public void Resume () {
        _playing = true;
    }

    public void Stop () {
        _playing = false;
        _t = 0f;
        _progress = 0f;

        _cameraToMove.transform.position = _positions[0].transform.position;
        _cameraToMove.transform.rotation = _positions[0].transform.rotation;
        _cameraToMove.fieldOfView = _positions[0].fov;

        _eventsToDo.Clear();
        foreach (var camEvent in _events) _eventsToDo.Add (camEvent);
    }

    public void AddPosition () {
        AddPositionAfterIndex (_positions.Count - 1);
    }

    public void AddPositionAfterIndex (int index) {
        PositionInfo newPosition = new PositionInfo();
        Transform newTransform = new GameObject ("Pos" + _nameCount++.ToString(),
            typeof(SphereCollider)).transform;
        newTransform.gameObject.GetComponent<SphereCollider>().radius = _GIZMO_SPHERE_RADIUS;
        newTransform.SetParent (transform);

        // If no positions yet, place new position at camera track position.
        // Otherwise, place new position at most recently created position.
        Transform transformToCopy = (index == -1 ? transform : _positions [index].transform);
        
        newTransform.position = transformToCopy.position;
        newTransform.rotation = transformToCopy.rotation;

        // First position
        if (index == -1) {
            if (_speed == null)
                _speed = new AnimationCurve();

            _speed.AddKey (0f, 0f);
            _speed.AddKey (1f, 1f);

        // Position at end
        } else if (index == _positions.Count - 1) {
            var lastKey = _speed.keys [_speed.length - 1];
            _speed.AddKey (lastKey.time + 1f, lastKey.value + 1f);

        // In between other positions
        } else {
            var prevKey = _speed.keys[index];
            var nextKey = _speed.keys[index+1];
            _speed.AddKey ((prevKey.time + nextKey.time) / 2f, (prevKey.value + nextKey.value) / 2f);
        }

        newPosition.transform = newTransform;
        _positions.Insert (index + 1, newPosition);
        newTransform.SetSiblingIndex (_positions.IndexOf(newPosition));
    }

    public void MovePositionForward (int index) {
        if (index == _positions.Count - 1) return;

        var temp = _positions[index + 1];
        _positions[index + 1] = _positions[index];
        _positions[index] = temp;

        _positions[index+1].transform.SetSiblingIndex (index+1);
        _positions[index].transform.SetSiblingIndex (index);
    }

    public void MovePositionBackward (int index) {
        if (index == 0) return;

        var temp = _positions[index - 1];
        _positions[index - 1] = _positions[index];
        _positions[index] = temp;

        _positions[index-1].transform.SetSiblingIndex (index-1);
        _positions[index].transform.SetSiblingIndex (index);
    }

    public void DeletePositionAtIndex (int index) {
        GameObject obj = _positions[index].transform.gameObject;
        DestroyImmediate (obj);
        _positions.RemoveAt (index);
    }

    public void AddCameraEvent () {
        _events.Add (new CameraEvent());
    }

    [System.Serializable]
    public class PositionInfo {
        public Transform transform;
        public float fov = 60f;
    }

    [System.Serializable]
    public class CameraEvent {
        [SerializeField]
        public UnityEvent onTrigger = new UnityEvent();

        [SerializeField]
        public float time = 0f;
    }
}
