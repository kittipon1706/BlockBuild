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
using System.Globalization;

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
    private Button apply_btn;

    [SerializeField]
    private Button selectAll_btn;

    [SerializeField]
    private bool selectAll_state;

    [SerializeField]
    private Button reset_filter_btn;

    [SerializeField]
    private Button default_filter_btn;

    [SerializeField]
    private Button cardinal_filter_btn;

    [SerializeField]
    private Button cardinal_f_filter_btn;

    [SerializeField]
    private Button cardinal_b_filter_btn;

    [SerializeField]
    private Button cardinal_v_filter_btn;

    [SerializeField]
    private Button facing_filter_btn;

    [SerializeField]
    private Button block_f_filter_btn;

    [SerializeField]
    private Button vertical_h_filter_btn;

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
    private TMP_InputField search_input;

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

    [SerializeField]
    private Slider destroy_time;

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
        selectAll_state = false;
        search_input.onValueChanged.AddListener((string input) => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(input, string.Empty, string.Empty, block_panel, null); });
        select_project_btn.onClick.AddListener(Select_Project);
        reset_project_btn.onClick.AddListener(main.instance.Reset_Project);
        select_asset_btn.onClick.AddListener(Select_Asset);
        apply_btn.onClick.AddListener(main.instance.ApplyAllToProject);
        reset_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[0], block_panel,
        new ToggleButtonColor[] { default_filter_btn.GetComponent<ToggleButtonColor>(),
        cardinal_filter_btn.GetComponent<ToggleButtonColor>(),
        cardinal_f_filter_btn.GetComponent<ToggleButtonColor>(),
        cardinal_b_filter_btn.GetComponent<ToggleButtonColor>(),
        cardinal_v_filter_btn.GetComponent<ToggleButtonColor>(),
        facing_filter_btn.GetComponent<ToggleButtonColor>(),
        block_f_filter_btn.GetComponent<ToggleButtonColor>(),
        vertical_h_filter_btn.GetComponent<ToggleButtonColor>() });
        });
        reset_filter_btn.onClick.Invoke();
        default_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[1], block_panel, new ToggleButtonColor[] { default_filter_btn.GetComponent<ToggleButtonColor>() }); });
        cardinal_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[2], block_panel, new ToggleButtonColor[] { cardinal_filter_btn.gameObject.GetComponent<ToggleButtonColor>() }); });
        cardinal_f_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[3], block_panel, new ToggleButtonColor[] { cardinal_f_filter_btn.GetComponent<ToggleButtonColor>() }); });
        cardinal_b_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[4], block_panel, new ToggleButtonColor[] { cardinal_b_filter_btn.GetComponent<ToggleButtonColor>() }); });
        cardinal_v_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[5], block_panel, new ToggleButtonColor[] { cardinal_v_filter_btn.GetComponent<ToggleButtonColor>() }); });
        facing_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[6], block_panel, new ToggleButtonColor[] { facing_filter_btn.GetComponent<ToggleButtonColor>() }); });
        block_f_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[7], block_panel, new ToggleButtonColor[] { block_f_filter_btn.GetComponent<ToggleButtonColor>() }); });
        vertical_h_filter_btn.onClick.AddListener(() => { main.instance.SetAll_HoldBlackData(false); main.instance.Show_BlockBtn(search_input.text, string.Empty, RotationData.types[8], block_panel, new ToggleButtonColor[] { vertical_h_filter_btn.GetComponent<ToggleButtonColor>() }); });
        selectAll_btn.onClick.AddListener(() => {
            if(selectAll_state == false)
                main.instance.SetAll_HoldBlackData(true);
            else
                main.instance.SetAll_HoldBlackData(false);
            selectAll_state = !selectAll_state;
        });
        namespace_input.onEndEdit.AddListener((string value) => {
            List<BlockData> blockDatas = main.instance.Get_AllHoldBlackData();
            foreach (BlockData blockData in blockDatas)
            {
                main.instance.Set_BlockData(blockData.blockName, DataType.NameSpace, value , 0.0f, Vector3.zero);
            }
            Get_SelectedBlockData(blockDatas);
        });

        destroy_time.onValueChanged.AddListener((float value) =>
        {
            List<BlockData> blockDatas = main.instance.Get_AllHoldBlackData();
            foreach (BlockData blockData in blockDatas)
            {
                main.instance.Set_BlockData(blockData.blockName, DataType.Destroy_Time, string.Empty, value, Vector3.zero);
            }
            Get_SelectedBlockData(blockDatas);
        });
        RemoveGroupContent();
        RemoveBlocksContent();
    }


    private void Select_Project()
    {
#if UNITY_EDITOR
        project_path.text = EditorUtility.OpenFolderPanel("Select resouce folder", "", "");
        main.instance.project_path = project_path.text;
        main.instance.FindBlockDir();
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
                MatchBlockFile(sub_folder_directoryInfo, true);
            }

            GameObject no_group = main.instance.CreateGroupContent("No Folder", group_panel);
            no_group.transform.SetAsFirstSibling();
            group_button no_group_Button = no_group.GetComponent<group_button>();
            no_group_Button.setup.Invoke();
            no_group_Button.my_button.onClick.AddListener(() =>
            {
                main.instance.SetAll_HoldBlackData(false);
                main.instance.Show_BlockBtn(search_input.text, "No_folder", string.Empty, block_panel, null);
            });

            GameObject all_group = main.instance.CreateGroupContent("All", group_panel);
            all_group.transform.SetAsFirstSibling();
            group_button all_group_Button = all_group.GetComponent<group_button>();
            all_group_Button.setup.Invoke();
            all_group_Button.my_button.onClick.AddListener(() =>
            {
                main.instance.SetAll_HoldBlackData(false);
                main.instance.Show_BlockBtn(search_input.text, string.Empty, string.Empty, block_panel, null);
            });

            List<GameObject> groups = main.instance.CreateGroupsContent(asset_directoryInfo, group_panel);
            foreach (GameObject group in groups)
            {
                group_button group_Button = group.GetComponent<group_button>();
                group_Button.setup.Invoke();
                group_Button.my_button.onClick.AddListener(() =>
                {
                    main.instance.SetAll_HoldBlackData(false);
                    main.instance.Show_BlockBtn(search_input.text, group_Button.name, string.Empty, block_panel, null);
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
                        blockData.file_path = geo_destinationPath.Replace(".geo", "");
                        blockData.destroy_time = 0.5f;
                        main.instance.all_blockData.Add(blockData);
                        GameObject model_obj = Block_Preview.instance.LoadGeoModel(jsonNameWithoutExtension);
                        main.instance.Calculate_Selection_Box(jsonNameWithoutExtension, model_obj);
                        JsonData.SaveToFile(blockData.file_path, blockData);

                        if (createObj)
                        {
                            string basePath = Application.streamingAssetsPath + main.instance.extractPath;
                            string fullPath = Path.GetFullPath(directoryInfo.FullName).Replace("\\", "/");

                            string relativePath = fullPath.StartsWith(basePath)
                                ? fullPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).TrimEnd('/', '\\')

                                : fullPath;

                            if (relativePath.Length <= 0) relativePath = "No_folder";
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
                namespaceId = blockDatas.All(b => b.namespaceId == first.namespaceId) ? first.namespaceId : "xxx",
                format_Version = blockDatas.All(b => b.format_Version == first.format_Version) ? first.format_Version : Data.VersionData.versions[0],
                geomerty = blockDatas.All(b => b.geomerty == first.geomerty) ? first.geomerty : null,
                texture = blockDatas.All(b => b.texture == first.texture) ? first.texture : null,
                render_method = blockDatas.All(b => b.render_method == first.render_method) ? first.render_method : Data.RenderData.types[0],
                destroy_time = blockDatas.All(b => b.destroy_time == first.destroy_time) ? first.destroy_time : 0.5f,
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
        destroy_time.value = blockData.destroy_time;
        UnityMainThreadDispatcher.Instance().Enqueue(() => namespace_input.text = blockData.namespaceId);
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
            namespaceId = "xxx",
            format_Version = Data.VersionData.versions[0],
            geomerty = string.Empty,
            texture = string.Empty,
            render_method =Data.RenderData.types[0],
            destroy_time = 0.5f,
            selectionBox_origin = Vector3.zero,
            selectionBox_size =  Vector3.zero,
            collision = Data.CollisionData.value[0],
            rotationType = Data.RotationData.types[0]
        };
    Show_SelectedBlockData(result);
    }
}
