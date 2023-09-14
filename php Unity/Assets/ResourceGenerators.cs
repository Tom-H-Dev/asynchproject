using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerators : MonoBehaviour
{

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