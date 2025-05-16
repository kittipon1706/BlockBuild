using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class group_button : MonoBehaviour
{
    public Button my_button;
    private TextMeshProUGUI name_btn;
    public UnityEvent setup = new UnityEvent();
    public string name = "...";
    private void Awake()
    {
        name_btn = GetComponentInChildren<TextMeshProUGUI>();
        my_button = GetComponent<Button>();
        setup.AddListener(SetName);
    }

    void Start()
    {
        setup.Invoke();
    }

    private void SetName()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => name_btn.text = name);
    }
    private void OnDestroy()
    {
        my_button.onClick.RemoveAllListeners();
    }
}
