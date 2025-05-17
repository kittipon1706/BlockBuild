using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using static Data;
using static UnityEngine.Rendering.DebugUI;

public class main : MonoBehaviour
{
    public static main instance;    

    [SerializeField]
    public string extractPath;
    public string blockPath;
    public string modelPath;
    public string texturePath;
    [SerializeField]
    public string project_path;
    [SerializeField]
    public string og_block_path;
    [SerializeField]
    public string defualt_path = "Project Path";
    [SerializeField]
    private GameObject block_prefab;
    [SerializeField]
    private GameObject group_prefab;
    [SerializeField]
    public List<Data.BlockData> all_HoldblockData = new List<Data.BlockData>();
    [SerializeField]
    public List<Data.BlockData> all_blockData = new List<Data.BlockData>();

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ClearExtractFolder();
        Craete_Folder_Project();
        project_path = defualt_path;
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    void Update()
    {
        
    }
    private void Craete_Folder_Project()
    {
        //modelPath = additional_path.text.Length > 0 ? modelPath + "/" + additional_path.text : modelPath;
        //texturePath = additional_path.text.Length > 0 ? texturePath + "/" + additional_path.text : texturePath;
        //blockPath = additional_path.text.Length > 0 ? blockPath + "/" + additional_path.text : blockPath;

        if (!Directory.Exists(UnityEngine.Application.streamingAssetsPath + extractPath))
        {
            Directory.CreateDirectory(UnityEngine.Application.streamingAssetsPath + extractPath);
        }
        if (!Directory.Exists(UnityEngine.Application.streamingAssetsPath + modelPath))
        {
            Directory.CreateDirectory(UnityEngine.Application.streamingAssetsPath + modelPath);
        }
        if (!Directory.Exists(UnityEngine.Application.streamingAssetsPath + blockPath))
        {
            Directory.CreateDirectory(UnityEngine.Application.streamingAssetsPath + blockPath);
        }
        if (!Directory.Exists(UnityEngine.Application.streamingAssetsPath + texturePath))
        {
            Directory.CreateDirectory(UnityEngine.Application.streamingAssetsPath + texturePath);
        }

        AssetDatabase.Refresh();
    }

    public void ClearExtractFolder()
    {
        DirectoryInfo extract_di = new DirectoryInfo(UnityEngine.Application.streamingAssetsPath + extractPath);
        foreach (FileInfo file in extract_di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in extract_di.GetDirectories())
        {
            dir.Delete(true);
        }
        DirectoryInfo model_di = new DirectoryInfo(UnityEngine.Application.streamingAssetsPath + modelPath);
        foreach (FileInfo file in model_di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in model_di.GetDirectories())
        {
            dir.Delete(true);
        }
        DirectoryInfo block_di = new DirectoryInfo(UnityEngine.Application.streamingAssetsPath + blockPath);
        foreach (FileInfo file in block_di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in block_di.GetDirectories())
        {
            dir.Delete(true);
        }
        DirectoryInfo texture_di = new DirectoryInfo(UnityEngine.Application.streamingAssetsPath + texturePath);
        foreach (FileInfo file in texture_di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in texture_di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    public static void Copy(string sourceDirectory, string targetDirectory)
    {
        var diSource = new DirectoryInfo(sourceDirectory);
        var diTarget = new DirectoryInfo(targetDirectory);

        CopyAll(diSource, diTarget);
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        Directory.CreateDirectory(target.FullName);

        // Copy each file into the new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir =
                target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }

    public void LoadBlockAsset(DirectoryInfo dir)
    {
        if (dir != null)
        {
            CopyAll(new DirectoryInfo(dir.FullName), new DirectoryInfo(Application.streamingAssetsPath + main.instance.extractPath));
        }
        else Debug.LogError("Fail");
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public GameObject CreateBLockCentent(Transform panel, Data.BlockData blockData, string subfolder)
    {
        if (blockData != null)
        {
            GameObject button_obj = Instantiate(block_prefab, panel.transform);
            button button = button_obj.GetComponent<button>();
            button.name = blockData.blockName;
            button.sub_folder = subfolder;
            button.blockData = blockData;
            return button_obj;
        }
        else return null;
    }

    public GameObject CreateGroupContent(string name, Transform panel)
    {
        GameObject obj = Instantiate(group_prefab, panel.transform);
        group_button button = obj.GetComponent<group_button>();
        button.name = name;
        return obj;
    }

    public List<GameObject> CreateGroupsContent(DirectoryInfo block_dir, Transform panel)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (DirectoryInfo dir in block_dir.GetDirectories())
        {
            group_button button = Instantiate(group_prefab, panel.transform).GetComponent<group_button>();
            button.name = dir.Name;
            result.Add(button.gameObject);
        }
        return result;
    }

    public void Reset_Project()
    {
        project_path = defualt_path;
        ClearExtractFolder();
    }
    public void RemoveAllContent(Transform panel)
    {
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }
    }

    public DirectoryInfo FindBlockDir(string subfolderl)
    {
        DirectoryInfo block_dir = null;

        if (project_path.Length > 0)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(project_path).GetDirectories();
            DirectoryInfo bp_dir = null;
            foreach (DirectoryInfo dir in dirs)
            {
                if (dir.Name.Contains("bp") || dir.Name.Contains("BP"))
                    bp_dir = dir;
                else if (dir.Name == "blocks")
                    block_dir = dir;
            }

            if (bp_dir != null && block_dir == null)
            {
                foreach (DirectoryInfo dir in bp_dir.GetDirectories())
                {
                    if (dir.Name == "blocks")
                        block_dir = dir;
                }
            }
            else if (bp_dir == null && block_dir == null)
            {
                if (project_path.Contains("blocks"))
                    block_dir = new DirectoryInfo(project_path);
            }
            og_block_path = block_dir.FullName;
        }
        return block_dir;
    }

    public void OnApplicationQuit()
    {
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public (Vector3 offset, Vector3 size) Calculate_Selection_Box(string name, GameObject model_obj)
    {
        BoxCollider boxCollider = model_obj.GetComponent<BoxCollider>();

        Vector3 localCenter = boxCollider.center;
        Vector3 localOffset = model_obj.transform.localPosition;
        Vector3 center = boxCollider.center;
        Vector3 offset = Vector3.zero;
        Vector3 size = boxCollider.size * 16f;
        offset.x = SmartRound((-size.x / 2f) + center.x);
        offset.z = SmartRound((-size.z / 2f) + center.z);

        BlockData result = main.instance.all_blockData.Find(b => b.blockName == name.Replace(".geo.json", ""));

        if (result != null)
        {
            result.selectionBox_size = size;
            result.selectionBox_origin = offset;
        }
        else
        {
            Debug.Log("Fail");
        }

        return (offset, size);
    }

    public float SmartRound(float value)
    {
        float floor = Mathf.Floor(value);
        float fraction = value - floor;
        if (fraction > 0.5f)
            return Mathf.Ceil(value);
        else
            return Mathf.Floor(value);
    }
    public BlockData Get_BlockData(string name)
    {
        return all_blockData.FirstOrDefault(b => b.blockName == name) ?? null;
    }

    public void Set_BlockData(string name, DataType dataType, string value, Vector3 vecValue)
    {
        BlockData blockData = Get_BlockData(name);

        if (blockData != null)
        {
            switch (dataType)
            {
                case DataType.Version :
                    blockData.format_Version = value;
                    break;
                case DataType.Render:
                    blockData.render_method = value;
                    break;
                case DataType.Rotation:
                    blockData.rotationType = value;
                    break;
                case DataType.Collision:
                    blockData.collision = value;
                    break;
                case DataType.NameSpace:
                    blockData.namespaceId = value;
                    break;
                case DataType.Selection_Box_Origin:
                    blockData.selectionBox_origin = vecValue;
                    break;
                case DataType.Selection_Box_Size:
                    blockData.selectionBox_size = vecValue;
                    break;
            }
            int index = all_blockData.FindIndex(b => b.blockName == blockData.blockName);
            all_blockData[index] = blockData;
        }
        else
        {
            all_blockData.Add(blockData);
        }
    }

    public void Set_HoldBlackData(BlockData blockData)
    {
        int index = all_HoldblockData.FindIndex(b => b.blockName == blockData.blockName);

        if (index != -1)
        {
            all_HoldblockData[index] = blockData;
        }
        else
        {
            all_HoldblockData.Add(blockData);
        }
    }

    public void Remove_HoldBlackData(BlockData blockData)
    {
        all_HoldblockData.RemoveAll(b => b.blockName == blockData.blockName);
    }

    public void RemoveAll_HoldBlackData()
    {
        button[] buttons = FindObjectsOfType<button>();

        foreach (button btn in buttons)
        {
            btn.Set_Hold(false);
        }
    }

    public List<BlockData> Get_AllHoldBlackData()
    {
        return all_HoldblockData;
    }
}
