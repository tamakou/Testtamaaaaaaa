using ApiCMS.FileCAD;
using ApiCMS.Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FileCADManager : MonoBehaviour
{
    public static FileCADManager instance;

    public Login login;
    public ListFile listFile;
    public string token { get => login.token; }
    public List<ThumbnailInfo> listThumbnails;
    public bool creatListThumnailsSuccess = false;
    public WorkSessionInfo currentSelectInfo;
    private void Awake()
    {
        instance = this;
        login.Send(this, OnLoginCallback);
    }

    private void OnLoginCallback(string token)
    {
        switch (login.requestState)
        {
            case RequestState.failed:
                Debug.Log($"Failed login with token: {token}");
                MainMenuUIHandler.instance.ToggleCantGetDataDialog(true);
                return;
            case RequestState.success:
                Debug.Log($"Success login with token: {token}");
                listFile = new ListFile(token);
                listFile.Send(this, OnGetListFileSuccess);
                FileCADExtension.token = token;
                return;
        }
    }

    private void OnGetListFileSuccess(ApiCMS.FileCAD.Result arg0)
    {
        switch (listFile.requestState)
        {
            case RequestState.failed:
                MainMenuUIHandler.instance.ToggleCantGetDataDialog(true);
                return;
            case RequestState.success:


                return;
        }
    }
    /// <summary>
    /// Get thumbnail by id mrline
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    public void GetThumbnailByID(int id, UnityAction<Sprite> callback)
    {
        var info = listThumbnails.FirstOrDefault(x => x.id == id);
        if (info == null)
        {
            ThumbnailInfo thumnailInfo = new ThumbnailInfo();
            thumnailInfo.id = id;

            Thumbnail thumbnail = new Thumbnail(id, token);

            thumbnail.Get(this, result =>
            {
                switch (thumbnail.requestState)
                {
                    case RequestState.success:
                        thumnailInfo.sprite = result;
                        callback?.Invoke(result);

                        if (listThumbnails.FirstOrDefault(x => x.id == id) == null)
                            listThumbnails.Add(thumnailInfo);

                        break;
                    case RequestState.failed:
                        callback?.Invoke(null);
                        break;
                }
            });
        }
        else
            callback?.Invoke(info.sprite);

    }
    /// <summary>
    /// Get thumbnail by WorkSessionInfo
    /// </summary>
    /// <param name="wsi"></param>
    /// <param name="callback"></param>
    public void GetThumbnailByWSI(WorkSessionInfo wsi, UnityAction<Sprite> callback)
    {
        GetThumbnailByID(wsi.idMRLine, callback);
    }
    /// <summary>
    /// Convert file cad To WorkSessionInfo
    /// </summary>
    /// <returns></returns>
    public WorkSessionInfo ConvertDatumToWSI(Datum datum)
    {
        WorkSessionInfo result = new WorkSessionInfo();
        result.nameMRLine = datum?.file_name;
        result.dateCreated = System.DateTime.Parse(datum.updated_at);
        result.idMRLine = datum.id;
        result.file = datum.file;
        return result;
    }

    /// <summary>
    /// Get WorkSessionInfo from id file cad
    /// </summary>
    /// <returns></returns>
    public WorkSessionInfo WSIfromID(int id)
    {
        return ConvertDatumToWSI(listFile.GetById(id));
    }

    [System.Serializable]
    public class ThumbnailInfo
    {
        public int id = 1;
        public Sprite sprite = null;
        public ThumbnailInfo() { }
    }
}
