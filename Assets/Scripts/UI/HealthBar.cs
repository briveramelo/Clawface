/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.Assertions;

public class HealthBar : MonoBehaviour {

    //// Unity Inspector Fields
    [SerializeField]
    private Transform bar;

    //// HealthBar Public Interface
    public void SetHealth(float health)
    {
        Assert.IsTrue(health >= 0.0F && health <= 1.0F);
        bar.localScale = new Vector3(health, 1.0F, 1.0F);
    }
}
