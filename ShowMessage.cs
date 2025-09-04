using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowMessage : MonoBehaviour
{
    public Text _text;
    public static ShowMessage instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    public void SendMessageText(string messsage)
    {
        _text.text = messsage;
    }
    public void Clear()
    {
        _text.text = "";
    }
}
