// Single thread

using Siccity.GLTFUtility;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using TextSketchUpFormat;

public class ModelHandled : MonoBehaviour
{
    public static ModelHandled instance;
    public bool isJoinSession = false; //true join session , false is create new session
    event Action<float> onProgress;
    bool loadding = false;
    [SerializeField] Material set_default;
    [SerializeField] GameObject objSkp;
    IEnumerator LoadSKP;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        onProgress += OnProgress;

    }
    public void LoadData()
    {
        if (loadding) return;

        {
            if (System.IO.Path.GetExtension(FileCADManager.instance.currentSelectInfo.file).Contains("glb") || (System.IO.Path.GetExtension(FileCADManager.instance.currentSelectInfo.file).Contains("GLB")))
                StartCoroutine(IEGet());
            else
             if (System.IO.Path.GetExtension(FileCADManager.instance.currentSelectInfo.file).Contains("tskp"))
            {
                if (LoadSKP != null) StopCoroutine(LoadSKP);
                LoadSKP = OnLoadSKP();
                StartCoroutine(LoadSKP);
            }
            else
            {
                MainMenuUIHandler.instance.dialogLoadingSession.GetComponent<DialogLoadingSession>().LoadState = DialogLoadingSession.State.failed;
                Debug.Log(System.IO.Path.GetExtension(FileCADManager.instance.currentSelectInfo.file));
            }
        }
    }
    private IEnumerator OnLoadSKP()
    {
        //var lineSize = 0.001f;
        var lineSize = 0.004f; // 4mm
        loadding = true;
        MainMenuUIHandler.instance.dialogLoadingSession.GetComponent<DialogLoadingSession>().Progress = 0;
        UnityWebRequest result = FileCADExtension.RequestDownLoad(FileCADManager.instance.currentSelectInfo.idMRLine, FileCADManager.instance.token);
        result.downloadHandler = new DownloadHandlerBuffer();
        yield return result.SendWebRequest();
        if (result.result == UnityWebRequest.Result.ConnectionError || result.result == UnityWebRequest.Result.ProtocolError)
        {
            loadding = false;
            MainMenuUIHandler.instance.dialogLoadingSession.GetComponent<DialogLoadingSession>().LoadState = DialogLoadingSession.State.failed;
            Debug.LogError("error" + result.error);

        }
        else
        {
            Debug.Log("Finished loadModel ");
            string results = result.downloadHandler.text;

            objSkp.GetComponent<TextSketchUpFormat.LoadSkp>().LoadSkpFile(
             results,
            Vector3.zero,
            Quaternion.Euler(0f, 180f, 0f),
            "Yellow",
            lineSize,
            OnFinishAsyncSKP);
        }

    }
    IEnumerator IEGet()
    {
        loadding = true;
        UnityWebRequest result = FileCADExtension.RequestDownLoad(FileCADManager.instance.currentSelectInfo.idMRLine, FileCADManager.instance.token);
        result.downloadHandler = new DownloadHandlerBuffer();
        yield return result.SendWebRequest();
        if (result.result == UnityWebRequest.Result.ConnectionError || result.result == UnityWebRequest.Result.ProtocolError)
        {
            loadding = false;
            MainMenuUIHandler.instance.dialogLoadingSession.GetComponent<DialogLoadingSession>().LoadState = DialogLoadingSession.State.failed;
            Debug.LogError("error" + result.error);

        }
        else
        {
            Debug.Log("Finished loadModel ");
            byte[] results = result.downloadHandler.data;
            Importer.ImportGLBAsync(results, new ImportSettings(), OnFinishAsync, onProgress);

        }

    }
    void OnFinishAsyncSKP(GameObject result)
    {
        loadding = false;
        Debug.Log("Finished importing " + result.name);
        MainMenuUIHandler.instance.dialogLoadingSession.GetComponent<DialogLoadingSession>().Progress = 1;
        result.transform.SetParent(GameManager.instance.gameObject.transform);
        Camera[] arCam = result.GetComponentsInChildren<Camera>();
        for (int i = 0; i < arCam.Length; i++)
        {
            arCam[i].gameObject.SetActive(false);
        }
        result.SetActive(false);
        GameManager.instance.currentObject = result;
        if (!isJoinSession)
            MainMenuUIHandler.instance.OnStartNewSessionClicked();
        else
        {
            MainMenuUIHandler.instance.sessionBrowserPanel.GetComponent<WorkSessionListUI>().OnJoinSession();
        }
    }
    void OnFinishAsync(GameObject result, AnimationClip[] animations)
    {
        loadding = false;
        Debug.Log("Finished importing " + result.name);
        MainMenuUIHandler.instance.dialogLoadingSession.GetComponent<DialogLoadingSession>().Progress = 1;
        result.transform.SetParent(GameManager.instance.gameObject.transform);
        Camera[] arCam = result.GetComponentsInChildren<Camera>();
        for (int i = 0; i < arCam.Length; i++)
        {
            arCam[i].gameObject.SetActive(false);
        }
        //MeshRenderer[] arMeshRender = result.GetComponentsInChildren<MeshRenderer>();

        ////set default material
        //if (arMeshRender.Length > 0)
        //{
        //    for (int i = 0; i < arMeshRender.Length; i++)
        //    {
        //        if (arMeshRender[i].material.name.Contains("Default"))
        //        {
        //            arMeshRender[i].material = set_default;
        //        }
        //    }
        //}
        result.SetActive(false);
        GameManager.instance.currentObject = result;
        if (!isJoinSession)
            MainMenuUIHandler.instance.OnStartNewSessionClicked();
        else
        {
            MainMenuUIHandler.instance.sessionBrowserPanel.GetComponent<WorkSessionListUI>().OnJoinSession();
        }


    }
    void OnProgress(float a)
    {
        MainMenuUIHandler.instance.dialogLoadingSession.GetComponent<DialogLoadingSession>().Progress = a;
    }
}