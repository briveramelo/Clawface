using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turing.VFX
{

    public class LightningChain : MonoBehaviour
    {

        [SerializeField]
        Material[] materials;

        private LineRenderer lineRender;
        public Transform origin;
        public Transform destination;
        private float distance;
        private float count;
        public float lightningTrailSpeed;

        private float randomWithOffsetMin = 2.5f;
        private float randomWithOffsetMax = 5.0f;

        private float materialSwitchInterval = 0.03f;

        private Vector3[] points = new Vector3[2];

        private void Awake()
        {
            lineRender = GetComponent<LineRenderer>();
            lineRender.startWidth = RandomWidthOffset();
            lineRender.endWidth = RandomWidthOffset();
            Reset();
        }

        private void OnEnable()
        {
            Reset();
        }

        private void OnDisable()
        {
            Reset();
        }

        public void SetOrigin (Transform origin)
        {
            if (origin == null) Debug.LogError ("Invalid origin!");
            this.origin = origin;
            lineRender.SetPosition (0, origin.position);
        }

        public void SetTarget (Transform target)
        {
            if (target == null) Debug.LogError ("Invalid target!");
            destination = target;
            lineRender.SetPosition(1, target.position);
            if (origin)
                distance = Vector3.Distance (origin.position, destination.position);
        }

        public void Reset()
        {
            lineRender.SetPosition (0, Vector3.zero);
            lineRender.SetPosition(1, Vector3.zero);
            SetOrigin (transform);
            SetTarget (transform);
        } 

        private float RandomWidthOffset()
        {
            return Random.Range(randomWithOffsetMin, randomWithOffsetMax);
        }

        void Update()
        {
            if (origin && destination)
            {
                lineRender.SetPosition(0, origin.position);
                lineRender.SetPosition(1, destination.position);
            }
        }
    }
}