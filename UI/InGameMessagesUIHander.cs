using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMessagesUIHander : MonoBehaviour
{
    [SerializeField] GameObject message1, message2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnGameMessageReceived(string message)
    {
        GameObject newMessage= Instantiate(message1, this.transform);
        newMessage.SetActive(true);
        newMessage.GetComponent<MessageUIHandler>().SetValueMessage(message);
        Debug.Log($"InGameMessagesUIHander {message}");
    }
    public void OnShareMessageReceived()
    {
        GameObject newMessage = Instantiate(message2, this.transform);
        newMessage.SetActive(true);
        Debug.Log($"You received a share from master");
    }
}
