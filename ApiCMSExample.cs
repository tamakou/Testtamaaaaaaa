using ApiCMS.FileCAD;
using ApiCMS.Login;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ApiCMSExample : MonoBehaviour
{
    [SerializeField] Login loginManager;
    [SerializeField] ListFile getListFile;

    public List<ThumnailInfo> thumnailss;

    private void Start()
    {
        loginManager = new Login();
        loginManager.Send(this, OnLoginSuccess);
    }
    void OnLoginSuccess(string token)
    {
        getListFile = new ListFile(token);
        getListFile.Send(this, OnGetListFileSuccess);
    }
    void OnGetListFileSuccess(ApiCMS.FileCAD.Result result)
    {
        foreach (var item in result.data)
        {
            item.GetThumnail(this, loginManager.token, result =>
            {
                thumnailss.Add(new ThumnailInfo(item.id, result));
            });
        }
    }
}
[System.Serializable]
public class ThumnailInfo
{
    public int id;
    public Sprite thumbnail;
    public ThumnailInfo(int id, Sprite thumbnail)
    {
        this.id = id;
        this.thumbnail = thumbnail;
    }
}