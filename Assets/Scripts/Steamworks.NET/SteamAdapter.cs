using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class SteamAdapter : Singleton<SteamAdapter> {

    #region Private Fields

    private static LevelData workingLevelDataCopy = null;
    private static string currentWorkingLevelDataFolderPath;
    private static string currentWorkingLevelImagePath;
    private static SteamWorkshop.SubmitItemCallBack onCurrentUpdateCallback;

    #endregion

    #region Public Fields

    public delegate void LevelsLoadedCallback();

    #endregion


    #region Public Interface

    public static void GenerateFileIDAndUpload(string i_dataPath, string i_imgPath, LevelData i_dataToCopy, SteamWorkshop.SubmitItemCallBack onUpdateComplete)
    {
        currentWorkingLevelDataFolderPath = i_dataPath;
        currentWorkingLevelImagePath = i_imgPath;
        workingLevelDataCopy = new LevelData(i_dataToCopy) {
            isDownloaded=true,
            isMadeByThisUser = false
        };
        onCurrentUpdateCallback = onUpdateComplete;

        //TODO Never update an existing file id, just upload a new one
        // ONLY SHOULD UPLOAD FILES MADE BY THIS USER
        if (workingLevelDataCopy.fileID.m_PublishedFileId == 0)
        {
            SteamWorkshop.Instance.CreateNewItem(OnNewFileIDVerified);
        }
        else
        {
            SteamWorkshop.Instance.UpdateItem(workingLevelDataCopy.fileID,
                workingLevelDataCopy.name,
                workingLevelDataCopy.description,
                i_dataPath,
                i_imgPath,
                "",
                onCurrentUpdateCallback);
        }

    }

    /// <summary>
    /// Synchronus call
    /// </summary>
    public static void LoadSteamLevelData(LevelsLoadedCallback i_toCall=null)
    {
        PublishedFileId_t[] items;
        uint numOfItems = 0;
        SteamWorkshop.instance.GetSubscribedItems(out items, out numOfItems);
        string path = "";
        foreach(PublishedFileId_t itemID in items)
        {
            path = SteamWorkshop.Instance.GetDirectoryForSubscription(itemID);
            if(path != null)
            {
                DataPersister.Instance.LoadLevelData(path);
            }
        }

        if (i_toCall!=null) {
            i_toCall();
        }
    }

    #endregion


    #region Private Interface

    private static void OnNewFileIDVerified(PublishedFileId_t fileId)
    {
        if(fileId.m_PublishedFileId != 0)
        {
            SteamWorkshop.Instance.UpdateItem(fileId,
            workingLevelDataCopy.name,
            workingLevelDataCopy.description,
            currentWorkingLevelDataFolderPath,
            currentWorkingLevelImagePath,
            "",
            onCurrentUpdateCallback);
        }
    }

    #endregion
}
