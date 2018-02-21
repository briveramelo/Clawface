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

    public Sprite weaponGraph;
}
