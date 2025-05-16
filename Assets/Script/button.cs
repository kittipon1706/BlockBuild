using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class button : MonoBehaviour
{
    public Button my_button;
    private TextMeshProUGUI name_btn;
    public UnityEvent setup = new UnityEvent();
    public string name;
    public string sub_folder;
    public Data.BlockData blockData;
    private void Awake()
    {
        name = string.Empty;
        sub_folder = string.Empty;
        name_btn = GetComponentInChildren<TextMeshProUGUI>();
        setup.AddListener(DoSomething);
        my_button.onClick.AddListener(Oncilck);
    }

    void Start()
    {
        setup.Invoke();
    }

    private void Oncilck()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            edit_scene.instance.ShowBlockData(name, sub_folder);
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            GameObject preview_model = Block_Preview.instance.LoadGeoModel(name.Replace(".geo.json", ""));
            var (center, size) = main.instance.Calculate_Selection_Box(name.Replace(".geo.json", ""), preview_model);
            create_scene.instance.GetSelectedBlockData(name);
        }
    }

    void DoSomething()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => name_btn.text = name);
    }

    private void OnDestroy()
    {
        my_button.onClick.RemoveAllListeners();
    }
}
