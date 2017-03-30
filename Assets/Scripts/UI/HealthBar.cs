/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.Assertions;

public class HealthBar : Singleton<HealthBar> {

    private Camera mCamera;
    
    protected HealthBar() { }

    //// Unity Inspector Fields
    [SerializeField]
    private Transform mask, bar;

    //// HealthBar Public Interface
    public void SetHealth(float health)
    {
        Assert.IsTrue(health >= 0.0F && health <= 1.0F);
        mask.localScale = new Vector3(health, 1.0F, 1.0F);
        bar.localScale = new Vector3(health == 0 ? 0 : 1 / health, 1.0F, 1.0F);
    }

    protected override void Awake()
    {
        base.Awake();
        mCamera = Camera.main;
 
    }

    private void Update()
    {
        //billboard to main camera
        transform.LookAt(transform.position + mCamera.transform.rotation * Vector3.forward,
                    mCamera.transform.rotation * Vector3.up);
    }
}
