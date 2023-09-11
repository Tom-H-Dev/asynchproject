using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string token;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        else instance = this;

        DontDestroyOnLoad(gameObject);
    }




}
