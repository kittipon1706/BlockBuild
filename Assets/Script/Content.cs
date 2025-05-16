using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using static Data;

public class Content : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI content_name;
    [SerializeField]
    private Button apply_all_select;
    [SerializeField]
    private TMP_InputField content_input;
    [SerializeField]
    public TMP_Dropdown content_dropdown;
    [SerializeField]
    private Data.DataType dataType;
    [SerializeField]
    public string selectedData;

    void Start()
    {
        content_name.text = GetNameByType(dataType);
        List<string> displayData = GetDataByType(dataType);
        if(content_dropdown != null)
        {
            content_dropdown.ClearOptions();
            content_dropdown.AddOptions(displayData);
            content_dropdown.onValueChanged.AddListener(GetSelectedData);
            GetSelectedData(0);
        }
        else if (content_input != null)
        {

        }
    }

    void GetSelectedData(int index)
    {
        selectedData = GetDataValueByType(dataType, index);
        create_scene.instance.SetSelectedBlockData();
    }

    string GetDataValueByType(DataType type, int index)
    {
        switch (type)
        {
            case DataType.Version:
                return VersionData.versions[index];
            case DataType.Rotation:
                return RotationData.types[index];
            case DataType.Collision:
                return CollisionData.value[index];
            case DataType.Render:
                return RenderData.types[index];
            default:
                return string.Empty;
        }
    }

    List<string> GetDataByType(DataType type)
    {
        switch (type)
        {
            case DataType.Version:
                return VersionData.versions;
            case DataType.Rotation:
                return RotationData.types;
            case DataType.Collision:
                return CollisionData.value;
            case DataType.Render:
                return RenderData.types;
            default:
                return new List<string> { "N/A" };
        }
    }

    string GetNameByType(DataType type)
    {
        switch (type)
        {
            case DataType.Version:
                return VersionData.dispaly_name;
            case DataType.Rotation:
                return RotationData.dispaly_name;
            case DataType.Collision:
                return CollisionData.dispaly_name;
            case DataType.Render:
                return RenderData.dispaly_name;
            default:
                return string.Empty;
        }
    }
}
