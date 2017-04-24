using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour {

	Rigidbody[] _ragdollRBs;
    Animator _animator;
    Rigidbody _rb;
    [SerializeField] Transform _skeleton;

    [SerializeField] bool _startRagdolled = false;

    VelocityBody _vb;
    MallCopController _controller;

    bool _ragdolled = false;
    float _ragdollTimer;


    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _ragdollRBs = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        _vb = GetComponent<VelocityBody>();
        _controller = GetComponent<MallCopController>();
    }

    private void Start() {
        if (_startRagdolled) EnterRagdoll();
        else ExitRagdoll();
    }

    private void Update() {
        if (_ragdollTimer > 0f) {
            _ragdollTimer -= Time.deltaTime;

            if (_ragdollTimer <= 0f) {
                ExitRagdoll();
                _ragdollTimer = -1f;
            }
        }
    }

    public void EnterRagdoll (float timer=-1f) {
        _animator.enabled = false;
        _ragdolled = true;
        //_vb.enabled = false;
        //_rb.velocity = Vector3.zero;
        _rb.detectCollisions = false;

        _ragdollTimer = timer;

        _controller.UpdateState (EMallCopState.Idle);

        _vb.SetMovementMode (MovementMode.RAGDOLL);

        foreach (var rb in _ragdollRBs) {

            // Skip parent RB
            if (rb == _rb) continue;

            rb.constraints = RigidbodyConstraints.None;
            rb.detectCollisions = true;
            rb.useGravity = true;
        }
    }

    public void ExitRagdoll () {
        _animator.enabled = true;
        //_vb.enabled = true;
        _ragdolled = false;
        _rb.detectCollisions = true;
        transform.position = _skeleton.transform.position;
        _skeleton.transform.localPosition = Vector3.zero;

        _controller.UpdateState(EMallCopState.GettingUp);

        _vb.SetMovementMode (MovementMode.PRECISE);

        foreach (var rb in _ragdollRBs) {


            // Skip parent RB
            if (rb == _rb) continue;

            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.detectCollisions = false;
            rb.useGravity = true;
        }
    }
}
