/**
 *  @author Cornelia Schultz
 */
 
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    #region Unity Inspector Fields
    // Unity Objects
    [SerializeField]
    private Image overlay = null;
    [SerializeField]
    private Sprite[] glitchSprites = {};

    // Configuration Variables
    [SerializeField]
    private float glitchSeconds = 1.0F;
    [SerializeField]
    private int glitchesPerSecond = 5;
    #endregion

    #region Private Fields
    private System.Random random = new System.Random();
    private bool glitchInProgress = false;
    #endregion

    #region Public Interface
    public void DoDamageEffect()
    {
        StartCoroutine(GlitchEffect());
    }
    #endregion

    #region Private Interface
    private IEnumerator GlitchEffect()
    {
        if (glitchInProgress)
            yield break;
        else
            glitchInProgress = true;

        overlay.color = new Color(1.0F, 1.0F, 1.0F, 1.0F);

        float elapsedTime = 0.0F;
        float totalTime = 0.0F;
        while (totalTime < glitchSeconds)
        {
            if (elapsedTime == 0.0F)
            {
                overlay.sprite = GetRandomSprite();
            }
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            totalTime += Time.fixedDeltaTime;

            if (elapsedTime >= 1.0F / glitchesPerSecond)
            {
                elapsedTime = 0.0F;
            }
        }

        overlay.sprite = null;
        overlay.color = new Color(1.0F, 1.0F, 1.0F, 0.0F);
        glitchInProgress = false;
    }

    private Sprite GetRandomSprite()
    {
        return glitchSprites[random.Next(glitchSprites.Length)];
    }
    #endregion
}
