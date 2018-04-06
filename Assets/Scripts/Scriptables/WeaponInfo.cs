using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Data", menuName = "ScriptableObjects/WeaponInfo", order = 2)]
public class WeaponInfo : ScriptableObject {

    public ModType weaponType = ModType.Blaster;

    public string weaponName = "New Weapon";

    [TextArea]
    public string weaponDescription = "Weapon Description";

    [Range(0f, 1f)]
    public float damageValue;

    [Range(0f, 1f)]
    public float rangeValue;

    [Range(0f, 1f)]
    public float rofValue;

    [Range(0f, 1f)]
    public float difficultyValue;

    public Sprite weaponImage;
}
