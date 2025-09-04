using ApiCMS.FileCAD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ApiCMS.FileCAD
{
    [System.Serializable]
    public class ListFile
    {
        public Result result;
        public string token;
        public RequestState requestState;
        public ListFile(string token)
        {
            this.token = token;
        }
        public void Send(MonoBehaviour behaviour, UnityAction<Result> callback)
        {
            requestState = RequestState.waiting;
            behaviour.StartCoroutine(Get(callback));
        }
        IEnumerator Get(UnityAction<Result> callback)
        {
            //var uri = "https://rmmrda-stg.ntq.solutions/api/file/list?search=&page=";
            var uri = $"{ApiCmsGlobal.CmsUrl}/api/file/list?search=&page=";
            Debug.Log($"Request {uri}");
            UnityWebRequest request = UnityWebRequest.Get(uri);
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                requestState = RequestState.failed;
                Debug.LogError(request.error);
                callback?.Invoke(null);
            }
            else
            {
                requestState = RequestState.success;
                result = JsonUtility.FromJson<Result>(request.downloadHandler.text);
                callback?.Invoke(result);
            }
        }
        public Datum GetById(int ID)
        {
            return result.data.Find(x => x.id == ID);
        }
    }
}
