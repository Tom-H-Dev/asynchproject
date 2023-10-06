using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
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


    public void FindNewOpponent()
    {

    }

    public void BattleCurrentOpponent()
    {

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
