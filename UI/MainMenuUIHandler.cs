using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenuUIHandler : MonoBehaviour
{
    public static MainMenuUIHandler instance;
    private string sessionName;
    [Header("Panels")]
    public GameObject playerDetailsPanel;
    public GameObject sessionBrowserPanel;
    public GameObject createSessionPanel;
    public GameObject cantGetDataDialog;
    public GameObject dialogConfirmFile;
    public GameObject dialogLoadingSession;
    public GameObject dialogConfirmJoinSession;
    public GameObject dialogNetworkError;
    public GameObject about;
    [Header("Player settings")]
    public TMP_Text playerNameDisplay;
    string playerName;
    [Header("New work session")]
    public TMP_InputField sessionNameInputField;
    public bool autoNameSession = false;

    private MagicLeapInputs mlInputs;
    private MagicLeapInputs.ControllerActions controllerActions;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        mlInputs = new MagicLeapInputs();
        mlInputs.Enable();
        controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);
        controllerActions.Menu.performed += HandleOnPress;
        //if (PlayerPrefs.HasKey("PlayerNickname"))
        //    playerName = PlayerPrefs.GetString("PlayerNickname");
        //else
        {
            playerName = "Worker" + DateTimeToUnixTimestamp(System.DateTime.Now);
            PlayerPrefs.SetString("PlayerNickname", playerName);
            PlayerPrefs.Save();
        }
        playerNameDisplay.text = playerName;
        GameManager.instance.playerNickName = playerName;
       
    }


    void HideAllPanels()
    {
        playerDetailsPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
        createSessionPanel.SetActive(false);
    }

    public void OnFindGameClicked()
    {
        

        GameManager.instance.playerNickName = playerName;

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobby();

        HideAllPanels();

        sessionBrowserPanel.gameObject.SetActive(true);
        FindObjectOfType<WorkSessionListUI>(true).OnLookingForGameSessions();
    }

    public void OnCreateNewGameClicked()
    {
        HideAllPanels();
        createSessionPanel.SetActive(true);
        if (autoNameSession)
        {
            sessionNameInputField.text = "Session" + DateTimeToUnixTimestamp(System.DateTime.Now);
            sessionName = sessionNameInputField.text;
        }
    }
    public string CurrentNameSession {  get { return sessionName; } }
    public void EndEditInputFieldNameSession(string value)
    {
        sessionName = value;
    }
    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionName, "Main");

        HideAllPanels();
    }

    public void OnJoiningServer()
    {
        HideAllPanels();
    }
    public static int DateTimeToUnixTimestamp(System.DateTime dateTime)
    {
        return (int)(System.TimeZoneInfo.ConvertTimeToUtc(dateTime) -
               new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
    }
    public void OnDropDownNumberOfParcitipantsChange(int value)
    {
        GameManager.instance.maxPlayer = value + 1;
    }
    public void ToggleCantGetDataDialog(bool isOn)
    {
        cantGetDataDialog.SetActive(isOn);
    }
    public void ToggleNetworkError(bool isOn)
    {
        dialogNetworkError.SetActive(isOn);
    }
    public void OnTryNetworkError()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }    
    public void OnClickQuit()
    {
        Application.Quit();
    }
    void HandleOnPress(InputAction.CallbackContext obj)
    {
        about.SetActive(!about.activeInHierarchy);
    }

}
