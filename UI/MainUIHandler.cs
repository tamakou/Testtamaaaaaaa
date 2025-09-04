using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIHandler : MonoBehaviour
{
    public static MainUIHandler _Instance;
    public InGameMessagesUIHander inGameMessagesUIHander;
    [SerializeField] GameObject objNetworkError;
    private void Awake()
    {
        _Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        objNetworkError.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClickButtonQuit()
    {
        ShutdownAll();
    }
    public void ShutdownAll()
    {
        foreach (var runner in NetworkRunner.Instances.ToList())
        {
            if (runner != null && runner.IsRunning)
            {
                runner.Shutdown();
            }
        }

        SceneManager.LoadScene("MainMenu");
        // Destroy our DontDestroyOnLoad objects to finish the reset
        Destroy(GameManager.instance.currentObject);
    }
    public void OnNetworkError(bool isOn)
    {
        objNetworkError.SetActive(isOn);
    }
    public void OnTryNetworkError()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(GameManager.instance.currentObject);
    }    
}
