using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLEEnemyPreview : MonoBehaviour {

    [SerializeField] Animator animator;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        animator.SetInteger("AnimationState", (int)AnimationStates.Idle);
    }
}
