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
        if (_startRagdolled) EnterRagdoll(1f);
        else ExitRagdoll();
    }

    private void Update() {
        if (_ragdollTimer > 0f) {
            _ragdollTimer -= Time.deltaTime;
            if (_ragdollTimer <= 0f) ExitRagdoll();
        }
    }

    public void EnterRagdoll (float timer=-1f) {
        _animator.enabled = false;
        _ragdolled = true;
        _vb.enabled = false;
        _rb.velocity = Vector3.zero;
        _rb.detectCollisions = false;

        _ragdollTimer = timer;

        _controller.UpdateState (EMallCopState.Idle);

        foreach (var rb in _ragdollRBs) {

            // Skip parent RB
            if (rb == _rb) continue;

            //rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void ExitRagdoll () {
        _animator.enabled = true;
        _vb.enabled = true;
        _ragdolled = false;
        _rb.detectCollisions = true;
        transform.position = _skeleton.transform.position;
        _skeleton.transform.localPosition = Vector3.zero;

        _controller.UpdateState(EMallCopState.GettingUp);

        foreach (var rb in _ragdollRBs) {

            // Skip parent RB
            if (rb == _rb) continue;

            //rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
