using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : Singleton<UpgradeManager> {

    #region Serialized
    [SerializeField]
    private GameObject canvasUI;

    [SerializeField]
    private Text availablePointsText;

    [SerializeField]
    private Text healthLabel;

    [SerializeField]
    private Image healthPip1;

    [SerializeField]
    private Image healthPip2;

    [SerializeField]
    private Image healthPip3;

    [SerializeField]
    private Image healthPip4;

    [SerializeField]
    private Image healthPip5;

    [SerializeField]
    private Image cursor;

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

    [SerializeField]
    private UpgradeMenuSelection currentSelection;

    #region Privates
    private bool canMoveCursor;
    private PlayerStatsManager playerStatsManager;
    private Stats playerStats;
    #endregion

    #region Unity Lifetime

    // Use this for initialization
    void Start () {
        maxLevel = expNeeded.Count;
	}

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {
            AddEXP(10000);
        }


        if (InputManager.Instance.QueryAction(Strings.Input.Actions.PAUSE, ButtonMode.DOWN))
        {
            if (canvasUI.activeSelf)
            {
                canvasUI.SetActive(false);

            }
            else
            {
                currentSelection = UpgradeMenuSelection.HP0;
                canvasUI.SetActive(true);
                canMoveCursor = true;
            }
        }

        
        if (canvasUI.activeSelf)
        {
            availablePointsText.text = availablePoints.ToString();

            HandleCursorInput();
            
            if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, ButtonMode.DOWN))
            {
                if (CheckIfEnoughPointsToUnlockCurrentSelection())
                {
                    UnlockCurrentSelection();
                }
            }


            UpdateHealthPipColor();
            UpdateCursorPosition();
            
            
        }
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

            while (currentLevel < maxLevel && currentEXP >= expNeeded[currentLevel])
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

    public void SetPlayerStatsManager(PlayerStatsManager stats)
    {
        playerStatsManager = stats;
    }

    public void SetPlayerStats(Stats stats)
    {
        playerStats = stats;
    }
    #endregion

    #region Private Methods
    private void UpdateHealthPipColor()
    {
        switch (pointsInvestedInHealth)
        {
            case 5:
                healthPip5.color = Color.green;
                healthPip4.color = Color.green;
                healthPip3.color = Color.green;
                healthPip2.color = Color.green;
                healthPip1.color = Color.green;
                break;
            case 4:
                healthPip5.color = Color.grey;
                healthPip4.color = Color.green;
                healthPip3.color = Color.green;
                healthPip2.color = Color.green;
                healthPip1.color = Color.green;
                break;
            case 3:
                healthPip5.color = Color.grey;
                healthPip4.color = Color.grey;
                healthPip3.color = Color.green;
                healthPip2.color = Color.green;
                healthPip1.color = Color.green;
                break;
            case 2:
                healthPip5.color = Color.grey;
                healthPip4.color = Color.grey;
                healthPip3.color = Color.grey;
                healthPip2.color = Color.green;
                healthPip1.color = Color.green;
                break;
            case 1:
                healthPip5.color = Color.grey;
                healthPip4.color = Color.grey;
                healthPip3.color = Color.grey;
                healthPip2.color = Color.grey;
                healthPip1.color = Color.green;
                break;
            case 0:
                healthPip5.color = Color.grey;
                healthPip4.color = Color.grey;
                healthPip3.color = Color.grey;
                healthPip2.color = Color.grey;
                healthPip1.color = Color.grey;
                break;
        }

    }

    private void HandleCursorInput()
    {
        Vector2 input = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);

        if (input.magnitude < 0.05)
        {
            canMoveCursor = true;
        }

        if (canMoveCursor)
        {
            switch (currentSelection)
            {
                case UpgradeMenuSelection.HP0:
                    if (input.x > 0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP1;
                        canMoveCursor = false;
                    }
                    break;
                case UpgradeMenuSelection.HP1:
                    if (input.x > 0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP2;
                        canMoveCursor = false;
                    }
                    else if (input.x < -0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP0;
                        canMoveCursor = false;
                    }
                    break;
                case UpgradeMenuSelection.HP2:
                    if (input.x > 0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP3;
                        canMoveCursor = false;
                    }
                    else if (input.x < -0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP1;
                        canMoveCursor = false;
                    }
                    break;
                case UpgradeMenuSelection.HP3:
                    if (input.x > 0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP4;
                        canMoveCursor = false;
                    }
                    else if (input.x < -0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP2;
                        canMoveCursor = false;
                    }
                    break;
                case UpgradeMenuSelection.HP4:
                    if (input.x > 0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP5;
                        canMoveCursor = false;
                    }
                    else if (input.x < -0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP3;
                        canMoveCursor = false;
                    }
                    break;
                case UpgradeMenuSelection.HP5:
                    if (input.x < -0.05)
                    {
                        currentSelection = UpgradeMenuSelection.HP4;
                        canMoveCursor = false;
                    }
                    break;
            }
        }
    }

    private bool CheckIfEnoughPointsToUnlockCurrentSelection()
    {
        switch (currentSelection)
        {
            case UpgradeMenuSelection.HP1:
                if (availablePoints >= 1) return true;
                break;
            case UpgradeMenuSelection.HP2:
                if (pointsInvestedInHealth == 1 && availablePoints >= 1) return true;
                if (pointsInvestedInHealth == 0 && availablePoints >= 2) return true;
                break;
            case UpgradeMenuSelection.HP3:
                if (pointsInvestedInHealth == 2 && availablePoints >= 1) return true;
                if (pointsInvestedInHealth == 1 && availablePoints >= 2) return true;
                if (pointsInvestedInHealth == 0 && availablePoints >= 3) return true;
                break;
            case UpgradeMenuSelection.HP4:
                if (pointsInvestedInHealth == 3 && availablePoints >= 1) return true;
                if (pointsInvestedInHealth == 2 && availablePoints >= 2) return true;
                if (pointsInvestedInHealth == 1 && availablePoints >= 3) return true;
                if (pointsInvestedInHealth == 0 && availablePoints >= 4) return true;
                break;
            case UpgradeMenuSelection.HP5:
                if (pointsInvestedInHealth == 4 && availablePoints >= 1) return true;
                if (pointsInvestedInHealth == 3 && availablePoints >= 2) return true;
                if (pointsInvestedInHealth == 2 && availablePoints >= 3) return true;
                if (pointsInvestedInHealth == 1 && availablePoints >= 4) return true;
                if (pointsInvestedInHealth == 0 && availablePoints >= 5) return true;
                break;
        }
        return false;
    }

    private void UpdateCursorPosition()
    {
        switch (currentSelection)
        {
            case UpgradeMenuSelection.HP0:
                cursor.rectTransform.position = new Vector3(healthLabel.rectTransform.position.x, healthLabel.rectTransform.position.y + 20);
                break;
            case UpgradeMenuSelection.HP1:
                cursor.rectTransform.position = new Vector3(healthPip1.rectTransform.position.x, healthPip1.rectTransform.position.y + 20);
                break;
            case UpgradeMenuSelection.HP2:
                cursor.rectTransform.position = new Vector3(healthPip2.rectTransform.position.x, healthPip2.rectTransform.position.y + 20);
                break;
            case UpgradeMenuSelection.HP3:
                cursor.rectTransform.position = new Vector3(healthPip3.rectTransform.position.x, healthPip3.rectTransform.position.y + 20);
                break;
            case UpgradeMenuSelection.HP4:
                cursor.rectTransform.position = new Vector3(healthPip4.rectTransform.position.x, healthPip4.rectTransform.position.y + 20);
                break;
            case UpgradeMenuSelection.HP5:
                cursor.rectTransform.position = new Vector3(healthPip5.rectTransform.position.x, healthPip5.rectTransform.position.y + 20);
                break;
        }
    }

    private void UnlockCurrentSelection()
    {
        switch (currentSelection)
        {
            case UpgradeMenuSelection.HP1:
                if (availablePoints >= 1)
                {
                    pointsInvestedInHealth = 1;
                    availablePoints -= 1;
                    currentSelection = UpgradeMenuSelection.HP0;
                }
                break;
            case UpgradeMenuSelection.HP2:
                if (pointsInvestedInHealth + availablePoints >= 2)
                {
                    availablePoints -= (2 - pointsInvestedInHealth);
                    pointsInvestedInHealth = 2;
                    currentSelection = UpgradeMenuSelection.HP0;
                }
                break;
            case UpgradeMenuSelection.HP3:
                if (pointsInvestedInHealth + availablePoints >= 3)
                {
                    availablePoints -= (3 - pointsInvestedInHealth);
                    pointsInvestedInHealth = 3;
                    currentSelection = UpgradeMenuSelection.HP0;
                }
                break;
            case UpgradeMenuSelection.HP4:
                if (pointsInvestedInHealth + availablePoints >= 4)
                {
                    availablePoints -= (4 - pointsInvestedInHealth);
                    pointsInvestedInHealth = 4;
                    currentSelection = UpgradeMenuSelection.HP0;
                }
                break;
            case UpgradeMenuSelection.HP5:
                if (pointsInvestedInHealth + availablePoints >= 5)
                {
                    availablePoints -= (5 - pointsInvestedInHealth);
                    pointsInvestedInHealth = 5;
                    currentSelection = UpgradeMenuSelection.HP0;
                }
                break;
        }

        if (playerStats != null)
        {
            playerStats.SetMaxHealth(GetHealthLevel());
        }

        if (playerStatsManager != null)
        {
            playerStatsManager.UpdateMaxHealth();
        }
    }

    #endregion

    #region Private Structures
    private enum UpgradeMenuSelection
    {
        HP0 = 0,
        HP1 = 1,
        HP2 = 2,
        HP3 = 3,
        HP4 = 4,
        HP5 = 5,
        CONFIRM = 6,
        REMOVE = 7
    }

    #endregion
}
