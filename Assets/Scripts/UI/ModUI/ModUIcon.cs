/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.UI;

public class ModUIcon : MonoBehaviour {

    //// Unity Inspector Fields
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Sprite defaultHeadSprite, defaultArmLeftSprite,
        defaultArmRightSprite, defaultLegsSprite;

    [SerializeField]
    private Image highlightRing;

    //// Internal State
    private ModUIProperties activeProperties;
    private ModSpot currentSpot;
    private ModUIState currentState;
	
    //// ModUIcon Public Interface
    public void Attach(ModUIProperties properties)
    {
        activeProperties = properties;
        UpdateAppearance();
    }

    public void Detach()
    {
        activeProperties = null;
        UpdateAppearance();
    }

    public void Relocate(ModSpot spot)
    {
        currentSpot = spot;
        UpdateLocation();
    }

    public void Apply(ModUIState state)
    {
        currentState = state;
        UpdateAppearance();
    }

    //// Private Functionality
    private void UpdateAppearance()
    {
        // Assign a ModIcon to display
        if (activeProperties != null)
        {
            icon.sprite = activeProperties.sprite;
        }
        else
        {
            switch (currentSpot)
            {
                case ModSpot.Head:
                    icon.sprite = defaultHeadSprite;
                    break;
                case ModSpot.Arm_L:
                    icon.sprite = defaultArmLeftSprite;
                    break;
                case ModSpot.Arm_R:
                    icon.sprite = defaultArmRightSprite;
                    break;
                case ModSpot.Legs:
                    icon.sprite = defaultHeadSprite;
                    break;
                default:
                    throw new System.SystemException("IMPOSSIBRU!!!");
            }
        }

        // Apply the ModUIState
        currentState.ApplyHiglightRing(ref highlightRing);
    }
    private void UpdateLocation()
    {
        animator.SetTrigger(GetTrigger(currentSpot));
    }
    private string GetTrigger(ModSpot spot)
    {
        switch (spot)
        {
            case ModSpot.Head:
                return "RELOCATE_TO_HEAD";
            case ModSpot.Arm_L:
                return "RELOCATE_TO_ARM_LEFT";
            case ModSpot.Arm_R:
                return "RELOCATE_TO_ARM_RIGHT";
            case ModSpot.Legs:
                return "RELOCATE_TO_LEGS";
            default:
                throw new System.SystemException("IMPOSSIBRU!!!");
        }
    }
}
