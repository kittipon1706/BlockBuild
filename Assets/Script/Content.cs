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
    public TMP_Dropdown content_dropdown;
    [SerializeField]
    private Data.DataType dataType;
    [SerializeField]
    public string selectedData;

    void Start()
    {
        content_name.text = GetNameByType(dataType);
        List<string> displayData = GetDataByType(dataType);
        content_dropdown.ClearOptions();
        content_dropdown.AddOptions(displayData);
        content_dropdown.onValueChanged.AddListener(GetSelectedData);
        GetSelectedData(0);
    }

    void GetSelectedData(int index)
    {
        selectedData = GetDataValueByType(dataType, index);
        if (selectedData == "---") return;
        List<BlockData> tempList = new List<BlockData>(main.instance.Get_AllHoldBlackData());
        foreach (BlockData blockData in tempList)
        {
            main.instance.Set_BlockData(blockData.blockName, dataType, selectedData, Vector3.zero);
        }
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
