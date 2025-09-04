using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ApiCMS.Login
{
    [System.Serializable]
    public class Login
    {
        public string token;
        public Result result;
        public RequestState requestState = RequestState.standby;
        public Login() { }
        public void Send(MonoBehaviour behaviour, UnityAction<string> callback)
        {
            requestState = RequestState.waiting;
            behaviour.StartCoroutine(DoLogin(callback));
        }
        IEnumerator DoLogin(UnityAction<string> callback)
        {
            // var uri = "https://rmmrda-stg.ntq.solutions/api/login";
            var uri = $"{ApiCmsGlobal.CmsUrl}/api/login";
            WWWForm form = new WWWForm();
            form.AddField("email", "admin@ntq-solution.com.vn");
            form.AddField("password", "123456");
            Debug.Log($"Request {uri}");
            UnityWebRequest www = UnityWebRequest.Post(uri, form);

            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);
                requestState = RequestState.failed;
                callback?.Invoke(null);
            }
            else
            {
                requestState = RequestState.success;
                Debug.Log($"Response: {www.downloadHandler.text}");
                result = JsonUtility.FromJson<Result>(www.downloadHandler.text);
                token = result.data.token;
                Debug.Log($"Token: {token}");
                Debug.Log($"User: {result.data.user.name}");
                Debug.Log($"Message: {result.message}");
                callback?.Invoke(token);
            }
        }
    }
    
}
public enum RequestState
{
    standby, waiting, failed, success
}