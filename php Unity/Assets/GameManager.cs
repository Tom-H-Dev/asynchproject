using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public const string url = "http://127.0.0.1/edsa-asyncserver/api.php";
    public string token;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        else instance = this;

        DontDestroyOnLoad(gameObject);
    }




}
