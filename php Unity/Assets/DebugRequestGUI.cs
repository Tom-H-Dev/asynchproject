using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugRequestGUI : MonoBehaviour
{
    private VisualElement root;

    private Button requestButton;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        requestButton = root.Q<Button>("Send-Button");
        requestButton.RegisterCallback<ClickEvent>(evt =>
        {
            Debug.Log("De knop werkt");
        });
    }
}
