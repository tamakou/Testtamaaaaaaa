using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class SessionInfoListUIItem : MonoBehaviour
{
    public TextMeshProUGUI sessionNameText;
    public TextMeshProUGUI playerCountText;
    public Button joinButton;

    SessionInfo sessionInfo;

    //Events
    public event Action<SessionInfo> OnJoinSession;

    public void SetInformation(SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;

        sessionNameText.text = sessionInfo.Name;
        playerCountText.text = $"{sessionInfo.PlayerCount.ToString()}/{sessionInfo.MaxPlayers.ToString()}";

        bool isJoinButtonActive = true;

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
            isJoinButtonActive = false;

        joinButton.gameObject.SetActive(isJoinButtonActive);
    }

    public void OnClick()
    {
        //Invoke the join session event
        OnJoinSession?.Invoke(sessionInfo);
    }
}
