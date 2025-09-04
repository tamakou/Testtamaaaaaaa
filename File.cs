using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ApiCMS.FileCAD
{
    public class File
    {
        public int id;
        public string token;
        public RequestState requestState;
        public Datum result;
        public File(int id, string token)
        {
            this.id = id;
            this.token = token;
        }
        public void Send(MonoBehaviour behaviour, UnityAction<Datum> callback)
        {
            requestState = RequestState.waiting;
            behaviour.StartCoroutine(Get(callback));
        }
        IEnumerator Get(UnityAction<Datum> callback)
        {
            //var uri = "https://rmmrda-stg.ntq.solutions/api/file/" + id.ToString();
            var uri = $"{ApiCmsGlobal.CmsUrl}/api/file/" + id.ToString();
            Debug.Log($"Request {uri}");
            UnityWebRequest request = UnityWebRequest.Get(uri);
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                requestState = RequestState.failed;
                callback?.Invoke(null);
            }
            else
            {
                requestState = RequestState.success;
                Debug.Log($"Response: {request.downloadHandler.text}");
                result = JsonUtility.FromJson<Datum>(request.downloadHandler.text);
                callback?.Invoke(result);
            }
        }
    } 
}
