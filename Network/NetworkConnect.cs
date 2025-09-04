using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NetworkConnect : MonoBehaviour
{
    public UnityEvent OnNetworkError;
    public UnityEvent OnNetworkConnect;
    public float waitTime = 1f;
    float timer;
    bool check = true;
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://google.com"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                action(false);
            }
            else
            {
                action(true);
            }
        }
    }

    void Update()
    {
        if (check)
        {
            timer += Time.deltaTime;
            if (timer > waitTime)
            {
                StartCoroutine(checkInternetConnection((isConnected) =>
                {
                    // handle connection status here
                    if (isConnected)
                    {
                        OnNetworkConnect?.Invoke();
                    }
                    else
                    {
                        OnNetworkError?.Invoke();
                        check = false;
                    }
                }));
                timer = 0f;
            }
        }
    }

}
