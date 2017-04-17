using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager> {

    #region Serialized
    [SerializeField]
    private List<int> expNeeded;

    [SerializeField]
    private int currentLevel;

    [SerializeField]
    private int maxLevel;

    [SerializeField]
    private int currentEXP;

    [SerializeField]
    private int availablePoints;

    [SerializeField]
    private List<int> healthUpgrades;

    [SerializeField]
    private int pointsInvestedInHealth;
    #endregion

    #region Unity Lifetime

    // Use this for initialization
    void Start () {
        maxLevel = expNeeded.Count;
	}

    #endregion

    #region Public Methods
    public void AddEXP(int exp)
    {
        
        if (currentLevel >= maxLevel)
        {
            currentEXP = 0;
        }
        else
        {
            currentEXP += exp;

            if (currentEXP >= expNeeded[currentLevel])
            {
                currentEXP -= expNeeded[currentLevel];
                currentLevel++;
                availablePoints++;

                if (currentLevel >= maxLevel) currentEXP = 0;
            }
        }
    }

    public int GetCurrentAvailablePoints()
    {
        return availablePoints;
    }

    public int GetPointsInvestedInHealth()
    {
        return pointsInvestedInHealth;
    }

    public int GetHealthLevel()
    {
        return healthUpgrades[pointsInvestedInHealth];
    }
    #endregion
}
