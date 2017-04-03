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
    private Dictionary<string, object> buttonPresses;

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
        foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
        {
            modTimeDictionary.Add(mod.ToString(), 0f);
        }

        buttonPresses = new Dictionary<string, object>();
        buttonPresses.Add("armL", 0f);
        buttonPresses.Add("armR", 0f);
        buttonPresses.Add("legs", 0f);
        buttonPresses.Add("dodge", 0f);
        buttonPresses.Add("swap", 0f);
        buttonPresses.Add("skin", 0f);
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
        Debug.Log("Sending data on application quit!");
        FormatPlayerModsInDictionary(ref quitGameDictionary);
        quitGameDictionary["averageHP"] = (int)(playerHealthTotal / playerHealthTicks);
        quitGameDictionary["currentHP"] = (int)playerStats.GetStat(StatType.Health);
        quitGameDictionary["deaths"] = deaths;
        quitGameDictionary["sessionTimeMinutes"] = totalTime / 60f;

        float totalTimeMinutes = totalTime / 60f;

        // Changes button presses to be average per minute
        buttonPresses["armL"] = (float)buttonPresses["armL"] / totalTimeMinutes;
        buttonPresses["armR"] = (float)buttonPresses["armR"] / totalTimeMinutes;
        buttonPresses["legs"] = (float)buttonPresses["legs"] / totalTimeMinutes;
        buttonPresses["dodge"] = (float)buttonPresses["dodge"] / totalTimeMinutes;
        buttonPresses["swap"] = (float)buttonPresses["swap"] / totalTimeMinutes;
        buttonPresses["skin"] = (float)buttonPresses["skin"] / totalTimeMinutes;


        // Changes mod times to be a percentage of total play time
        foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
        {
            modTimeDictionary[mod.ToString()] = (float)modTimeDictionary[mod.ToString()] / totalTime;
        }

        Analytics.CustomEvent("modTimes", modTimeDictionary);
        Analytics.CustomEvent("quitGame", quitGameDictionary);
        Analytics.CustomEvent("buttonPresses", buttonPresses);
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
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, ButtonMode.DOWN)) {
            buttonPresses["armL"] = (float)buttonPresses["armL"] + 1f;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, ButtonMode.DOWN))
        {
            buttonPresses["armR"] = (float)buttonPresses["armR"] + 1f;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, ButtonMode.DOWN))
        {
            buttonPresses["legs"] = (float)buttonPresses["legs"] + 1f;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN))
        {
            buttonPresses["dodge"] = (float)buttonPresses["dodge"] + 1f;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE, ButtonMode.DOWN))
        {
            buttonPresses["swap"] = (float)buttonPresses["swap"] + 1f;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_SKIN, ButtonMode.DOWN))
        {
            buttonPresses["skin"] = (float)buttonPresses["skin"] + 1f;
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
