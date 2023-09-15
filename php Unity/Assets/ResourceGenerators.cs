using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResourceGenerators : MonoBehaviour
{
    private const string url = "http://127.0.0.1/edsa-asyncserver/api.php";

    private float tick = 5;
    private bool isGathering;
    private IEnumerator timer, requestAsync;
    private int _goldIncome, _lumberIncome, _manaIncome;

    private void Start()
    {
        timer = GeneratorTimer(tick);
        StartCoroutine(timer);
    }

    IEnumerator GeneratorTimer(float l_waitTime)
    {
        yield return new WaitForSeconds(l_waitTime);
        ResourceIncrease();
        StartCoroutine(timer);
    }

    public void ResourceIncrease()
    {
        if (requestAsync == null)
        {
            requestAsync = UpdateResource();
            StartCoroutine(requestAsync);
        }
    }

    private IEnumerator UpdateResource()
    {
        UpdateResourcesRequest request = new();
        request.token = GameManager.instance.token;
        request.goldIncome = _goldIncome;
        request.lumberIncome = _lumberIncome;
        request.manaIncome = _manaIncome;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);
        Debug.Log("REQUEST JSON:\n" + json);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            UpdateResourceResponse response = JsonUtility.FromJson<UpdateResourceResponse>(webRequest.downloadHandler.text);
            if (response.serverMessage == "Troops bought!")
            {

            }
        }
        requestAsync = null;
    }
}

[System.Serializable]
public class GatherGeneratorStatsRequest
{
    public string action = "gather_generator_stats";
    public string token;
}

[System.Serializable]
public class GatherGeneratorStatsResponse
{
    public string serverMessage;
    public string goldIncome;
    public string lumberIncome;
    public string manaIncome;
    public string goldUpgradePrice;
    public string lumberUpgradePrice;
    public string manaUpgradePrice;
}

[System.Serializable]
public class UpdateResourcesRequest
{
    public string action = "update_resource";
    public string token;
    public int goldIncome;
    public int lumberIncome;
    public int manaIncome;
}

[System.Serializable]
public class UpdateResourceResponse
{
    public string serverMessage;
    public int goldIncome;
    public int lumberIncome;
    public int manaIncome;
    public int goldUpgradePrice;
    public int lumberUpgradePrice;
    public int manaUpgradePrice;
}