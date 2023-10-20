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
    [SerializeField] private TextMeshProUGUI _peasantEnemyAmount, _knightEnemyAmount, _archerEnemyAmount, _mageEnemyAmount, _catapultEnemyAmount;

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI _gold;
    [SerializeField] private TextMeshProUGUI _lumber, _mana;
    [SerializeField] private TextMeshProUGUI _peasantBattle, _peasantArmy, _knightBattle, _knightArmy, _archerBattle, _archerArmy, _mageBattle, _mageArmy, _catapultBattle, _catapultArmy;
    private int latestOpponentID = 0;


    private void Start()
    {
        FindNewOpponent();
    }

    public void FindNewOpponent()
    {
        if (requestAsync == null)
        {
            requestAsync = FindNewOpponentCoroutine();
            StartCoroutine(requestAsync);
        }
    }

    public void BattleCurrentOpponent()
    {
        if (requestAsync == null)
        {
            requestAsync = BattleCurrentOpponentCoroutine();
            StartCoroutine(requestAsync);
        }
    }


    private IEnumerator FindNewOpponentCoroutine()
    {
        FindNewOpponentRequest request = new();
        request.token = GameManager.instance.token;
        request.latestOpponentID = latestOpponentID;

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
                _enemyNameTroops.text = response.opponentName + "'s Troops";

                _goldRecieveAmount.text = "Gold: " + response.opponentGold;
                _lumberRecieveAmount.text = "Lumber: " + response.opponentLumber;
                _manaRecieveAmount.text = "Mana: " + response.opponentMana;

                _peasantEnemyAmount.text = "Peasant: " + response.opponentPeasant;
                _knightEnemyAmount.text = "Knight: " + response.opponentKnight;
                _archerEnemyAmount.text = "Archer: " + response.opponentArcher;
                _mageEnemyAmount.text = "Mage: " + response.opponentMage;
                _catapultEnemyAmount.text = "Catapult: " + response.opponentCatapult;

                latestOpponentID = response.latestOpponentID;
            }
        }
        requestAsync = null;

    }

    private IEnumerator BattleCurrentOpponentCoroutine()
    {
        BattleOpponentRequest request = new();
        request.token = GameManager.instance.token;
        request.latestOpponentID = latestOpponentID;

        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            BattleOpponentResponse response = JsonUtility.FromJson<BattleOpponentResponse>(webRequest.downloadHandler.text);
            Debug.Log(response.serverMessage);

            _peasantArmy.text = response.peasant.ToString();
            _knightArmy.text = response.knight.ToString();
            _archerArmy.text = response.archer.ToString();
            _mageArmy.text = response.mage.ToString();
            _catapultArmy.text = response.catapult.ToString();

            _peasantBattle.text = "Peasant: " + response.peasant;
            _knightBattle.text = "Knight: " + response.knight;
            _archerBattle.text = "Archer: " + response.archer;
            _mageBattle.text = "Mage: " + response.mage;
            _catapultBattle.text = "Catapult: " + response.catapult;

            if (response.serverMessage == "Battle Won!")
            {
                _gold.text = "Gold: " + response.gold;
                _lumber.text = "Lumber: " + response.lumber;
                _mana.text = "Mana: " + response.mana;

                //TODO: Pop up won
                Debug.Log("You won!");
            }
            if (response.serverMessage == "Battle Lost.")
            {
                //TODO: Pop up you lost
                Debug.Log("You lost. :(");
            }

            GameManager.instance.debugID = response.debugID;
        }
        requestAsync = null;
        FindNewOpponent();
    }

    //Find new opponent (Goes through the database and find another player by ID that you haven't found yet (stored in list) and display 10% of resources that you can steal)
    //Pop up of when you are attacked ((new bool in database) and show how many troops and resources you have lost(and remove them from database))
    //Check if player has been attacked and skip those players
    // when you attack play small simple animation to show that you are attacking
}

[System.Serializable]
public class FindNewOpponentRequest
{
    public string action = "find_new_opponent";
    public string token;
    public int latestOpponentID;
}


[System.Serializable]
public class FindNewOpponentResponse
{
    public string serverMessage;
    public int latestOpponentID;

    public string opponentName;

    public int opponentGold;
    public int opponentLumber;
    public int opponentMana;

    public int opponentPeasant;
    public int opponentKnight;
    public int opponentArcher;
    public int opponentMage;
    public int opponentCatapult;

    public int debugID;
}

[System.Serializable]
public class BattleOpponentRequest
{
    public string action = "battle_opponent";
    public string token;

    public int latestOpponentID;
}


[System.Serializable]
public class BattleOpponentResponse
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

    public int debugID;
}