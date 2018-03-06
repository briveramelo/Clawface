using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponLineup : MonoBehaviour
{
    #region Public Fields (Accessors)

    public WeaponSelectGlowController GlowControl { get { return glowControl; } }
    public ModType GetModType { get { return selectedWeapon; } }

    #endregion  

    #region Serialized Unity Fields

    [SerializeField] private GameObject[] slots;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Sprite[] weaponGraphs;
    [SerializeField] private ModType selectedWeapon = ModType.Geyser;
    [SerializeField] private float weaponSwapTime = 0.25f;
    [SerializeField] private Sprite selectedRightSprite, selectedLeftSprite;
    [SerializeField] private Sprite unselectedRightSprite, unselectedLeftSprite;
    [SerializeField] private Button moveLeftButton, moveRightButton;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color initialColor = Color.blue;
    [SerializeField] private WeaponSelectGlowController glowControl;
    [SerializeField] private ButtonSFXEffects buttonFX;
    [SerializeField] private Image graphImage;

    #endregion

    #region Private References

    #endregion

    
    
    public void SelectArrows() {
        moveRightButton.image.sprite = selectedRightSprite;
        moveLeftButton.image.sprite = selectedLeftSprite;
        moveRightButton.image.color = selectedColor;
        moveLeftButton.image.color = selectedColor;
        buttonFX.PlayButtonConfirm();
    }

    public void UnselectArrows()
    {
        moveRightButton.image.sprite = unselectedRightSprite;
        moveLeftButton.image.sprite = unselectedLeftSprite;
        moveRightButton.image.color = initialColor;
        moveLeftButton.image.color = initialColor;
    }
    public void MoveRight()
    {

        //make associated arrow "Blink" to show movement
        moveRightButton.image.sprite = selectedRightSprite;
        moveLeftButton.image.sprite = unselectedLeftSprite;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (i != weapons.Length-1)
            {
                LeanTween.move(weapons[i], slots[i + 1].transform, weaponSwapTime);
            }
            else
            {
                LeanTween.move(weapons[i], slots[0].transform, weaponSwapTime);
            }

        }
        
        LeanTween.scale(weapons[0], new Vector3(0.5f, 0.5f, 0.5f), weaponSwapTime);
        LeanTween.scale(weapons[1], Vector3.one, weaponSwapTime);
        LeanTween.scale(weapons[2], new Vector3(0.5f, 0.5f, 0.5f), weaponSwapTime);
        LeanTween.scale(weapons[3], Vector3.zero, weaponSwapTime);
        LeanTween.scale(weapons[4], Vector3.zero, weaponSwapTime);

        selectedWeapon = weapons[1].GetComponent<WeaponTypeComponent>().typeOfMod;
        graphImage.sprite = weaponGraphs[1];

        GameObject[] weaponsCopy = new GameObject[weapons.Length];
        Sprite[] graphsCopy = new Sprite[weapons.Length];
        weapons.CopyTo(weaponsCopy, 0);
        weaponGraphs.CopyTo(graphsCopy, 0);

        for (int i = weapons.Length - 1; i >= 0; i--)
        {
            if (i != weapons.Length - 1)
            {
                weaponsCopy[i + 1] = weapons[i];
                graphsCopy[i + 1] = weaponGraphs[i];
            }
            else
            {
                weaponsCopy[0] = weapons[i];
                graphsCopy[0] = weaponGraphs[i];
            }
        }


        this.weapons = weaponsCopy;
        this.weaponGraphs = graphsCopy;
        
    }
    public void MoveLeft()
    {

        //make associated arrow "Blink" to show movement
        moveRightButton.image.sprite = unselectedRightSprite;
        moveLeftButton.image.sprite = selectedLeftSprite;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (i != 0)
            {
                LeanTween.move(weapons[i], slots[i - 1].transform, weaponSwapTime);
            }
            else
            {
                LeanTween.move(weapons[i], slots[slots.Length-1].transform, weaponSwapTime);
            }

        }

        LeanTween.scale(weapons[0], Vector3.zero, weaponSwapTime);
        LeanTween.scale(weapons[1], Vector3.zero, weaponSwapTime);
        LeanTween.scale(weapons[2], new Vector3(0.5f, 0.5f, 0.5f), weaponSwapTime);
        LeanTween.scale(weapons[3], Vector3.one, weaponSwapTime);
        LeanTween.scale(weapons[4], new Vector3(0.5f, 0.5f, 0.5f), weaponSwapTime);
        
        selectedWeapon = weapons[3].GetComponent<WeaponTypeComponent>().typeOfMod;
        graphImage.sprite = weaponGraphs[3];

        GameObject[] weaponsCopy = new GameObject[weapons.Length];
        Sprite[] graphsCopy = new Sprite[weapons.Length];
        weapons.CopyTo(weaponsCopy,0);
        weaponGraphs.CopyTo(graphsCopy, 0);

        for (int i =weapons.Length-1 ; i>=0; i--)
        {
            if (i != 0)
            {
                weaponsCopy[i - 1] = weapons[i];
                graphsCopy[i - 1] = weaponGraphs[i];
            }
            else
            {
                weaponsCopy[weapons.Length - 1] = weapons[0];
                graphsCopy[weaponGraphs.Length - 1] = weaponGraphs[0];
            }
        }
        

        this.weapons = weaponsCopy;
        this.weaponGraphs = graphsCopy;
        
    }
}
