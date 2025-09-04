using System.Collections;
using System.Collections.Generic;
using ApiCMS;
using UnityEngine;
using ApiCMS.FileCAD;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class FileCADExtension
{
    public static string token;
    public static void GetThumbnail(this WorkSessionInfo wsi,MonoBehaviour behaviour, string token, UnityAction<Sprite> callback)
    {
        Thumbnail thumbnail = new Thumbnail(wsi.idMRLine, token);
        thumbnail.Get(behaviour, callback);
    }
    public static UnityWebRequest RequestDownLoad(int id, string token)
    {
        //string link = "https://rmmrda-stg.ntq.solutions/api/file/download/";
        string link = $"{ApiCmsGlobal.CmsUrl}/api/file/download/";
        string url = $"{link}{id}";
        Debug.Log($"Request {url}");
        Debug.Log($"Token: {token}");
        UnityWebRequest result = new UnityWebRequest(url);
        result.method = UnityWebRequest.kHttpVerbGET;
        result.SetRequestHeader("Authorization", "Bearer " + token);
        return result;
    }
}
