using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class SteamWorkshop : Singleton<SteamWorkshop> {

    public delegate void CreateItemCallBack(PublishedFileId_t fileId);
    public delegate void SubmitItemCallBack(bool result);

    private CallResult<CreateItemResult_t> createItemResult;
    private CallResult<SubmitItemUpdateResult_t> submitItemResult;
    private CreateItemCallBack createItemCallBack;
    private SubmitItemCallBack submitItemCallBack;
    private UGCUpdateHandle_t updateHandle;

    #region unity lifecycle    
    protected override void OnEnable()
    {
        base.OnEnable();
        createItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
        submitItemResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
    }
    #endregion

    #region public methods
    //Creates new item and returns ID in the callback
    //Always check if the returned id is valid to check if the call was successful (valid ids are not 0)
    public bool CreateNewItem(CreateItemCallBack callback)
    {
        bool result = false;
        if (SteamManager.Initialized)
        {
            SteamAPICall_t apiCall = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            if (createItemResult.IsActive())
            {
                createItemResult.Cancel();
                createItemResult.Dispose();
            }
            createItemResult.Set(apiCall);
            createItemCallBack = callback;
            result = true;
        }
        return result;
    }


    //Used to update a workshop item, the callback function is called after the update is completed. The return value will indicate success or failure.
    public bool UpdateItem(PublishedFileId_t fileId, string title, string description, string contentPath, string previewImagePath, string changeNote, SubmitItemCallBack submitItemCallBack)
    {
        bool result = false;
        if(SteamManager.Initialized && fileId.m_PublishedFileId != 0)
        {
            updateHandle =  SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), fileId);
            result = true;
            result = result && SteamUGC.SetItemTitle(updateHandle, title);

            if (result)
            {
                result = result && SteamUGC.SetItemDescription(updateHandle, description);
            }

            if (result)
            {
                result = result && SteamUGC.SetItemContent(updateHandle, contentPath);
            }

            if (result)
            {
                result = result && SteamUGC.SetItemPreview(updateHandle, previewImagePath);
            }

            if (result)
            {
                SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(updateHandle, changeNote);
                if (submitItemResult.IsActive())
                {
                    submitItemResult.Cancel();
                    submitItemResult.Dispose();
                }
                submitItemResult.Set(apiCall);
                this.submitItemCallBack = submitItemCallBack;
            }
        }
        return result;
    }

    public void GetUpdateProgress(out ulong bytesProcessed, out ulong bytesTotal)
    {
        EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(updateHandle, out bytesProcessed, out bytesTotal);
    }

    public void GetSubscribedItems(out PublishedFileId_t[] subscribedItems, out uint numberOfSubscribedItems)
    {
        numberOfSubscribedItems = SteamUGC.GetNumSubscribedItems();
        subscribedItems = new PublishedFileId_t[numberOfSubscribedItems];
        SteamUGC.GetSubscribedItems(subscribedItems, numberOfSubscribedItems);
    }

    //If string returned is null, it means that the folder does not exist
    public string GetDirectoryForSubscription(PublishedFileId_t itemId)
    {
        ulong sizeOnDisk;
        string folder;
        uint folderSize = 1024;
        uint timeStamp;        
        if(!SteamUGC.GetItemInstallInfo(itemId, out sizeOnDisk, out folder, folderSize, out timeStamp))
        {
            folder = null;
        }
        return folder;
    }
    #endregion

    #region private methods
    private void OnCreateItemResult(CreateItemResult_t param, bool bIOFailure)
    {
        if(!bIOFailure && param.m_eResult != 0)
        {
            createItemCallBack(param.m_nPublishedFileId);
        }
        else
        {
            Debug.LogError("Failed to create workshop item!");
            PublishedFileId_t fileId = PublishedFileId_t.Invalid;
            createItemCallBack(fileId);
        }
    }

    private void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t param, bool bIOFailure)
    {
        if (!bIOFailure && param.m_eResult != 0)
        {
            submitItemCallBack(true);
        }
        else
        {
            Debug.LogError("Failed to submit workshop item!");
            submitItemCallBack(false);
        }
    }
    #endregion

}
