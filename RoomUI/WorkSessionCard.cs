using ApiCMS.FileCAD;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WorkSessionCard : MonoBehaviour
{
    [SerializeField] Image imgAvatar;
    [SerializeField] Text txtNameSession;
    [SerializeField] Text txtNameMRLine;
    [SerializeField] TMP_Text txtParticipant;
    [SerializeField] Text txtNameSpace;
    [SerializeField] TMP_Text txtDateCreated;
    [SerializeField] Button btnJoin,btnJoinHidden;
    SessionInfo sessionInfo;
    WorkSessionInfo workSessionInfo;
    //Events
    public event Action<WorkSessionInfo,SessionInfo> OnJoinSession;

    public void SetData(WorkSessionInfo info, SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;
        this.workSessionInfo = info;
        FileCADManager.instance.GetThumbnailByWSI(info, result =>
        {
            if (result != null)
                imgAvatar.sprite = result;
        });

        txtNameSession.text = sessionInfo.Name;
        txtNameMRLine.text = info.nameMRLine;
        txtParticipant.text = LanguageManager.Instance.GetLocalizedString("txt_participant") + ": " + $"{sessionInfo.PlayerCount.ToString()}/{sessionInfo.MaxPlayers.ToString()}";
        txtNameSpace.text = info.nameSpace;
        // Check for a specific Custom Property
        if (sessionInfo.Properties.TryGetValue("createdIn", out var createIn))
        {
            txtDateCreated.text = LanguageManager.Instance.GetLocalizedString("txt_createdin") + " " + (string)createIn.PropertyValue;
        }
        btnJoin.onClick.AddListener(Join);
        bool isJoinButtonActive = true;

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
            isJoinButtonActive = false;

        btnJoin.gameObject.SetActive(isJoinButtonActive);
        btnJoinHidden.gameObject.SetActive(!isJoinButtonActive);
    }
    public void Join()
    {
        //Invoke the join session event
        OnJoinSession?.Invoke(workSessionInfo,sessionInfo);
    }
}
[Serializable]
public class WorkSessionInfo
{
    public Sprite avatar;
    public string nameMRLine;
    public string file;
    public string nameSpace;

    public DateTime dateCreated;
    public int idMRLine;
    public WorkSessionInfo() { }
    public WorkSessionInfo(Sprite avatar, string nameSession, string nameMRLine, string participant, string nameSpace, DateTime dateCreated, int idMRLine)
    {
        this.avatar = avatar;

        this.nameMRLine = nameMRLine;

        this.nameSpace = nameSpace;
        this.dateCreated = dateCreated;
        this.idMRLine = idMRLine;
    }
    public WorkSessionInfo(WorkSessionInfo _base)
    {
        this.avatar = _base.avatar;

        this.nameMRLine = _base.nameMRLine;

        this.nameSpace = _base.nameSpace;
        this.dateCreated = _base.dateCreated;
        this.idMRLine = _base.idMRLine;
    }
}