// TemporaryGib.cs
// Author: Aaron

using System.Collections;

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Behavior to control temporary gib objects.
    /// </summary>
    public sealed class TemporaryGib : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Lifetime of this gib (seconds).
        /// </summary>
        [Tooltip("Lifetime of this gib (seconds).")]
        [SerializeField] float lifetime = 5f;
        
        /// <summary>
        /// Time taken to fade (seconds).
        /// </summary>
        [Tooltip("Time taken to fade (seconds).")]
        [SerializeField] float fadeTime = 2f;
        
        /// <summary>
        /// Distance to paint blood (UU).
        /// </summary>
        [Tooltip("Distance to paint blood (UU).")]
        [SerializeField] float bloodPaintDist = 1f;
        
        /// <summary>
        /// Show blood smear?
        /// </summary>
        [Tooltip("Show blood smear?")]
        [SerializeField] bool doSmear = false;


        #endregion
        #region Private Fields

        const float BLOOD_SMEAR_Y_OFFSET = 0.01f;

        /// <summary>
        /// Maximum number of points in smear trail.
        /// </summary>
        const int MAX_SMEAR_POINTS = 10;

        /// <summary>
        /// Rotation of smear points.
        /// </summary>
        static Quaternion BLOOD_SMEAR_ROTATION = 
            Quaternion.Euler(90f, 0f, 0f);

        /// <summary>
        /// Layer mask of the gorund.
        /// </summary>
        int groundLayerMask = 1 << (int)Layers.Ground;

        /// <summary>
        /// Y-position of smear points.
        /// </summary>
        float smearY;

        /// <summary>
        /// List of smear points.
        /// </summary>
        Vector3[] smearPoints;

        /// <summary>
        /// Current lifetime (seconds).
        /// </summary>
        float t;

        /// <summary>
        /// Current gib color.
        /// </summary>
        Color color;

        /// <summary>
        /// Original gib color.
        /// </summary>
        Color originalColor;

        /// <summary>
        /// Gib material.
        /// </summary>
        Material material;

        /// <summary>
        /// Rigidbody attached to this gib.
        /// </summary>
        Rigidbody rb;

        /// <summary>
        /// LineRenderer attached to this gib.
        /// </summary>
        LineRenderer lineRenderer;

        /// <summary>
        /// Last point of smearing.
        /// </summary>
        Vector3 lastSmearPoint;

        #endregion
        #region Unity Lifecycle

        private void Awake()
        {
            material = GetComponentInChildren<MeshRenderer>(true).material;
            originalColor = material.color;
            color = material.color;

            if (doSmear)
            {
                lineRenderer = GetComponentInChildren<LineRenderer>(true);

                lineRenderer.enabled = false;
                smearPoints = new Vector3[MAX_SMEAR_POINTS];
                lineRenderer.positionCount = 0;
                lineRenderer.SetPositions(smearPoints);
            }

            StartCoroutine(LiveAndFade());
        }

        private void OnEnable()
        {
            color = originalColor;
            material.color = color;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (doSmear)
            {

                if (collision.collider.gameObject.layer == (int)Layers.Ground)
                {
                    var point = collision.contacts[0].point;
                    var smearPos = transform.position;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, Vector3.down, out hit, 
                        Mathf.Infinity, groundLayerMask))
                    {
                        smearY = hit.point.y;
                    }

                    smearPos.y = smearY + BLOOD_SMEAR_Y_OFFSET;
                    lineRenderer.enabled = true;
                    lastSmearPoint = smearPos;
                    smearPoints[0] = lastSmearPoint;
                    smearPoints[1] = lastSmearPoint;
                    lineRenderer.positionCount = 2;
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (doSmear)
            {
                if (collision.collider.gameObject.layer == (int)Layers.Ground)
                {
                    var point = collision.contacts[0].point;
                    var smearPos = transform.position;
                    smearPos.y = point.y + BLOOD_SMEAR_Y_OFFSET;
                    var dist = Vector3.Distance(smearPos, lastSmearPoint);
                    if (dist >= bloodPaintDist)
                    {
                        if (lineRenderer.positionCount < MAX_SMEAR_POINTS - 1)
                            lineRenderer.positionCount++;

                        var start = Mathf.Min(lineRenderer.positionCount, 
                            MAX_SMEAR_POINTS) - 1;

                        for (int i = start; i > 1; i--)
                        {
                            smearPoints[i] = smearPoints[i - 1];
                        }

                        smearPoints[1] = smearPos;
                        lastSmearPoint = smearPos;

                    } 
                    
                    else
                    {
                        smearPoints[0] = smearPos;
                    }

                    lineRenderer.SetPositions(smearPoints);
                    lineRenderer.transform.rotation = BLOOD_SMEAR_ROTATION;
                }
            }
        }

        private IEnumerator LiveAndFade()
        {
            yield return new WaitForSecondsRealtime(lifetime);

            t = fadeTime;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                color.a = t / fadeTime;
                material.color = color;
                if (doSmear)
                {
                    var color = lineRenderer.material.color;
                    this.color.a = color.a;
                    lineRenderer.material.color = color;
                }
                yield return null;
            }

            Destroy(gameObject);
            yield return null;
        }

        #endregion
    }
}