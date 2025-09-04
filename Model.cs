using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ApiCMS.FileCAD
{
    [System.Serializable]
    public class Model
    {
        public int id;
        public string token;
        public Sprite thumbnail;

        public RequestState requestState;
        public Model(int id, string token)
        {
            this.id = id;
            this.token = token;
            requestState = RequestState.standby;
        }
        public void Get(MonoBehaviour behaviour, UnityAction<Object> callback)
        {
            requestState = RequestState.waiting;
            behaviour.StartCoroutine(IEGet(callback));
        }
        public UnityWebRequest Request()
        {
            //string link = "https://rmmrda-stg.ntq.solutions/api/file/download/";
            string link = $"{ApiCmsGlobal.CmsUrl}/api/file/download/";
            string url = $"{link}{id}";
            Debug.Log($"Request {url}");
            UnityWebRequest result =  new UnityWebRequest(url);
            result.method = UnityWebRequest.kHttpVerbGET;
            result.SetRequestHeader("Authorization", "Bearer " + token);

            return result;
        }
        IEnumerator IEGet(UnityAction<Object> callback)
        {
            //string link = "https://rmmrda-stg.ntq.solutions/api/file/download/";
            string link = $"{ApiCmsGlobal.CmsUrl}/api/file/download/";
            string url = $"{link}{id}";
            Debug.Log($"Request {url}");
            using (var request = new UnityWebRequest(url))
            {
                request.method = UnityWebRequest.kHttpVerbGET;
                request.SetRequestHeader("Authorization", "Bearer " + token);

                request.downloadHandler = new DownloadHandlerTexture();
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    requestState = RequestState.failed;
                    Debug.LogError(request.error);
                    callback?.Invoke(null);
                }
                else
                {
                    ///
                    requestState = RequestState.success;

                    callback?.Invoke(null);
                }
            }
        }
    }
}
