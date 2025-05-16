using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Selection_box : MonoBehaviour
{
    private enum selection_type
    {
        origin,
        size
    }

    [SerializeField]
    private TMP_InputField value_x;
    [SerializeField]
    private TMP_InputField value_y;
    [SerializeField]
    private TMP_InputField value_z;
    [SerializeField] 
    private selection_type selection_Type;

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
        create_scene.instance.SetSelectedBlockData();
    }
    private void OnDestroy()
    {
        value_x.onEndEdit.RemoveAllListeners();
        value_y.onEndEdit.RemoveAllListeners();
        value_z.onEndEdit.RemoveAllListeners();
    }
}
