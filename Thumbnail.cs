using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ApiCMS.FileCAD
{
    [System.Serializable]
    public class Thumbnail
    {
        public int id;
        public string token;
        public Sprite thumbnail;

        public RequestState requestState;
        public Thumbnail(int id, string token)
        {
            this.id = id;
            this.token = token;
            requestState = RequestState.standby;
        }
        public void Get(MonoBehaviour behaviour, UnityAction<Sprite> callback)
        {
            requestState = RequestState.waiting;
            behaviour.StartCoroutine(IEGet(callback));
        }
        IEnumerator IEGet(UnityAction<Sprite> callback)
        {
            //string link = "https://rmmrda-stg.ntq.solutions/api/thumbnail/download/";
            string link = $"{ApiCmsGlobal.CmsUrl}/api/thumbnail/download/";
            string url = $"{link}{id}";
            Debug.Log($"Request {url}");
            using (var request = new UnityWebRequest(url))
            {
                request.method = UnityWebRequest.kHttpVerbGET;
                request.SetRequestHeader("Authorization", "Bearer " + token);
                request.downloadHandler = new DownloadHandlerTexture();
                yield return request.SendWebRequest();

                // Check for errors
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    requestState = RequestState.failed;
                    callback?.Invoke(null);
                }
                else
                {
                    requestState = RequestState.success;
                    var texture = DownloadHandlerTexture.GetContent(request);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    thumbnail = sprite;
                    callback?.Invoke(sprite);
                }
            }
        }
    }
}
