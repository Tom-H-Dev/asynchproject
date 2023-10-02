// Ignore Spelling: mage mana noresource

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArmyManager : MonoBehaviour
{
    private const string url = "http://127.0.0.1/edsa-asyncserver/api.php";

    [Header("Resource info")]
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI lumberAmount, manaAmount;
    private int _gold, _lumber, _mana;
    [Header("Troop amount")]
    public TextMeshProUGUI peasantAmount;
    public TextMeshProUGUI knightAmount, archerAmount, mageAmount, catapultAmount;
    private int _peasant, _knight, _archer, _mage, _catapult;
    public TMP_InputField peasantInput, knightInput, archerInput, mageInput, catapultInput;
    [Header("Order")]
    public TextMeshProUGUI totalOrderAmount;
    [Header("BuyPrices")]
    [SerializeField] private int _goldPrice;
    [SerializeField] private int _lumberPrice, _manaPrice;
    [SerializeField] private Vector3 _peasantPrice = new Vector3(5, 5, 0), _knightPrice = new Vector3(15, 5, 0), _archerPrice = new Vector3(10, 15, 5), _magePrice = new Vector3(20, 0, 20), _catapultPrice = new Vector3(50, 50, 10);
    [Header("Error Message")]
    public TextMeshProUGUI noResourceMessage;

    [Space]
    private IEnumerator requestAsync;

    void Start()
    {
        noResourceMessage.text = string.Empty;
        //Show values
        StartCoroutine(DisplayStartValues());
    }

    public void UpdateTotalUnitCost()
    {
        _goldPrice = (int)_peasantPrice.x * int.Parse(peasantInput.text) + (int)_knightPrice.x * int.Parse(knightInput.text) + (int)_archerPrice.x * int.Parse(archerInput.text) + (int)_magePrice.x * int.Parse(mageInput.text) + (int)_catapultPrice.x * int.Parse(catapultInput.text);
        _lumberPrice = (int)_peasantPrice.y * int.Parse(peasantInput.text) + (int)_knightPrice.y * int.Parse(knightInput.text) + (int)_archerPrice.y * int.Parse(archerInput.text) + (int)_magePrice.y * int.Parse(mageInput.text) + (int)_catapultPrice.y * int.Parse(catapultInput.text);
        _manaPrice = (int)_peasantPrice.z * int.Parse(peasantInput.text) + (int)_knightPrice.z * int.Parse(knightInput.text) + (int)_archerPrice.z * int.Parse(archerInput.text) + (int)_magePrice.z * int.Parse(mageInput.text) + (int)_catapultPrice.z * int.Parse(catapultInput.text);
        totalOrderAmount.text = "Total Order Cost: Gold: " + _goldPrice + " Lumber: " + _lumberPrice + " Mana: " + _manaPrice;
        _peasant = int.Parse(peasantInput.text);
        _knight = int.Parse(knightInput.text);
        _archer = int.Parse(archerInput.text);
        _mage = int.Parse(mageInput.text);
        _catapult = int.Parse(catapultInput.text);
    }

    public void OrderTroops()
    {
        if (requestAsync == null)
        {
            requestAsync = BuyTroopsASync();
            StartCoroutine(requestAsync);
        }
    }

    public void LogOutButton()
    {
        if (requestAsync == null)
        {
            requestAsync = LogOutAsync();
            StartCoroutine(requestAsync);
        }
    }

    private IEnumerator DisplayStartValues()
    {
        ResourceUpdateRequest request = new();
        request.token = GameManager.instance.token;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            ResourceUpdateResponse response = JsonUtility.FromJson<ResourceUpdateResponse>(webRequest.downloadHandler.text);
            Debug.Log(response.serverMessage);

            peasantAmount.text = response.peasant.ToString();
            knightAmount.text = response.knight.ToString();
            archerAmount.text = response.archer.ToString();
            mageAmount.text = response.mage.ToString();
            catapultAmount.text = response.catapult.ToString();

            goldAmount.text = "Gold: " + response.gold;
            lumberAmount.text = "Lumber: " + response.lumber;
            manaAmount.text = "Mana: " + response.mana;
        }
        requestAsync = null;
    }

    private IEnumerator BuyTroopsASync()
    {
        BuyTroopsRequest request = new();
        request.token = GameManager.instance.token;
        request.peasantTroop = _peasant;
        request.knightTroop = _knight;
        request.archerTroop = _archer;
        request.mageTroop = _mage;
        request.catapultTroop = _catapult;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);
        Debug.Log("REQUEST JSON:\n" + json);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            BuyTroopsResponse response = JsonUtility.FromJson<BuyTroopsResponse>(webRequest.downloadHandler.text);
            if (response.serverMessage == "Troops bought!")
            {
                peasantAmount.text = response.peasant.ToString();
                knightAmount.text = response.knight.ToString();
                archerAmount.text = response.archer.ToString();
                mageAmount.text = response.mage.ToString();
                catapultAmount.text = response.catapult.ToString();

                goldAmount.text = "Gold: " + response.gold;
                lumberAmount.text = "Lumber: " + response.lumber;
                manaAmount.text = "Mana: " + response.mana;
            }
            else noResourceMessage.text = response.noresource;
        }
        requestAsync = null;
    }

    private IEnumerator LogOutAsync()
    {
        LogOutRequest request = new();
        request.token = GameManager.instance.token;


        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            LogOutResponse response = JsonUtility.FromJson<LogOutResponse>(webRequest.downloadHandler.text);
            if (response.serverMessage == "Succes")
            {
                //Load main Scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
            Debug.Log(response.serverMessage);
        }
        requestAsync = null;
    }
}

[System.Serializable]
public class ResourceUpdateRequest
{
    public string action = "Resource_Display";
    public string token;
}
[System.Serializable]
public class ResourceUpdateResponse
{
    public string serverMessage;
    public int gold;
    public int lumber;
    public int mana;
    public int peasant;
    public int knight;
    public int archer;
    public int mage;
    public int catapult;
}
[System.Serializable]
public class BuyTroopsRequest
{
    public string action = "Buy_Troops_Request";
    public string token;
    public int peasantTroop;
    public int knightTroop;
    public int archerTroop;
    public int mageTroop;
    public int catapultTroop;
}
[System.Serializable]
public class BuyTroopsResponse
{
    public string serverMessage;
    public int gold;
    public int lumber;
    public int mana;
    public int peasant;
    public int knight;
    public int archer;
    public int mage;
    public int catapult;
    public string noresource;
}

[System.Serializable]
public class LogOutRequest
{
    public string action = "logout_action";
    public string token;
}

[System.Serializable]
public class LogOutResponse
{
    public string serverMessage;
}