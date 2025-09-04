using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MessageUIHandler : MonoBehaviour
{
    [SerializeField] TMP_Text txtMessage;
    [SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("HideMessage", time);
    }
    void HideMessage()
    {
        this.gameObject.SetActive(false);
    }
    public void SetValueMessage(string str)
    {
        txtMessage.text = str;
    }

}
