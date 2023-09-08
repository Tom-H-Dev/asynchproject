using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class LoginScreen : MonoBehaviour
{
    private const string url = "http://127.0.0.1/edsa-asyncserver/api.php";

    private VisualElement root;
    private TextField createEmailText, createPasswordText, createUsernameText, loginEmailText, loginPasswordText;
    private Button createAccountButton, loginButton;
    private Label outputLabel;
    private IEnumerator requestAsync;

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

        //Output
        outputLabel = root.Q<Label>("output-label");

        

        //Clicks
        createAccountButton.RegisterCallback<ClickEvent>(evt =>
        {
            if (requestAsync == null)
            {
                requestAsync = CreateAccountAsync();
                StartCoroutine(requestAsync);
            }
            Debug.Log("het werkt");
        });

        loginButton.RegisterCallback<ClickEvent>(evt =>
        {
            if (requestAsync == null)
            {
                requestAsync = LoginAsync();
                StartCoroutine(requestAsync);
            }
            Debug.Log("Login Works");
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
            Debug.Log(webRequest.downloadHandler.text);
            CreateAccountResponse response = JsonUtility.FromJson<CreateAccountResponse>(webRequest.downloadHandler.text);
            Debug.Log(response.serverMessage);
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
            Debug.Log(response.serverMessage);
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