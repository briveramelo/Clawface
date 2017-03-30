using UnityEngine;
using UnityEngine.Assertions;

public class BillboardComponent : MonoBehaviour {

    #region Unity Inspector Fields
    [SerializeField]
    private Vector3 forward = Vector3.forward;
    [SerializeField]
    private Vector3 up = Vector3.up;
    [SerializeField]
    private float updatePeriod = 0.0F;
    #endregion

    #region Private Fields
    private float deltaTime = 0.0F;
    #endregion

    #region Unity Lifecycle Functions
    private void Start()
    {
        Assert.IsTrue(forward.sqrMagnitude != 0);
        Assert.IsTrue(up.sqrMagnitude != 0);
        forward.Normalize();
        up.Normalize();
        Billboard();
    }

	private void Update () {
        if (updatePeriod < 0.0F) // Don't update again
            return;

        deltaTime += Time.deltaTime;
        if (deltaTime >= updatePeriod)
        {
            Billboard();
            deltaTime = 0.0F;
        }
	}
    #endregion

    #region Private Methods
    private void Billboard()
    {
        // Billboard this component to main camera
        gameObject.transform.LookAt(gameObject.transform.position +
            Camera.main.transform.rotation * forward,
            Camera.main.transform.rotation * up);
    }

    #endregion
}
