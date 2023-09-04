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
    private IEnumerator createAccountAsync;

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
            if (createAccountAsync == null)
            {
                createAccountAsync = CreateAccountAsync();
                StartCoroutine(createAccountAsync);
            }
            Debug.Log("het werkt");
        });

        loginButton.RegisterCallback<ClickEvent>(evt =>
        {

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
        createAccountAsync = null;
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