using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using static Data;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class create_scene : MonoBehaviour
{
    public static create_scene instance;

    [SerializeField]
    private Button select_project_btn;

    [SerializeField]
    private Button reset_project_btn;

    [SerializeField]
    private Button select_asset_btn;

    [SerializeField]
    private TextMeshProUGUI project_path;

    [SerializeField]
    private TextMeshProUGUI asset_path;

    [SerializeField]
    public Transform block_panel;

    [SerializeField]
    public Transform group_panel;

    [SerializeField]
    private TMP_InputField namespace_input;

    [SerializeField]
    private TextMeshProUGUI name_txt;

    [SerializeField]
    private Content version_content;

    [SerializeField]
    private Content collision_content;

    [SerializeField]
    private Content rotaion_content;

    [SerializeField]
    private Content render_content;

    [SerializeField]
    private Selection_box offset_Box;

    [SerializeField]
    private Selection_box size_Box;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        select_project_btn.onClick.AddListener(Select_Project);
        reset_project_btn.onClick.AddListener(main.instance.Reset_Project);
        select_asset_btn.onClick.AddListener(Select_Asset);
        namespace_input.onEndEdit.AddListener((string value) => {
            foreach (BlockData blockData in main.instance.Get_AllHoldBlackData())
            {
                main.instance.Set_BlockData(blockData.blockName, DataType.NameSpace, value, Vector3.zero);
            }
            Get_SelectedBlockData(main.instance.Get_AllHoldBlackData());
        });
        RemoveGroupContent();
        RemoveBlocksContent();
    }


    private void Select_Project()
    {
#if UNITY_EDITOR
        project_path.text = EditorUtility.OpenFolderPanel("Select resouce folder", "", "");
        main.instance.project_path = project_path.text;
#endif
#if !UNITY_EDITOR

        var paths = StandaloneFileBrowser.OpenFolderPanel("Select resouce folder", "", false);

        if (paths.Length > 0)
        {
            file_path.text = paths[0];
        }
#endif
    }

    private void Select_Asset()
    {
        {
#if UNITY_EDITOR
            asset_path.text = EditorUtility.OpenFolderPanel("Select resouce folder", "", "");
#endif
#if !UNITY_EDITOR

        var paths = StandaloneFileBrowser.OpenFolderPanel("Select resouce folder", "", false);

        if (paths.Length > 0)
        {
            file_path.text = paths[0];
        }
#endif
            DirectoryInfo asset_directoryInfo = new DirectoryInfo(asset_path.text);
            main.instance.LoadBlockAsset(asset_directoryInfo);
            DirectoryInfo no_folder_directoryInfo = new DirectoryInfo(Application.streamingAssetsPath + main.instance.extractPath);
            MatchBlockFile(no_folder_directoryInfo, true);
            DirectoryInfo subs_folder_directoryInfo = new DirectoryInfo(Application.streamingAssetsPath + main.instance.extractPath);
            foreach (DirectoryInfo sub_folder_directoryInfo in subs_folder_directoryInfo.GetDirectories())
            {
                MatchBlockFile(sub_folder_directoryInfo, false);
            }
            List<GameObject> groups = main.instance.CreateGroupsContent(asset_directoryInfo, group_panel);
            groups.Add(main.instance.CreateGroupContent("", group_panel));
            foreach (GameObject group in groups)
            {
                group_button group_Button = group.GetComponent<group_button>();
                group_Button.setup.Invoke();
                group_Button.my_button.onClick.AddListener(RemoveBlocksContent);
                group_Button.my_button.onClick.AddListener(() =>
                {
                    DirectoryInfo blocks_dirs = new DirectoryInfo(Application.streamingAssetsPath + main.instance.blockPath + "/" + group_Button.name);
                    foreach(DirectoryInfo blocks_dir in blocks_dirs.GetDirectories())
                    {
                        FileInfo[] jsonFiles = blocks_dir.GetFiles("*.geo.json");
                        foreach (FileInfo jsonFile in jsonFiles)
                        {
                            main.instance.CreateBLockCentent(block_panel, main.instance.Get_BlockData(jsonFile.Name.Replace(".geo.json", "")), group_Button.name);
                        }
                    }
                });
            }
            Block_Preview.instance.ClearModel(false);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }

    private void MatchBlockFile(DirectoryInfo directoryInfo, bool createObj)
    {
        FileInfo[] pngFiles = directoryInfo.GetFiles("*.png");

        foreach (FileInfo jsonFile in directoryInfo.GetFiles("*.geo.json"))
        {
            if (jsonFile.Name.EndsWith(".geo.json") && !jsonFile.Name.EndsWith(".meta"))
            {
                string jsonNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonFile.Name);
                jsonNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonNameWithoutExtension);

                foreach (FileInfo png in pngFiles)
                {
                    string pngNameWithoutExtension = Path.GetFileNameWithoutExtension(png.Name);
                    pngNameWithoutExtension = Path.GetFileNameWithoutExtension(pngNameWithoutExtension);

                    if (jsonNameWithoutExtension.StartsWith(pngNameWithoutExtension))
                    {
                        Directory.CreateDirectory(Application.streamingAssetsPath + main.instance.blockPath + "/" + directoryInfo.Name.Replace("Extract","") + "/" + jsonNameWithoutExtension);

                        string geo_destinationPath = Path.Combine(Application.streamingAssetsPath + main.instance.blockPath + "/" + directoryInfo.Name.Replace("Extract", "") + "/" + jsonNameWithoutExtension, jsonFile.Name);

                        if (!File.Exists(geo_destinationPath))
                        {
                            jsonFile.CopyTo(geo_destinationPath);
                        }

                        string png_destinationPath = Path.Combine(Application.streamingAssetsPath + main.instance.blockPath + "/" + directoryInfo.Name.Replace("Extract", "") + "/" + jsonNameWithoutExtension, png.Name);

                        if (!File.Exists(png_destinationPath))
                        {
                            png.CopyTo(png_destinationPath);
                        }

                        Data.BlockData blockData = new Data.BlockData();
                        blockData.blockName = jsonFile.Name.Replace(".geo.json", "");
                        blockData.format_Version = Data.VersionData.versions[1];
                        blockData.geomerty = jsonFile.FullName;
                        blockData.texture = png.FullName;
                        main.instance.all_blockData.Add(blockData);
                        GameObject model_obj = Block_Preview.instance.LoadGeoModel(jsonNameWithoutExtension);
                        main.instance.Calculate_Selection_Box(jsonNameWithoutExtension, model_obj);
                        JsonData.SaveToFile(geo_destinationPath.Replace(".geo", ""), blockData);

                        if (createObj)
                        {
                            string basePath = Application.streamingAssetsPath + main.instance.extractPath;
                            string fullPath = Path.GetFullPath(directoryInfo.FullName).Replace("\\", "/");

                            string relativePath = fullPath.StartsWith(basePath)
                                ? fullPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).TrimEnd('/', '\\')

                                : fullPath;
                            main.instance.CreateBLockCentent(block_panel, blockData, relativePath);
                        }
                    }
                }
            }
        }
    }

    public void RemoveGroupContent()
    {
        main.instance.RemoveAllContent(group_panel);
    }

    public void RemoveBlocksContent()
    {
        main.instance.RemoveAllContent(block_panel);
    }

    public BlockData Get_SelectedBlockData(List<BlockData> blockDatas)
    {
        if(blockDatas == null || blockDatas.Count <= 0) return null;
        BlockData result = new BlockData();
        if (blockDatas.Count == 1)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => name_txt.text = blockDatas[0].blockName.Replace(".geo.json", ""));
            result = blockDatas[0];
        }
        else
        {
            if (blockDatas == null || blockDatas.Count == 0)
                return null;

            BlockData first = blockDatas[0];

            result = new BlockData
            {
                blockName = blockDatas.All(b => b.blockName == first.blockName) ? first.blockName : null,
                format_Version = blockDatas.All(b => b.format_Version == first.format_Version) ? first.format_Version : Data.VersionData.versions[0],
                geomerty = blockDatas.All(b => b.geomerty == first.geomerty) ? first.geomerty : null,
                texture = blockDatas.All(b => b.texture == first.texture) ? first.texture : null,
                render_method = blockDatas.All(b => b.render_method == first.render_method) ? first.render_method : Data.RenderData.types[0],
                destroy_time = blockDatas.All(b => b.destroy_time == first.destroy_time) ? first.destroy_time : null,
                selectionBox_origin = blockDatas.All(b => b.selectionBox_origin == first.selectionBox_origin) ? first.selectionBox_origin : Vector3.zero,
                selectionBox_size = blockDatas.All(b => b.selectionBox_size == first.selectionBox_size) ? first.selectionBox_size : Vector3.zero,
                collision = blockDatas.All(b => b.collision == first.collision) ? first.collision : Data.CollisionData.value[0],
                rotationType = blockDatas.All(b => b.rotationType == first.rotationType) ? first.rotationType : Data.RotationData.types[0]
            };
        }
        Show_SelectedBlockData(result);
        return result;
    }

    private void Show_SelectedBlockData(BlockData blockData)
    {
        int versionIndex = version_content.content_dropdown.options
       .FindIndex(option => option.text == blockData.format_Version);
        if (versionIndex >= 0)
            version_content.content_dropdown.value = versionIndex;

        int collisionIndex = collision_content.content_dropdown.options
            .FindIndex(option => option.text == blockData.collision);
        if (collisionIndex >= 0)
            collision_content.content_dropdown.value = collisionIndex;

        int rotationIndex = rotaion_content.content_dropdown.options
            .FindIndex(option => option.text == blockData.rotationType);
        if (rotationIndex >= 0)
            rotaion_content.content_dropdown.value = rotationIndex;

        int renderIndex = render_content.content_dropdown.options
            .FindIndex(option => option.text == blockData.render_method);
        if (renderIndex >= 0)
            render_content.content_dropdown.value = renderIndex;

        namespace_input.text = blockData.namespaceId;
        size_Box.SetValue(blockData.selectionBox_size);
        Vector3 offset = Vector3.zero;
        offset.x = main.instance.SmartRound(blockData.selectionBox_origin.x + (blockData.selectionBox_size.x / 2f));
        offset.z = main.instance.SmartRound(blockData.selectionBox_origin.z + (blockData.selectionBox_size.z / 2f));
        offset_Box.SetValue(offset);
    }

    public void ShowDefualt()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => name_txt.text = "None");

        BlockData result = new BlockData
        {
            blockName = string.Empty,
            format_Version = Data.VersionData.versions[0],
            geomerty = string.Empty,
            texture = string.Empty,
            render_method =Data.RenderData.types[0],
            destroy_time = string.Empty,
            selectionBox_origin = Vector3.zero,
            selectionBox_size =  Vector3.zero,
            collision = Data.CollisionData.value[0],
            rotationType = Data.RotationData.types[0]
        };
    Show_SelectedBlockData(result);
    }
}
