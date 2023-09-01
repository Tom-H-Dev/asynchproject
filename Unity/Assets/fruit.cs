using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class fruit : MonoBehaviour
{
    string url = "http://127.0.0.1/edsa-asyncserver/";



    IEnumerator Start()
    {

        NameList request = new NameList();
        request.fruit = new string[] { "peer", "appel", "aardbei" };
        string json = JsonUtility.ToJson(request);
        Debug.Log(json);

        List<IMultipartFormSection> formData = new();
        MultipartFormDataSection entery = new MultipartFormDataSection("json", json);

        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
        }
    }
}


public class NameList
{
    public string[] fruit;
}