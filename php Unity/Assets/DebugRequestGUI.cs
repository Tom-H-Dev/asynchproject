using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class DebugRequestGUI : MonoBehaviour
{
    string url = "http://127.0.0.1/edsa-asyncserver/";

    private VisualElement root;
    private Button requestButton;
    private Label debugLabel;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        requestButton = root.Q<Button>("Send-Button");
        debugLabel = root.Q<Label>("Output-Label");
        debugLabel.text = "";
        requestButton.RegisterCallback<ClickEvent>(evt =>
        {
            StartCoroutine(RandomFruit());
        });
    }



    //I want to commit

    IEnumerator RandomFruit()
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
            debugLabel.text += response.fruit[Random.Range(0, response.fruit.Length)] + "\n";
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