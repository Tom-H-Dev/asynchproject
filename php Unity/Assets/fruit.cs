using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class fruit : MonoBehaviour
{
    string url = "http://127.0.0.1/edsa-asyncserver/";

    //I want to commit

    IEnumerator Start()
    {

        NameList request = new NameList();


        string json = JsonUtility.ToJson(request);
        Debug.Log(json);

        List<IMultipartFormSection> formData = new();
        MultipartFormDataSection entery = new MultipartFormDataSection("json", json);

        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            FruitResponse response = JsonUtility.FromJson<FruitResponse>(webRequest.downloadHandler.text);
            Debug.Log(response.fruit[Random.Range(0, response.fruit.Length)]);
        }
    }
}


public class NameList
{
    public string action = "get_fruit";
}

public class FruitResponse
{
    public string[] fruit;
}