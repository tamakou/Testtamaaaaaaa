using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogConfirmJoinSession : MonoBehaviour
{
    [SerializeField] Image imgAvatar;
    [SerializeField] Text txtNameWorkSession;
    [SerializeField] TMP_Text txtDateCreated;
    public void SetData(WorkSessionInfo info, Fusion.SessionInfo sessionInfo)
    {
        FileCADManager.instance.GetThumbnailByWSI(info, result =>
        {
            if (result != null)
                imgAvatar.sprite = result;
        });

        txtNameWorkSession.text = sessionInfo.Name;
        if (sessionInfo.Properties.TryGetValue("createdIn", out var createIn))
        {
            txtDateCreated.text = LanguageManager.Instance.GetLocalizedString("txt_createdin") +" " + (string)createIn.PropertyValue;
        }
    }
}
