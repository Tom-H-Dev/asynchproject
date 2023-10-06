using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginScreen : MonoBehaviour
{
    private const string url = "http://127.0.0.1/edsa-asyncserver/api.php";

    private VisualElement root;
    private TextField createEmailText, createPasswordText, createUsernameText, loginEmailText, loginPasswordText;
    private Button createAccountButton, loginButton;
    private IEnumerator requestAsync;
    private VisualElement logedInElement;
    private Button debugGameActionButton, logOutButton;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        //Register
        createEmailText = root.Q<TextField>("EmailTextField");
        createUsernameText = root.Q<TextField>("UsernameTextField");
        createPasswordText = root.Q<TextField>("PasswordTextField");
        createAccountButton = root.Q<Button>("CreateAccountButton");

        //Login
        loginEmailText = root.Q<TextField>("EmailLoginTextField");
        loginPasswordText = root.Q<TextField>("PasswordLoginTextField");
        loginButton = root.Q<Button>("LoginButton");

        //Debug
        debugGameActionButton = root.Q<Button>("Game-Action-Button");
        logedInElement = root.Q<VisualElement>("Loged-in-element");
        logedInElement.style.display = DisplayStyle.None;

        //Log out
        logOutButton = root.Q<Button>("Log-Out");
        logOutButton.style.display = DisplayStyle.None;


        //Clicks
        createAccountButton.RegisterCallback<ClickEvent>(evt =>
        {
            if (requestAsync == null)
            {
                requestAsync = CreateAccountAsync();
                StartCoroutine(requestAsync);
            }
        });
        loginButton.RegisterCallback<ClickEvent>(evt =>
        {
            if (requestAsync == null)
            {
                requestAsync = LoginAsync();
                StartCoroutine(requestAsync);
            }
        });
        debugGameActionButton.RegisterCallback<ClickEvent>(evt =>
        {
            if (requestAsync == null)
            {
                requestAsync = GameAsync();
                StartCoroutine(requestAsync);
            }
            Debug.Log("game debug async Works");
        });
    }

    private IEnumerator CreateAccountAsync()
    {
        CreateAccountRequest request = new();
        request.email = createEmailText.text;
        request.username = createUsernameText.text;
        request.password = createPasswordText.text;


        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            CreateAccountResponse response = JsonUtility.FromJson<CreateAccountResponse>(webRequest.downloadHandler.text);
        }
        requestAsync = null;
    }

    private IEnumerator LoginAsync()
    {
        LoginRequest request = new();
        request.email = loginEmailText.text;
        request.password = loginPasswordText.text;


        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);
            if (response.serverMessage == "Succes")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                //logedInElement.style.display = DisplayStyle.Flex;
                //logOutButton.style.display = DisplayStyle.Flex;
            }
            GameManager.instance.token = response.token;

            Debug.Log(response.serverMessage);
        }
        requestAsync = null;
    }

    private IEnumerator GameAsync()
    {
        GameRequest request = new();
        request.token = GameManager.instance.token;


        List<IMultipartFormSection> formData = new();
        string json = JsonUtility.ToJson(request);

        MultipartFormDataSection entery = new("json", json);
        formData.Add(entery);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            yield return webRequest.SendWebRequest();
            GameResponse response = JsonUtility.FromJson<GameResponse>(webRequest.downloadHandler.text);
        }
        requestAsync = null;
    }
}

[System.Serializable]
public class CreateAccountRequest
{
    public string action = "create_account";

    public string email;
    public string username;
    public string password;
}

[System.Serializable]
public class CreateAccountResponse
{
    public string serverMessage;
}

[System.Serializable]
public class LoginRequest
{
    public string action = "login_request";
    public string email;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public string serverMessage;
    public string token;
}

[System.Serializable]
public class GameRequest
{
    public string action = "game_action";
    public string token;
}

[System.Serializable] 
public class GameResponse
{
    public string serverMessage;
}

