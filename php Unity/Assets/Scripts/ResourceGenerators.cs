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

    [Header("Resource Generator Stats")]
    public TextMeshProUGUI goldIncomeAmount;
    public TextMeshProUGUI lumberIncomeAmount, manaIncomeAmount;
    public TextMeshProUGUI goldUpgradeButton, lumberUpgradeButton, manaUpgradeButton;

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
        yield return new WaitForSeconds(l_waitTime);
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
        UpdateResourcesRequest request = new();
        request.token = GameManager.instance.token;
        request.lastOnlineTick = GameManager.instance.lastTickTimeStamp;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            UpgradeResourceResponse response = JsonUtility.FromJson<UpgradeResourceResponse>(webRequest.downloadHandler.text);
            if (response.serverMessage == "Resource Update!")
            {
                goldAmount.text = "Gold: " + response.goldIncome;
                lumberAmount.text = "Lumber: " + response.lumberIncome;
                manaAmount.text = "Mana: "+ response.manaIncome;
            }
        }
        resourceGenerator = null;
        StartCoroutine(GeneratorTimer(tick));
    }


    public void UpgradeResourceButton(string type)
    {
        if (requestAsync == null)
        {
            requestAsync = UpgradeResourceGenerator(type);
            StartCoroutine(requestAsync);
        }
    }

    private IEnumerator UpgradeResourceGenerator(string genType)
    {
        UpgradeResourcesRequest request = new();
        request.token = GameManager.instance.token;
        request.generatorType = genType;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            UpgradeResourceResponse response = JsonUtility.FromJson<UpgradeResourceResponse>(webRequest.downloadHandler.text);
            if (response.serverMessage == "Upgrade!")
            {
                goldIncomeAmount.text = "+" + response.goldIncome;
                lumberIncomeAmount.text = "+" + response.lumberIncome;
                manaIncomeAmount.text = "+" + response.manaIncome;

                goldUpgradeButton.text = "Upgrade Cost:" + "\n" + response.goldUpgradePrice + " Gold";
                lumberUpgradeButton.text = "Upgrade Cost:" + "\n" + response.lumberUpgradePrice + " Lumber";
                manaUpgradeButton.text = "Upgrade Cost:" + "\n" + response.manaUpgradePrice + " Mana";

                goldAmount.text = "Gold: " + response.gold;
                lumberAmount.text = "Lumber: " + response.lumber;
                manaAmount.text = "Mana: " + response.mana;
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
public class UpgradeResourcesRequest
{
    public string action = "upgrade_generator";
    public string token;
    public string generatorType;
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
    public int gold;
    public int lumber;
    public int mana;
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