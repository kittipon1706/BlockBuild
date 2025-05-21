using Newtonsoft.Json.Linq;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
    public string bp_path;
    [SerializeField]
    public string rp_path;
    [SerializeField]
    public string og_block_path;
    [SerializeField]
    public string og_model_path;
    [SerializeField]
    public string og_texture_path;
    [SerializeField]
    public string og_texture_terrain_path;
    [SerializeField]
    public string og_block_json_path;
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
    [SerializeField]
    public List<string> rotate_filter = new List<string>();
    [SerializeField]
    public string group_filter;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else Destroy(gameObject);

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

    public void FindBlockDir()
    {
        DirectoryInfo block_dir = null;
        DirectoryInfo model_dir = null;
        DirectoryInfo texture_dir = null;

        if (project_path.Length > 0)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(project_path).GetDirectories();
            DirectoryInfo bp_dir = null;
            DirectoryInfo rp_dir = null;
            foreach (DirectoryInfo dir in dirs)
            {
                if (dir.Name.Contains("bp") || dir.Name.Contains("BP"))
                    bp_dir = dir;
                else if (dir.Name == "blocks")
                    block_dir = dir;

                if (!dir.Name.Contains("rp") && !dir.Name.Contains("RP"))
                {
                    if (dir.Name == "models")
                        model_dir = dir;
                    else if (dir.Name == "textures")
                        texture_dir = dir;
                }
                else rp_dir = dir;
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

            if (rp_dir != null)
            {
                foreach (DirectoryInfo dir in rp_dir.GetDirectories())
                {
                    if (dir.Name == "models")
                        model_dir = dir;
                    else if (dir.Name == "textures")
                        texture_dir = dir;
                }
            }
            else
            {
                if (project_path.Contains("models"))
                    model_dir = new DirectoryInfo(project_path);
                if (project_path.Contains("textures"))
                    texture_dir = new DirectoryInfo(project_path);
            }

            og_block_path = block_dir.FullName;
            og_model_path = model_dir.FullName;
            og_texture_path = texture_dir.FullName;
            rp_path = rp_dir.FullName;
            bp_path = bp_dir.FullName;

            FileInfo[] terrain_texture = texture_dir.GetFiles("terrain_texture.json");
            if(terrain_texture.Length > 0)
                og_texture_terrain_path = terrain_texture[0].FullName;

            FileInfo[] blocksJson = rp_dir.GetFiles("blocks.json");
            if (blocksJson.Length > 0)
                og_block_json_path = blocksJson[0].FullName;
        }
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
        Vector3 newSize = boxCollider.size;

        if (newSize.x >= 1.0f)
            newSize.x = 1.0f;

        if (newSize.z >= 1.0f)
            newSize.z = 1.0f;

        boxCollider.size = newSize;

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
        return all_blockData.FirstOrDefault(b => b.blockName.Contains(name)) ?? null;
    }

    public void Set_BlockData(string name, DataType dataType, string value, float flValue, Vector3 vecValue)
    {
        BlockData blockData = Get_BlockData(name);

        if (blockData != null)
        {
            switch (dataType)
            {
                case DataType.Version:
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
                case DataType.Destroy_Time:
                    blockData.destroy_time = flValue;
                    break;
            }
            int index = all_blockData.FindIndex(b => b.blockName == blockData.blockName);
            all_blockData[index] = blockData;

            if(dataType == DataType.NameSpace)
            {
                var geoJson = JObject.Parse(File.ReadAllText(blockData.geomerty));

                string newIdentifier = $"{"geometry"}.{blockData.namespaceId}.{Path.GetFileNameWithoutExtension(blockData.geomerty).Replace(".geo", "")}";

                if (geoJson["minecraft:geometry"] is JArray geometryArray && geometryArray.Count > 0)
                {
                    var geometryObj = (JObject)geometryArray[0];

                    if (geometryObj["description"] is JObject description)
                    {
                        description["identifier"] = newIdentifier;
                        Debug.Log($"✅ Updated identifier to: {newIdentifier}");

                        File.WriteAllText(blockData.geomerty, geoJson.ToString(Newtonsoft.Json.Formatting.Indented));
                    }
                    else
                    {
                        Debug.Log("❌ geometry.description ไม่พบ");
                    }
                }
                else
                {
                    Debug.Log("❌ minecraft:geometry ไม่พบ หรือไม่ใช่ array");
                }
            }
        }
        else
        {
            all_blockData.Add(blockData);
        }

        JsonData.SaveToFile(blockData.file_path, blockData);
    }
    public void Show_AllBlockBtn(Transform parent)
    {
        foreach (Transform child in parent)
        {
            button button = child.GetComponent<button>();
            if (button != null)
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    public void Show_BlockBtn(string searchText, string sub_folder, string rotationType, Transform parent, ToggleButtonColor[] toggleButtonColors)
    {
        Show_AllBlockBtn(parent);

        if (rotationType != string.Empty && toggleButtonColors.Length > 0)
        {
            if (!rotate_filter.Contains(rotationType))
            {
                foreach (ToggleButtonColor toggleButtonColor in toggleButtonColors)
                {
                    toggleButtonColor.SetButtonColor(UnityEngine.Color.green);
                }
                rotate_filter.Add(rotationType);
            }
            else
            {
                foreach (ToggleButtonColor toggleButtonColor in toggleButtonColors)
                {
                    toggleButtonColor.SetButtonColor(UnityEngine.Color.white);
                }
                rotate_filter.Remove(rotationType);
            }

            if (rotationType == RotationData.types[0])
            {
                foreach (ToggleButtonColor toggleButtonColor in toggleButtonColors)
                {
                    toggleButtonColor.SetButtonColor(UnityEngine.Color.green);
                }
                rotate_filter = new List<string> { RotationData.types[1], RotationData.types[2], RotationData.types[3], RotationData.types[4], RotationData.types[5], RotationData.types[6], RotationData.types[7], RotationData.types[8] };
            }
        }

        group_filter = sub_folder;

        foreach (Transform child in parent)
        {
            button button = child.GetComponent<button>();
            if (button != null)
            {
                if (searchText != string.Empty)
                {
                    button.gameObject.SetActive(button.blockData.blockName.ToLower().Contains(searchText.ToLower()));
                }

                if (group_filter != string.Empty)
                {
                    if (button.sub_folder != sub_folder)
                        button.gameObject.SetActive(false);
                }

                if (!rotate_filter.Contains(button.blockData.rotationType))
                    button.gameObject.SetActive(false);
            }
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

    public void SetAll_HoldBlackData(bool value)
    {
        if (create_scene.instance != null)
        {
            foreach (Transform child in create_scene.instance.block_panel)
            {
                button button = child.GetComponent<button>();
                if (button != null)
                {
                    button.Set_Hold(value);
                }
            }
        }
    }

    public List<BlockData> Get_AllHoldBlackData()
    {
        return all_HoldblockData;
    }

    public void ApplyAllToProject()
    {
        foreach (BlockData blockData in all_blockData)
        { 
            if (og_texture_terrain_path.Length <= 0)
            {
                Debug.LogError("Cannot write terrain: " + blockData.blockName);
                return;
            }
            var terrainJson = JObject.Parse(File.ReadAllText(og_texture_terrain_path));
            var textureData = (JObject)terrainJson["texture_data"];
            List<string> result = new List<string>();
            string saveFolder = og_texture_path;
            string[] parts = blockData.namespaceId.Split('_');
            if (parts.Length < 1) continue;
            if (parts.Length >= 2)
            {
                if (!result.Contains(parts[0])) result.Add(parts[0]);
                if (!result.Contains(parts[1])) result.Add(parts[1]);
            }
            else result.Add(blockData.namespaceId);

            foreach (string part in result)
                saveFolder = Path.Combine(saveFolder, part);
            saveFolder = Path.Combine(saveFolder, "blocks");
            string fullPath = Path.Combine(saveFolder, Path.GetFileNameWithoutExtension(blockData.texture) + ".png");
            int index = fullPath.IndexOf("textures");
            string relativePath = fullPath.Substring(index).Replace("\\", "/");

            textureData[blockData.namespaceId + ":" + Path.GetFileNameWithoutExtension(blockData.texture)] = new JObject
            {
                ["textures"] = relativePath
            };

            File.WriteAllText(og_texture_terrain_path, terrainJson.ToString());

            if (og_block_json_path.Length > 0)
            {
                var json = JObject.Parse(File.ReadAllText(og_block_json_path));

                json[blockData.namespaceId + ":" + Path.GetFileNameWithoutExtension(blockData.geomerty).Replace(".geo", "")] = new JObject
                {
                    ["sound"] = "grass"
                };

                File.WriteAllText(og_block_json_path, json.ToString());
            }
        }

        DirectoryInfo block_dir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath + blockPath));

        foreach (DirectoryInfo dir in block_dir.GetDirectories())
        {
            ApplyFilesFromDir(dir);
            foreach (DirectoryInfo sub in dir.GetDirectories())
            {
                ApplyFilesFromDir(sub);
            }
        }

        
    }

    private void ApplyFilesFromDir(DirectoryInfo dir)
    {
        ApplyFileType(dir.GetFiles("*.json").Where(file => !file.Name.EndsWith(".meta")).ToArray(), ".json", og_block_path, "json");
        ApplyFileType(dir.GetFiles("*.geo.json").Where(file => !file.Name.EndsWith(".meta")).ToArray(), ".geo.json", Path.Combine(og_model_path, "blocks"), "geo");
        ApplyFileType(dir.GetFiles("*.png").Where(file => !file.Name.EndsWith(".meta")).ToArray(), ".png", og_texture_path, "png");

    }

    private void ApplyFileType(FileInfo[] files, string extension, string baseOutputPath, string type)
    {
        foreach (FileInfo file in files)
        {
            if (type == "json" && file.Name.Contains(".geo")) continue;
            string nameWithoutExt = file.Name.Replace(extension, "");
            BlockData blockData = Get_BlockData(nameWithoutExt);
            if (blockData == null) continue;

            string[] parts = blockData.namespaceId.Split('_');
            if (parts.Length < 1) continue;

            string saveFolder;

            if (type == "png")
            {
                List<string> result = new List<string>();
                if (parts.Length >= 2)
                {
                    if (!result.Contains(parts[0])) result.Add(parts[0]);
                    if (!result.Contains(parts[1])) result.Add(parts[1]);
                }
                else result.Add(blockData.namespaceId);

                saveFolder = baseOutputPath;
                foreach (string part in result)
                    saveFolder = Path.Combine(baseOutputPath, saveFolder, part);
                saveFolder = Path.Combine(saveFolder, "blocks");
            }
            else
            {
                List<string> result = new List<string>();
                if (parts.Length >= 2)
                {
                    if (!result.Contains(parts[0])) result.Add(parts[0]);
                    if (!result.Contains(parts[1])) result.Add(parts[1]);
                }
                else result.Add(blockData.namespaceId);

                saveFolder = baseOutputPath;
                foreach (string part in result)
                    saveFolder = Path.Combine(saveFolder, part);
            }

            if (!Directory.Exists(saveFolder))
            {
                if (File.Exists(saveFolder))
                {
                    Debug.LogError("Cannot create folder. A file with the same name exists: " + saveFolder);
                    continue;
                }
                Directory.CreateDirectory(saveFolder);
            }

            string fullFilePath = Path.Combine(saveFolder, file.Name);
            file.CopyTo(fullFilePath, true);
        }
    }
}
