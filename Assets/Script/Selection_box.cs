using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static Data;

public class Selection_box : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField value_x;
    [SerializeField]
    private TMP_InputField value_y;
    [SerializeField]
    private TMP_InputField value_z;
    [SerializeField] 
    private DataType dataType;

    void Start()
    {
        value_x.onEndEdit.AddListener(onChangeValue);
        value_y.onEndEdit.AddListener(onChangeValue);
        value_z.onEndEdit.AddListener(onChangeValue);
    }

    public Vector3 GetValue()
    {
        Vector3 vector = Vector3.zero;
        vector.x = float.Parse(value_x.text);
        vector.y = float.Parse(value_y.text);
        vector.z = float.Parse(value_z.text);
        return vector;
    }

    public void SetValue(Vector3 vector)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => value_x.text = vector.x.ToString());
        UnityMainThreadDispatcher.Instance().Enqueue(() => value_y.text = vector.y.ToString());
        UnityMainThreadDispatcher.Instance().Enqueue(() => value_z.text = vector.z.ToString());
    }
    private void onChangeValue(string x)
    {
        Vector3 offset = GetValue();
        Vector3 size = GetValue();
        if (dataType == DataType.Selection_Box_Origin)
        {
            offset.x = main.instance.SmartRound((-size.x / 2f) + offset.x);
            offset.y = 0;
            offset.z = main.instance.SmartRound((-size.z / 2f) + offset.z);

            List<BlockData> tempList = new List<BlockData>(main.instance.Get_AllHoldBlackData());

            foreach ( BlockData blockData in tempList)
            {
                main.instance.Set_BlockData(blockData.blockName, DataType.Selection_Box_Origin, string.Empty, 0.0f, offset);
                main.instance.Set_HoldBlackData(main.instance.Get_BlockData(blockData.blockName));
            }
        }
        else if(dataType == DataType.Selection_Box_Size)
        {
            List<BlockData> tempList = new List<BlockData>(main.instance.Get_AllHoldBlackData());

            foreach (BlockData blockData in tempList)
            {
                main.instance.Set_BlockData(blockData.blockName, DataType.Selection_Box_Size, string.Empty, 0.0f, size);
                main.instance.Set_HoldBlackData(main.instance.Get_BlockData(blockData.blockName));
            }
        }
    }
    private void OnDestroy()
    {
        value_x.onEndEdit.RemoveAllListeners();
        value_y.onEndEdit.RemoveAllListeners();
        value_z.onEndEdit.RemoveAllListeners();
    }
}
