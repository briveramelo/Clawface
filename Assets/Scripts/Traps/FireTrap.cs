using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class FireTrap : MonoBehaviour {

    const float _DOOR_OPEN_DISTANCE = 0.25f;
    const float _DOOR_Y = 0.2f;
    const float _GRATE_LIFT_DISTANCE = 0.25f;
    const float _GRATE_MIN_Y = -0.03f;
    const float _GRATE_MAX_Y = 0.02f;

    enum State {
        Closed,
        Open,
        Opening,
        Closing
    }

    public enum Mode {
        ContinuousStream,
        ContinuousOpenClose,
        PressureTrigger
    }

    Transform _door1;

    Transform _door2;

    Transform _grate;

    //Collider _damageVolume;

    [Tooltip("How long it takes for the trap to open (seconds).")]
    [SerializeField]
    float _openTime = 1f;

    [Tooltip("How long the trap stays open (seconds).")]
    [SerializeField]
    float _stayOpenTime = 3f;

    [Tooltip("How long it takes for the trap to close (seconds).")]
    [SerializeField]
    float _closeTime = 1f;

    [Tooltip("How long the trap stays closed (seconds).")]
    [SerializeField]
    float _stayClosedTime = 3f;

    [Tooltip("How much damage to deal to objects in the trap per second.")]
    [SerializeField]
    float _damagePerSecond = 10f;

    float _damageTimer = 0f;

    [Tooltip("Type of trap functionality.")]
    [SerializeField]
    Mode _mode;

    float _t;

    float _stateTimer;

    State _currentState = State.Closed;

    List<IDamageable> _objectsInTrap = new List<IDamageable>();
    private Damager damager=new Damager();


    VFXFireEffect _fireEffect;

    void Awake() {
        _door1 = gameObject.FindInChildren("Door1").transform;
        _door2 = gameObject.FindInChildren("Door2").transform;
        _grate = gameObject.FindInChildren("Grate").transform;
        _fireEffect = GetComponentInChildren<VFXFireEffect>();
        damager.Set(_damagePerSecond, DamagerType.FireTrap, Vector3.up);
        //_damageVolume = GetComponent<Collider>();

        if (_mode == Mode.ContinuousStream) Open();
    }

    void Update() {
        var dt = Time.deltaTime;

        switch (_currentState) {
            case State.Closing:
                if (_t >= dt) _t -= dt;
                else {
                    _t = 0f;
                    _currentState = State.Closed;

                    if (_mode == Mode.ContinuousOpenClose)
                        _stateTimer = _stayClosedTime;
                }

                DrawFrame(_t / _closeTime);
                break;

            case State.Opening:
                if (_t <= _openTime - dt) _t += dt;
                else {
                    _t = _openTime;
                    _currentState = State.Open;

                    if (_fireEffect != null) _fireEffect.Play();

                    if (_mode == Mode.ContinuousOpenClose)
                        _stateTimer = _stayOpenTime;
                }

                DrawFrame(_t / _openTime);
                break;

            case State.Open:
                _damageTimer -= dt;
                if (_damageTimer <= 0f) {
                    DoDamage();
                    _damageTimer += 1f;
                }

                if (_mode == Mode.ContinuousOpenClose) {
                    _stateTimer -= dt;
                    if (_stateTimer <= 0f) Close();
                }
                break;

            case State.Closed:
                if (_mode == Mode.ContinuousOpenClose) {
                    _stateTimer -= dt;
                    if (_stateTimer <= 0f) Open();
                }
                break;
        }
    }

    //private void OnCollisionEnter(Collision collision) {
    void OnTriggerEnter (Collider other) {
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable == null) return;

        if (!_objectsInTrap.Contains (damageable)) {
            _objectsInTrap.Add(damageable);
            if (_mode == Mode.PressureTrigger && _currentState == State.Closed) Open();
        }
    }

    private void OnTriggerExit(Collider other) {
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable == null) return;

        if (_objectsInTrap.Contains(damageable)) {
            _objectsInTrap.Remove (damageable);
            if (_objectsInTrap.Count == 0 && _currentState == State.Open) Close();
        }
    }

    public Mode TrapMode {
        get { return _mode; }
        set { _mode = value; }
    }

    public float OpenTime {
        get { return _openTime; }
        set { _openTime = value; }
    }

    public float StayOpenTime {
        get { return _stayOpenTime; }
        set { _stayOpenTime = value; }
    }

    public float DamagePerSecond {
        get { return _damagePerSecond; }
        set { _damagePerSecond = value; }
    }

    public float CloseTime {
        get { return _closeTime; }
        set { _closeTime = value; }
    }

    public float StayClosedTime {
        get { return _stayClosedTime; }
        set { _stayClosedTime = value; }
    }

    public void Open() {
        if (_currentState == State.Open) return;

        _currentState = State.Opening;
    }

    public void Close() {
        if (_currentState == State.Closed) return;
        if (_fireEffect != null) _fireEffect.Stop();
        _currentState = State.Closing;
    }

    void DrawFrame(float t) {
        _door1.localPosition = new Vector3(0f, _DOOR_Y, t * _DOOR_OPEN_DISTANCE);
        _door2.localPosition = new Vector3(0f, _DOOR_Y, -t * _DOOR_OPEN_DISTANCE);

        var grateY = _GRATE_MIN_Y + (_GRATE_MAX_Y - _GRATE_MIN_Y) * t;
        _grate.localPosition = new Vector3(0f, grateY, 0f);
    }

    void DoDamage() {        
        foreach (var obj in _objectsInTrap) {
            obj.TakeDamage(damager);
        }
    }
}
