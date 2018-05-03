﻿#region Using Statements

using UnityEngine;
using UnityEngine.Assertions;

#endregion

/// <summary>
/// A class used for applying an EightiesFilter shader as a post processing effect.
/// </summary>
[ExecuteInEditMode]
public class EightiesFilterOperation : MonoBehaviour
{
	#region Fields (Unity Serialization)

    /// <summary>
    /// The ScriptableObject containing the Shader Data.
    /// </summary>
    [SerializeField]
    private EightiesFilterData data;

	#endregion

	#region Fields (Private)

    /// <summary>
    /// The material to use when post-processing an image.
    /// </summary>
    private Material material;

	#endregion

	#region Interface (Unity Lifecycle)

    /// <summary>
    /// Called when this MonoBehaviour is started.
    /// </summary>
    private void Start()
    {
        Assert.IsNotNull(data);
        material = new Material(data.Shader);
    }

    /// <summary>
    /// Called when this MonoBehaviour is attached to a GameObject with a Camera
    /// and it is about to be rendered.
    /// </summary>
    /// <param name="source">The source render texture.</param>
    /// <param name="destination">The destination render texture.</param>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (SettingsManager.Instance.EightiesFilter)
        {
            Assert.IsNotNull(material);
            data.SetValues(material);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            // Perform a passthrough if nothing to do.
            Graphics.Blit(source, destination);
        }
    }

    /// <summary>
    /// Called when this MonoBehaviour is edited in the Inspector.
    /// </summary>
    private void OnValidate()
    {
        Start();
    }

	#endregion
}