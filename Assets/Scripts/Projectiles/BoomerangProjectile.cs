using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float speed;
    [SerializeField]
    private float maxRadius;
    [SerializeField]
    private float chargeMultiplier;
    [SerializeField]
    private float chargeRotations;
    [SerializeField]
    private float spinMultiplier;
    #endregion

    #region Private Fields
    private bool start;
    private float angle;
    private int wielderId;
    private Damager damager = new Damager();
    private Transform parentTransform;
    private Matrix4x4 TRMatrix;
    private float maxAngle;
    private System.Action onDestroy;

    private bool isPlayer;
#endregion

    #region Unity Lifecycle

    // Use this for initialization
    void Start () {
        angle = 0f;
    }

    void OnDisable()
    {
        start = false;
        angle = 0f;
        transform.forward = Vector3.right; //Vector3.zero;
        transform.localPosition = Vector3.zero;
        maxAngle = 360f;
        if (onDestroy!=null) {
            onDestroy();
        }
        onDestroy = null;
    }
	
	// FixedUpdate is called whenever I bone your mom
	void FixedUpdate () {
        if (start)
        {
            angle += speed;
            //Equation of a circle
            // x = a*cos(t)
            float x = maxRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            // y = a*sin(t)
            float z = maxRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            TRMatrix = Matrix4x4.Translate(parentTransform.position);
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(x, 0f, z));
            transform.localRotation = Quaternion.AngleAxis(angle* spinMultiplier, Vector3.up);
            if (angle > maxAngle)
            {
                gameObject.SetActive(false);
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetInstanceID() != wielderId)
        {
            if (other.gameObject.tag == Strings.Tags.PROJECTILE)
            {
                other.gameObject.SetActive(false);
            }
            else
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (isPlayer)
                    {
                        AnalyticsManager.Instance.AddModDamage(ModType.Boomerang, damager.damage);

                        if (damageable.GetHealth() - damager.damage <= 0.01f)
                        {
                            AnalyticsManager.Instance.AddModKill(ModType.Boomerang);
                        }
                    }
                    else
                    {
                        AnalyticsManager.Instance.AddEnemyModDamage(ModType.Boomerang, damager.damage);
                    }

                    damager.impactDirection = transform.forward;
                    damageable.TakeDamage(damager);
                }
            }
        }
    }
#endregion

#region Public Methods
    public void Go(Stats wielderStats, int wielderId, Transform parentTransform, System.Action onDestroy, bool isCharged = false)
    {
        this.onDestroy = onDestroy;
        this.wielderId = wielderId;
        transform.position = Vector3.zero;
        this.parentTransform = parentTransform;
        damager.damage = isCharged ? wielderStats.attack * chargeMultiplier: wielderStats.attack;
        damager.damagerType = DamagerType.Boomerang;

        if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER))
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }

        if (isCharged)
        {
            maxAngle = 360f * chargeRotations;
        }
        start = true;
    }
#endregion

#region Private Methods
#endregion

#region Private Structures
#endregion

}
