using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogLoadingSession : MonoBehaviour
{
    public enum State
    { 
        prepareLoad,
        loading,
        success,
        failed
    }
    [SerializeField] State loadState = State.prepareLoad;
    public State LoadState
    {
        set
        {
            loadState = value;

            switch (loadState)
            {
                case State.failed:
                    objLoading.SetActive(false);
                    objLoadingFailed.SetActive(true);
                    break;
            }
        }
        get => loadState;
    }

    [SerializeField] Image imgAvatar;
    [SerializeField] Text txtNameWorkSession;
    [SerializeField] TMP_Text txtDateCreated;
    [SerializeField] Text txtProgress;
    [SerializeField] Image imgFillProgress;
    [SerializeField] GameObject objLoading;
    [SerializeField] GameObject objLoadingFailed;

    [SerializeField] private float progress;
    
    public float Progress
    {
        set
        {
            float result = Mathf.Clamp(value, 0, 1);
            progress = result;
            txtProgress.text = $"{Mathf.RoundToInt(result * 100)}%";
            imgFillProgress.fillAmount = progress;
        }
        get => Mathf.Clamp(progress, 0, 1);
    }

    public void SetData(WorkSessionInfo info)
    {
        FileCADManager.instance.GetThumbnailByWSI(info, result =>
        {
            if (result != null)
                imgAvatar.sprite = result;
        });

        txtDateCreated.text = LanguageManager.Instance.GetLocalizedString("txt_createdin") + " " +System.DateTime.Now.ToString("dd/MM/yyyy");
        txtNameWorkSession.text = MainMenuUIHandler.instance.CurrentNameSession;
    }
    public void Retry()
    {
        objLoading.SetActive(true);
        objLoadingFailed.SetActive(false);
        ModelHandled.instance.LoadData();
    }    

    private void OnEnable()
    {
        SetData(FileCADManager.instance.WSIfromID(FileCADManager.instance.currentSelectInfo.idMRLine));
        ModelHandled.instance.LoadData();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            LoadState = State.failed;
    }
}
