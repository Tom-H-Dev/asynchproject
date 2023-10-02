using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResourceGenerators : MonoBehaviour
{
    private const string url = "http://127.0.0.1/edsa-asyncserver/api.php";

    [Header("Resource info")]
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI lumberAmount, manaAmount;

    private float tick = 5;
    private bool isGathering;
    private IEnumerator timer, requestAsync, resourceGenerator;
    private int _goldIncome, _lumberIncome, _manaIncome;

    private void Start()
    {
        timer = GeneratorTimer(tick);
        StartCoroutine(timer);
    }

    IEnumerator GeneratorTimer(float l_waitTime)
    {
        Debug.Log("Starting timer");
        yield return new WaitForSeconds(l_waitTime);
        Debug.Log("function");
        ResourceIncrease();
    }

    public void ResourceIncrease()
    {
        //send resources to the db

        if (resourceGenerator == null)
        {
            resourceGenerator = UpdateResource();
            StartCoroutine(resourceGenerator);
        }
    }

    private IEnumerator UpdateResource()
    {
        Debug.Log("Updating resource start");
        UpdateResourcesRequest request = new();
        request.token = GameManager.instance.token;
        request.lastOnlineTick = GameManager.instance.lastTickTimeStamp;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);
        Debug.Log("REQUEST JSON:\n" + json);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            UpgradeResourceResponse response = JsonUtility.FromJson<UpgradeResourceResponse>(webRequest.downloadHandler.text);
            if (response.serverMessage == "Resource Update!")
            {
                goldAmount.text = "Gold: " + response.goldIncome;
                lumberAmount.text = "Lumber: " + response.lumberIncome;
                manaAmount.text = "Mana: "+ response.manaIncome;
            }
            Debug.Log(response.serverMessage);
        }
        resourceGenerator = null;
        Debug.Log("Updating resource end");
        Debug.Log(timer);
        StartCoroutine(GeneratorTimer(tick));
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
public class UpgradeResourcesRequest
{
    public string action = "upgrade_gold_mine";
    public string token;
    public string generatorType;
    public int goldIncome;
    public int lumberIncome;
    public int manaIncome;
}

[System.Serializable]
public class UpgradeResourceResponse
{
    public string serverMessage;
    public int goldIncome;
    public int lumberIncome;
    public int manaIncome;
    public int goldUpgradePrice;
    public int lumberUpgradePrice;
    public int manaUpgradePrice;
}

[System.Serializable]
public class UpdateResourcesRequest
{
    public string action = "update_resource";
    public string token;
    public int lastOnlineTick;
}

[System.Serializable]
public class UpdateResourcesResponse
{
    public string serverMessage;
    public string goldIncome;
    public string lumberIncome;
    public string manaIncome;
    public string lastOnlineTick;
}