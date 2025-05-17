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
    [SerializeField]
    private Button hold_selected_btn;
    [SerializeField]
    private bool Onhold;

    private void Awake()
    {
        Onhold = false;
        name = string.Empty;
        sub_folder = string.Empty;
        name_btn = GetComponentInChildren<TextMeshProUGUI>();
        setup.AddListener(DoSomething);
        my_button.onClick.AddListener(Oncilck);
        hold_selected_btn.onClick.AddListener(OnClick_holdSelect_Btn);
    }

    void Start()
    {
        setup.Invoke();
    }
    private void OnClick_holdSelect_Btn()
    {
        Set_Hold(!Onhold);
    }

    public void Set_Hold(bool value)
    {
        Debug.Log(value);
        Onhold = value;
        if (Onhold == true)
        {
            hold_selected_btn.GetComponent<Image>().color = Color.green;
            main.instance.Set_HoldBlackData(blockData);
            if (main.instance.Get_AllHoldBlackData().Count == 1)
            {
                GameObject preview_model = Block_Preview.instance.LoadGeoModel(name.Replace(".geo.json", ""));
            }
            else if (main.instance.Get_AllHoldBlackData().Count > 1)
            {
                Block_Preview.instance.ClearModel(true);
            }
            create_scene.instance.Get_SelectedBlockData(main.instance.Get_AllHoldBlackData());
        }
        else
        {
            hold_selected_btn.GetComponent<Image>().color = Color.white;
            main.instance.Remove_HoldBlackData(blockData);
            if (main.instance.Get_AllHoldBlackData().Count <= 0)
            {
                Block_Preview.instance.ClearModel(true);
                create_scene.instance.ShowDefualt();
            }
            else if (main.instance.Get_AllHoldBlackData().Count == 1)
            {
                GameObject preview_model = Block_Preview.instance.LoadGeoModel(name.Replace(".geo.json", ""));
            }
        }
    }

    private void Oncilck()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            edit_scene.instance.ShowBlockData(name, sub_folder);
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (main.instance.Get_AllHoldBlackData().Count >= 1 && !Onhold == true)
                main.instance.RemoveAll_HoldBlackData();
            Set_Hold(!Onhold);
        }
    }

    void DoSomething()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => name_btn.text = name);
    }

    private void OnDestroy()
    {
        my_button.onClick.RemoveAllListeners();
        hold_selected_btn.onClick.RemoveAllListeners();
    }
}
