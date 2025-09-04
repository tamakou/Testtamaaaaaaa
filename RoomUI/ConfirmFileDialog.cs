using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmFileDialog : MonoBehaviour
{
    [SerializeField] Image imgAvatar;
    [SerializeField] Text txtNameLine;
    [SerializeField] TMP_Text txtDateCreated;
    public void SetData(WorkSessionInfo info)
    {
        FileCADManager.instance.GetThumbnailByWSI(info, result =>
        {
            if (result != null)
                imgAvatar.sprite = result;
        });

        txtNameLine.text = info.nameMRLine;
        txtDateCreated.text = LanguageManager.Instance.GetLocalizedString("txt_createdin") + " "+info.dateCreated.ToString("dd/MM/yyyy");
        GameManager.instance.idObject = info.idMRLine;
    }
}
