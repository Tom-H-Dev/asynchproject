using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BattleManager : MonoBehaviour
{
    private const string url = "http://127.0.0.1/edsa-asyncserver/api.php";
    private IEnumerator requestAsync;

    [Header("Enemy Stats")]
    [SerializeField] private TextMeshProUGUI _enemyName;
    [Space]
    [SerializeField] private TextMeshProUGUI _goldRecieveAmount;
    [SerializeField] private TextMeshProUGUI _lumberRecieveAmount, _manaRecieveAmount;
    [Space]
    [SerializeField] private TextMeshProUGUI _enemyNameTroops;
    private string _secondPartTroops = " Troops";
    [SerializeField] private TextMeshProUGUI _peasantEnemyAmount, _knightEnemyAmount, _archerEnemyAmount, _mageEnemyAmount, _catapultEnemyAmount;

    [Header("Player Stats")]
    private string nothing;


    private void OnEnable()
    {
        
    }

    public void FindNewOpponent()
    {
        if (requestAsync == null)
        {
            requestAsync = UpgradeResourceGenerator();
            StartCoroutine(requestAsync);
        }
    }

    public void BattleCurrentOpponent()
    {

    }


    private IEnumerator UpgradeResourceGenerator()
    {
        FindNewOpponentRequest request = new();
        request.token = GameManager.instance.token;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            FindNewOpponentResponse response = JsonUtility.FromJson<FindNewOpponentResponse>(webRequest.downloadHandler.text);
            Debug.Log(response.serverMessage);
            if (response.serverMessage == "Find New Opponent.")
            {
                _enemyName.text = response.opponentName;

                _goldRecieveAmount.text = "Gold: " + response.opponentGold;
                _lumberRecieveAmount.text = "Lumber: " + response.opponentLumber;
                _manaRecieveAmount.text = "Mana: " + response.opponentMana;
            }
        }
        requestAsync = null;

    }

    //Find new opponent (Goes through the database and find another player by ID that you haven't found yet (stored in list) and display 10% of resources that you can steal)
    //Pop up of when you are attacked ((new bool in database) and show how many troops and resources you have lost(and remove them from database))
    //Check if player has been attacked and skip those players
    //Battle button where you attack the other player
    //Get a percentage of the enemy troops and display those
    //display the name of the opponent by id
    // when you attack play small simple animation to show that you are attacking
    //check your troops vs enemy troops and check who wins.
    //Set attack and defense values and combine those when attacking and check who will win.
}

[System.Serializable]
public class FindNewOpponentRequest
{
    public string action = "find_new_opponent";
    public string token;
    public string latestOpponentID;   
}


[System.Serializable]
public class FindNewOpponentResponse
{
    public string serverMessage;
    public string latestOpponentID;

    public string opponentName;

    public int opponentGold;
    public int opponentLumber;
    public int opponentMana;
    
    public int opponentPeasant;
    public int opponentKnight;
    public int opponentArcher;
    public int opponentMage;
    public int opponentCatapult;
}