using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class SteamAdapter : MonoBehaviour {

    #region Private Fields

    private static LevelData workingLevelData = null;
    private static string currentWorkingLevelDataFolderPath;
    private static string currentWorkingLevelImagePath;

    #endregion

    public static void GenerateFileIDAndUpload(string i_dataPath, string i_imgPath, LevelData i_data)
    {
        workingLevelData = i_data;
        currentWorkingLevelDataFolderPath = i_dataPath;
        currentWorkingLevelImagePath = i_imgPath;

        if(workingLevelData.fileID.m_PublishedFileId == 0)
        {
            SteamWorkshop.Instance.CreateNewItem(OnFileIDVerified);
        }

    }

    private static void OnFileIDVerified(PublishedFileId_t fileId)
    {
        if(fileId.m_PublishedFileId != 0)
        {
            SteamWorkshop.Instance.UpdateItem(fileId,
            workingLevelData.name,
            workingLevelData.description,
            currentWorkingLevelDataFolderPath,
            currentWorkingLevelImagePath,
            "",
            OnSubmitItem);
        }
    }

    private static void OnSubmitItem(bool result)
    {
        print("Submit of " + workingLevelData.name + ".dat returned " + result);
    }
}
