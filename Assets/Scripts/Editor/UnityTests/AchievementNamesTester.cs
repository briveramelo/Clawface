using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using ModMan;
/// <summary>
/// Provides testing for string field alignment between the achievement manager's scriptable object achievements list and the const string keys in code
/// </summary>
public class AchievementNamesTester {

    /// <summary>
    /// Detects duplicate or missing Achievements where the AchievementNames fields in code counterpart expects 1
    /// </summary>

    [Test]
    public void AchievementNamesTest() {
        List<Achievement> achievements = GameObject.FindObjectOfType<AchievementManager>().AchievementsList;
        List<string> achievementNames = Helpers.GetConstantsOfType<Strings.AchievementNames, string>();
        achievementNames.ForEach(name => {
            int numMatches = achievements.FindAll(chieve => chieve.name == name).Count;
            bool isExactlyOneMatch = numMatches == 1;
            if (!isExactlyOneMatch) {
                Debug.LogError(string.Format("{0} achievements found with the name '{1}', expected 1", numMatches, name));
            }
            Assert.IsTrue(isExactlyOneMatch);
        });
    }

    /// <summary>
    /// </summary>A
    [Test]
    public void AchievementTest() {
        List<Achievement> achievements = GameObject.FindObjectOfType<AchievementManager>().AchievementsList;
        List<string> achievementNames = Helpers.GetConstantsOfType<Strings.AchievementNames, string>();
        achievements.ForEach(achievement => {
            int numMatches = achievementNames.FindAll(name => name == achievement.name).Count;
            bool isExactlyOneMatch = numMatches == 1;
            if (!isExactlyOneMatch) {
                Debug.LogError(string.Format("{0} AchievementNames found with the name '{1}', expected 1", numMatches, achievement.name));
            }
            Assert.IsTrue(isExactlyOneMatch);
        });
    }
}