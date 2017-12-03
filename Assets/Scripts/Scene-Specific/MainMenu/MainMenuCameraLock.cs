using UnityEngine;

[ExecuteInEditMode]
public class MainMenuCameraLock : MonoBehaviour {

    #region Fields (Unity Serialization)

    [SerializeField]
    private Camera cameraToLock;

    [SerializeField]
    private float rightSideLockingCoord;

    #endregion

    #region Inteface (Unity Lifecycle)

    private void Update()
    {
        if (cameraToLock != null)
        {
            Vector3 worldSpaceBound = cameraToLock.ViewportToWorldPoint(new Vector3(1, 0, 0));
            Vector3 position = transform.position;
            float difference = rightSideLockingCoord - worldSpaceBound.x;
            position.x += difference;
            transform.position = position;
        }
    } 

    #endregion
}
