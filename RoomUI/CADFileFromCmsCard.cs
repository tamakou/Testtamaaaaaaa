using ApiCMS.FileCAD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CADFileFromCmsCard : MonoBehaviour
{
    [SerializeField] Image imgAvatar;
    [SerializeField] Text txtNameMRLine;
    [SerializeField] TMP_Text txtDateCreated;
    [SerializeField] Button btnJoin;
    [SerializeField] GameObject btnHidden;
    public WorkSessionInfo myInfo;
    public void SetData(WorkSessionInfo info)
    {
        btnJoin.gameObject.SetActive(false);
        btnHidden.SetActive(true);
        myInfo = info;// new WorkSessionInfo(info);
        //info.GetThumnail(this, FileCADManager.instance.login.token, result =>
        //{
        //    if (result != null)
        //        imgAvatar.sprite = result;
        //});
        FileCADManager.instance.GetThumbnailByWSI(info, result =>
        {
            if (result != null)
            {
                imgAvatar.sprite = result;
                btnJoin.gameObject.SetActive(true);
                btnHidden.SetActive(false);
            }

        });

        txtNameMRLine.text = info.nameMRLine;
        txtDateCreated.text = LanguageManager.Instance.GetLocalizedString("txt_createdin") + " "+  info.dateCreated.ToString("dd/MM/yyyy");
        btnJoin.onClick.AddListener(Join);
    }
    public void Join()
    {
        FileCADManager.instance.currentSelectInfo = myInfo;
        MainMenuUIHandler.instance.dialogConfirmFile.SetActive(true);
        MainMenuUIHandler.instance.dialogConfirmFile.GetComponent<ConfirmFileDialog>().SetData(myInfo);

    }
}
