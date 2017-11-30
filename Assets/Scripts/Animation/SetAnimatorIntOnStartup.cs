// SetAnimatorIntOnStartup.cs
// Author: Aaron

using UnityEngine;

public sealed class SetAnimatorIntOnStartup : MonoBehaviour
{
    #region Serialized Unity Inspector Fields

    [SerializeField] Animator animator;
    [SerializeField] string parameterName;
    [SerializeField] int parameterValue;

    #endregion
    #region Unity Lifecycle

    private void Awake()
    {
        animator.SetInteger (parameterName, parameterValue);
    }

    #endregion
}
