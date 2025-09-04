using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkSessionListUI : MonoBehaviour
{
    public List<WorkSessionCard> listCard;

    [SerializeField] Transform rootCard;
    [SerializeField] WorkSessionCard cardPrefabs;

    [SerializeField] RectTransform rectBg;
    [SerializeField] RectTransform rectContentScroll;
    [SerializeField] GameObject nextBack;
    [SerializeField] GameObject nullImg;

    [SerializeField] float step;
    [SerializeField] Text textStatus;

    // DialogConfirmJoinSession
    [SerializeField] RectTransform[] rectBgDialog;

    SessionInfo session;

    private void Start()
    {
        OnListCardChangeCount();
    }
    private void OnDisable()
    {
        ModelHandled.instance.isJoinSession =false;
    }
    public void ClearList()
    {
        if (rectContentScroll.transform.childCount > 0)
        {
            //Delete all children of the vertical layout group
            foreach (Transform child in rectContentScroll.transform)
            {
                Destroy(child.gameObject);
            }
        }
        nullImg.SetActive(false);
        listCard = new List<WorkSessionCard>();

        //Hide the status message
        // textStatus.gameObject.SetActive(false);
    }
    public void AddToList(SessionInfo sessionInfo, int idMRLine)
    {
        AddCard(FileCADManager.instance.WSIfromID(idMRLine), sessionInfo);
        Debug.Log("Found idMrLine " + idMRLine);
    }
    public void AddCard(WorkSessionInfo cardInfo, SessionInfo sessionInfo)
    {
        WorkSessionCard newCard = Instantiate(cardPrefabs, rootCard);
        newCard.SetData(cardInfo, sessionInfo);
        listCard.Add(newCard);
        OnListCardChangeCount();
        newCard.OnJoinSession += ShowDialogConfirmJoinSession;
    }
    void ShowDialogConfirmJoinSession(WorkSessionInfo cardInfo,SessionInfo sessionInfo)
    {
        FileCADManager.instance.currentSelectInfo = cardInfo;
        MainMenuUIHandler.instance.dialogConfirmJoinSession.SetActive(true);
        MainMenuUIHandler.instance.dialogConfirmJoinSession.GetComponent<DialogConfirmJoinSession>().SetData(cardInfo,sessionInfo);
        this.session = sessionInfo;
        ModelHandled.instance.isJoinSession = true;
    } 
    
   public void OnJoinSession()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.JoinGame(session);

        MainMenuUIHandler mainMenuUIHandler = FindObjectOfType<MainMenuUIHandler>();
        mainMenuUIHandler.OnJoiningServer();
    }

    public void Next()
    {
        rectContentScroll.position -= new Vector3(step, 0, 0);
    }
    public void Previor()
    {
        rectContentScroll.position += new Vector3(step, 0, 0);
    }
    void OnListCardChangeCount()
    {
        if (listCard.Count < 3)
        {
            UnScale();
        }
        else
        {
            Scale();
        }
    }
    void Scale()
    {
        rectBg.sizeDelta = new Vector2(960, 560);
        nextBack.SetActive(true);
        foreach (RectTransform rec in rectBgDialog)
        rec.sizeDelta = new Vector2(960, 560);
    }
    void UnScale()
    {
        rectBg.sizeDelta = new Vector2(584, 560);
        nextBack.SetActive(false);
        foreach (RectTransform rec in rectBgDialog)
        rec.sizeDelta= new Vector2(584, 560);
    }
    public void OnNoSessionsFound()
    {
        ClearList();
        nullImg.SetActive(true);
        // textStatus.text = "No game session found";
        // textStatus.gameObject.SetActive(true);
    }
    public void OnLookingForGameSessions()
    {
        ClearList();

        // textStatus.text = "Looking for game sessions";
        //  textStatus.gameObject.SetActive(true);
    }
}
