using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DebugUI : MonoBehaviour
{
    public TMP_Text _text;
    public TMP_Text fps;
    public static DebugUI instance;
    void Start()
    {
        instance = this;
    }
    private void Update()
    {
        fps.text = "FPS : " + (int)(1 / Time.deltaTime);
    }
    public void SendMessgeToUI(string context)
    {
        string curr = _text.text;
        if(curr.Length > 0)
        {
            curr += System.Environment.NewLine + context;
        }
        else
        {
            curr = context;
        }
        _text.text = curr;
    }

    public void Clear()
    {
        _text.text = "";
    }
}
