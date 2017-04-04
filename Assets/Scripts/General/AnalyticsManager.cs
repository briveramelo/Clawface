// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : Singleton<AnalyticsManager>
{

    #region Public fields


    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Stats playerStats;

    [SerializeField]
    private ModManager manager;

    [SerializeField]
    private float timeBetweenTicks;
    #endregion



    #region Private Fields
    private Dictionary<string, object> playerDeathDictionary;
    private Dictionary<string, object> quitGameDictionary;
    private Dictionary<string, object> modTimeDictionary;
    private Dictionary<string, object> buttonPressesDictionary;
    private Dictionary<string, object> modDamageDictionary;
    private Dictionary<string, object> enemyModDamageDictionary;
    private Dictionary<string, object> modArmLPressesDictionary;
    private Dictionary<string, object> modArmRPressesDictionary;
    private Dictionary<string, object> modLegsPressesDictionary;

    private float timer;
    private float playerHealthTotal;
    private int playerHealthTicks;

    private float totalTime;

    private int deaths;

    #endregion


    #region Unity Lifecycle


    // Use this for initialization
    void Start()
    {
        quitGameDictionary = new Dictionary<string, object>();
        quitGameDictionary.Add("armL", "null");
        quitGameDictionary.Add("armR", "null");
        quitGameDictionary.Add("legs", "null");
        quitGameDictionary.Add("averageHP", 0);
        quitGameDictionary.Add("currentHP", 0);
        quitGameDictionary.Add("deaths", 0);

        playerDeathDictionary = new Dictionary<string, object>();
        playerDeathDictionary.Add("armL", "null");
        playerDeathDictionary.Add("armR", "null");
        playerDeathDictionary.Add("legs", "null");
        playerDeathDictionary.Add("averageHP", 0);

        modTimeDictionary = new Dictionary<string, object>();
        modDamageDictionary = new Dictionary<string, object>();
        enemyModDamageDictionary = new Dictionary<string, object>();
        modArmLPressesDictionary = new Dictionary<string, object>();
        modArmRPressesDictionary = new Dictionary<string, object>();
        modLegsPressesDictionary = new Dictionary<string, object>();

        foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
        {
            modTimeDictionary.Add(mod.ToString(), 0f);
            modDamageDictionary.Add(mod.ToString(), 0f);
            enemyModDamageDictionary.Add(mod.ToString(), 0f);
            modArmLPressesDictionary.Add(mod.ToString(), 0f);
            modArmRPressesDictionary.Add(mod.ToString(), 0f);
            modLegsPressesDictionary.Add(mod.ToString(), 0f);
        }

        buttonPressesDictionary = new Dictionary<string, object>();
        buttonPressesDictionary.Add("armL", 0f);
        buttonPressesDictionary.Add("armR", 0f);
        buttonPressesDictionary.Add("legs", 0f);
        buttonPressesDictionary.Add("dodge", 0f);
        buttonPressesDictionary.Add("swap", 0f);
        buttonPressesDictionary.Add("skin", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        totalTime += Time.deltaTime;

        if (timer >= timeBetweenTicks)
        {
            playerHealthTicks++;
            timer = 0;

            UpdatePlayerHealthTotal();
            UpdateModTimes();
        }

        UpdateButtonPresses();
    }



    private void OnApplicationQuit()
    {
        float totalTimeMinutes = totalTime / 60f;

        FormatPlayerModsInDictionary(ref quitGameDictionary);
        quitGameDictionary["averageHP"] = (int)(playerHealthTotal / playerHealthTicks);
        quitGameDictionary["currentHP"] = (int)playerStats.GetStat(StatType.Health);
        quitGameDictionary["deaths"] = deaths;
        quitGameDictionary["sessionTimeMinutes"] = totalTimeMinutes;

        // Changes button presses to be average per minute
        buttonPressesDictionary["armL"] = (float)buttonPressesDictionary["armL"] / totalTimeMinutes;
        buttonPressesDictionary["armR"] = (float)buttonPressesDictionary["armR"] / totalTimeMinutes;
        buttonPressesDictionary["legs"] = (float)buttonPressesDictionary["legs"] / totalTimeMinutes;
        buttonPressesDictionary["dodge"] = (float)buttonPressesDictionary["dodge"] / totalTimeMinutes;
        buttonPressesDictionary["swap"] = (float)buttonPressesDictionary["swap"] / totalTimeMinutes;
        buttonPressesDictionary["skin"] = (float)buttonPressesDictionary["skin"] / totalTimeMinutes;


        // Changes mod times to be a percentage of total play time, and mod presses to be average per minute
        foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
        {
            modTimeDictionary[mod.ToString()] = (float)modTimeDictionary[mod.ToString()] / totalTime;
            modArmLPressesDictionary[mod.ToString()] = (float)modArmLPressesDictionary[mod.ToString()] / totalTimeMinutes;
            modArmRPressesDictionary[mod.ToString()] = (float)modArmRPressesDictionary[mod.ToString()] / totalTimeMinutes;
            modLegsPressesDictionary[mod.ToString()] = (float)modLegsPressesDictionary[mod.ToString()] / totalTimeMinutes;
        }

        Analytics.CustomEvent("modTimes", modTimeDictionary);
        Analytics.CustomEvent("quitGame", quitGameDictionary);
        Analytics.CustomEvent("buttonPresses", buttonPressesDictionary);
        Analytics.CustomEvent("modDamage", modDamageDictionary);
        Analytics.CustomEvent("enemyModDamage", enemyModDamageDictionary);
        Analytics.CustomEvent("armLButtonPresses", modArmLPressesDictionary);
        Analytics.CustomEvent("armRButtonPresses", modArmRPressesDictionary);
        Analytics.CustomEvent("legsButtonPresses", modLegsPressesDictionary);
    }
    #endregion

    #region Public Methods
    public void FormatPlayerModsInDictionary(ref Dictionary<string, object> dict)
    {
        Dictionary<ModSpot, ModManager.ModSocket> modSocketDictionary = manager.GetModSpotDictionary();

        if (modSocketDictionary[ModSpot.ArmL].mod != null)
        {
            dict["armL"] = modSocketDictionary[ModSpot.ArmL].mod.getModType().ToString();
        }
        else
        {
            dict["armL"] = "null";
        }

        if (modSocketDictionary[ModSpot.ArmR].mod != null)
        {
            dict["armR"] = modSocketDictionary[ModSpot.ArmR].mod.getModType().ToString();
        }
        else
        {
            dict["armR"] = "null";
        }

        if (modSocketDictionary[ModSpot.Legs].mod != null)
        {
            dict["legs"] = modSocketDictionary[ModSpot.Legs].mod.getModType().ToString();
        }
        else
        {
            dict["legs"] = "null";
        }

    }

    public void PlayerDeath()
    {
        deaths++;
        playerDeathDictionary["averageHP"] = (int)(playerHealthTotal / playerHealthTicks);
        FormatPlayerModsInDictionary(ref playerDeathDictionary);

        Analytics.CustomEvent("playerDeath", playerDeathDictionary);


        playerHealthTotal = playerStats.GetStat(StatType.Health);
        playerHealthTicks = 1;

    }

    public void SetModManager(ModManager manager)
    {
        this.manager = manager;


    }

    public void AddModDamage(ModType modType, float damage)
    {
        modDamageDictionary[modType.ToString()] = (float) modDamageDictionary[modType.ToString()] + damage;
    }

    public void AddEnemyModDamage(ModType modType, float damage)
    {
        enemyModDamageDictionary[modType.ToString()] = (float)enemyModDamageDictionary[modType.ToString()] + damage;
    }

    public void SetPlayerStats(Stats stats)
    {
        playerStats = stats;
        playerHealthTotal = stats.GetStat(StatType.Health);
        playerHealthTicks = 1;
    }

    #endregion

    #region Private Methods
    private void UpdatePlayerHealthTotal()
    {
        playerHealthTotal += playerStats.GetStat(StatType.Health);
    }

    private void UpdateButtonPresses()
    {
        Dictionary<ModSpot, ModManager.ModSocket> modSocketDictionary = manager.GetModSpotDictionary();

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, ButtonMode.DOWN))
        {
            buttonPressesDictionary["armL"] = (float)buttonPressesDictionary["armL"] + 1f;

            if (modSocketDictionary[ModSpot.ArmL].mod != null)
            {
                string modTypeToString = modSocketDictionary[ModSpot.ArmL].mod.getModType().ToString();
                modArmLPressesDictionary[modTypeToString] = (float)modArmLPressesDictionary[modTypeToString] + 1f;
            }
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, ButtonMode.DOWN))
        {
            buttonPressesDictionary["armR"] = (float)buttonPressesDictionary["armR"] + 1f;

            if (modSocketDictionary[ModSpot.ArmR].mod != null)
            {
                string modTypeToString = modSocketDictionary[ModSpot.ArmR].mod.getModType().ToString();
                modArmRPressesDictionary[modTypeToString] = (float)modArmRPressesDictionary[modTypeToString] + 1f;
            }
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, ButtonMode.DOWN))
        {
            buttonPressesDictionary["legs"] = (float)buttonPressesDictionary["legs"] + 1f;

            if (modSocketDictionary[ModSpot.Legs].mod != null)
            {
                string modTypeToString = modSocketDictionary[ModSpot.Legs].mod.getModType().ToString();
                modLegsPressesDictionary[modTypeToString] = (float)modLegsPressesDictionary[modTypeToString] + 1f;
            }
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN))
        {
            buttonPressesDictionary["dodge"] = (float)buttonPressesDictionary["dodge"] + 1f;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE, ButtonMode.DOWN))
        {
            buttonPressesDictionary["swap"] = (float)buttonPressesDictionary["swap"] + 1f;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_SKIN, ButtonMode.DOWN))
        {
            buttonPressesDictionary["skin"] = (float)buttonPressesDictionary["skin"] + 1f;
        }
    }

    private void UpdateModTimes()
    {
        Dictionary<ModSpot, ModManager.ModSocket> modSpotDictionary = manager.GetModSpotDictionary();
        List<ModType> equippedMods = new List<ModType>();

        if (modSpotDictionary[ModSpot.ArmL].mod != null)
        {
            equippedMods.Add(modSpotDictionary[ModSpot.ArmL].mod.getModType());
        }

        if (modSpotDictionary[ModSpot.ArmR].mod != null)
        {
            if (!equippedMods.Contains(modSpotDictionary[ModSpot.ArmR].mod.getModType()))
            {
                equippedMods.Add(modSpotDictionary[ModSpot.ArmR].mod.getModType());
            }
        }

        if (modSpotDictionary[ModSpot.Legs].mod != null)
        {
            if (!equippedMods.Contains(modSpotDictionary[ModSpot.Legs].mod.getModType()))
            {
                equippedMods.Add(modSpotDictionary[ModSpot.Legs].mod.getModType());
            }
        }

        foreach (ModType m in equippedMods)
        {
            modTimeDictionary[m.ToString()] = (float) modTimeDictionary[m.ToString()] + timeBetweenTicks;
        }
    }
    #endregion

    #region Private Structures
    #endregion

}
