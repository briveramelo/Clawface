using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponLineup : MonoBehaviour
{

    #region Accessors (Public)

    public ModType SelectedWeapon
    {
        get { return selectedWeapon; }
    }

    #endregion

    [SerializeField] private GameObject[] slots;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private ModType selectedWeapon = ModType.Geyser;
    [SerializeField] private float weaponSwapTime = 0.25f;


    public void MoveRight()
    {
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

        selectedWeapon = weapons[1].GetComponent<WeaponTypeComponent>().typeOfMod;

        GameObject[] weaponsCopy = new GameObject[weapons.Length];
        weapons.CopyTo(weaponsCopy, 0);

        for (int i = weapons.Length - 1; i >= 0; i--)
        {
            if (i != weapons.Length - 1)
                weaponsCopy[i + 1] = weapons[i];
            else
                weaponsCopy[0] = weapons[i];
        }


        this.weapons = weaponsCopy;
        
    }
    public void MoveLeft()
    {
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
        GameObject[] weaponsCopy = new GameObject[weapons.Length];
        weapons.CopyTo(weaponsCopy,0);
        for (int i =weapons.Length-1 ; i>=0; i--)
        {
            if (i != 0)
                weaponsCopy[i-1] = weapons[i];
            else
                weaponsCopy[weapons.Length-1] = weapons[0];
        }
        

        this.weapons = weaponsCopy;
        
    }
}
