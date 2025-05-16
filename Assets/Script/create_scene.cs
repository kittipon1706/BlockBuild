using PimDeWitte.UnityMainThreadDispatcher;
using Siccity.GLTFUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Data;

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
    private Transform model_panel;

    [SerializeField]
    private Data.BlockData SelectedBlock;

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
            SetSelectedBlockData();
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
            DirectoryInfo directoryInfo = new DirectoryInfo(asset_path.text);
            foreach (DirectoryInfo diSourceSubDir in directoryInfo.GetDirectories())
            {
                Directory.CreateDirectory(UnityEngine.Application.streamingAssetsPath + main.instance.blockPath + "/" + diSourceSubDir.Name);
            }
            main.instance.LoadBlockAsset(directoryInfo);
            MatchBlockFile();
            List<GameObject> groups = main.instance.CreateGroupsContent(directoryInfo, group_panel);
            groups.Add(main.instance.CreateGroupContent("", group_panel));
            foreach (GameObject group in groups)
            {
                group_button group_Button = group.GetComponent<group_button>();
                group_Button.setup.Invoke();
                group_Button.my_button.onClick.AddListener(RemoveBlocksContent);
                group_Button.my_button.onClick.AddListener(() =>
                {
                    DirectoryInfo block_dir1 = new DirectoryInfo(UnityEngine.Application.streamingAssetsPath + main.instance.blockPath + "/" + group_Button.name);
                    main.instance.CreateBLocksCentent(block_dir1, block_panel, null, group_Button.name);
                });
            }


            DirectoryInfo directoryInfo1 = new DirectoryInfo((UnityEngine.Application.streamingAssetsPath + main.instance.blockPath));
            main.instance.CreateBLocksCentent(directoryInfo1, block_panel, null, "");
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }

    private void MatchBlockFile()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(UnityEngine.Application.streamingAssetsPath + main.instance.extractPath);
        foreach (DirectoryInfo SubDir in directoryInfo.GetDirectories())
        {
            FileInfo[] pngFiles = SubDir.GetFiles("*.png");
            
            foreach (FileInfo jsonFile in SubDir.GetFiles("*.json"))
            {
                if (jsonFile.Name.EndsWith(".json") && !jsonFile.Name.EndsWith(".meta"))
                {
                    string jsonNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonFile.Name);
                    jsonNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonNameWithoutExtension);

                    foreach (FileInfo png in pngFiles)
                    {
                        string pngNameWithoutExtension = Path.GetFileNameWithoutExtension(png.Name);
                        pngNameWithoutExtension = Path.GetFileNameWithoutExtension(pngNameWithoutExtension);

                        if (jsonNameWithoutExtension.StartsWith(pngNameWithoutExtension))
                        {
                            Directory.CreateDirectory(UnityEngine.Application.streamingAssetsPath + main.instance.blockPath + "/" + SubDir.Name + "/" + jsonNameWithoutExtension);

                            string geo_destinationPath = Path.Combine(UnityEngine.Application.streamingAssetsPath + main.instance.blockPath + "/" + SubDir.Name + "/" + jsonNameWithoutExtension, jsonFile.Name);

                            if (!File.Exists(geo_destinationPath))
                            {
                                jsonFile.CopyTo(geo_destinationPath);
                            }

                            string png_destinationPath = Path.Combine(UnityEngine.Application.streamingAssetsPath + main.instance.blockPath + "/" + SubDir.Name + "/" + jsonNameWithoutExtension, png.Name);

                            if (!File.Exists(png_destinationPath))
                            {
                                png.CopyTo(png_destinationPath);
                            }

                            //FileInfo gltfFile = SubDir.GetFiles("*.gltf")
                            //.FirstOrDefault(file => Path.GetFileNameWithoutExtension(file.Name) == jsonNameWithoutExtension);
                            //if (gltfFile != null)
                            //{
                            //    string gltf_destinationPath = Path.Combine(UnityEngine.Application.streamingAssetsPath + main.instance.blockPath + "/" + SubDir.Name + "/" + jsonNameWithoutExtension, gltfFile.Name);

                            //    if (!File.Exists(gltf_destinationPath))
                            //    {
                            //        gltfFile.CopyTo(gltf_destinationPath);
                            //    }
                            //}



                            Data.BlockData blockData = new Data.BlockData();
                            blockData.blockName = jsonFile.Name.Replace(".geo.json","");
                            blockData.format_Version = Data.VersionData.versions[0];
                            blockData.geomerty = jsonFile.FullName;
                            blockData.texture = png.FullName;
                            main.instance.all_blockData.Add(blockData);
                            GameObject model_obj = Block_Preview.instance.LoadGeoModel(jsonNameWithoutExtension);
                            main.instance.Calculate_Selection_Box(jsonNameWithoutExtension, model_obj);
                            JsonData.SaveToFile(Path.Combine(UnityEngine.Application.streamingAssetsPath + main.instance.modelPath, blockData.blockName + ".json"), blockData);
                        }
                    }
                }
            }
        }
        main.instance.RemoveAllContent(model_panel);
    }

    public void RemoveGroupContent()
    {
        main.instance.RemoveAllContent(group_panel);
    }

    public void RemoveBlocksContent()
    {
        main.instance.RemoveAllContent(block_panel);
    }

    public void GetSelectedBlockData(string name)
    {
        BlockData result = main.instance.all_blockData.Find(b => b.blockName == name.Replace(".geo.json",""));

        if (result != null)
        {
            SelectedBlock = result;
            UnityMainThreadDispatcher.Instance().Enqueue(() => name_txt.text = name.Replace(".geo.json", ""));
            version_content.content_dropdown.options.FindLast(option => option.text == result.format_Version);
            collision_content.content_dropdown.options.FindLast(option => option.text == result.collision.ToString());
            rotaion_content.content_dropdown.options.FindLast(option => option.text == result.rotationType);
            render_content.content_dropdown.options.FindLast(option => option.text == result.render_method);
            Vector3 offset = Vector3.zero;
            offset.x = main.instance.SmartRound(result.selectionBox_origin.x + (result.selectionBox_size.x / 2f)); 
            offset.z = main.instance.SmartRound(result.selectionBox_origin.z + (result.selectionBox_size.z / 2f));
            offset_Box.SetValue(offset);
            size_Box.SetValue(result.selectionBox_size);
        }
        else
        {
            Debug.Log("Fail");
        }
    }

    public void SetSelectedBlockData()
    {
        int result = main.instance.all_blockData.FindIndex(b => b.blockName == SelectedBlock.blockName.Replace(".geo.json", ""));
        Debug.Log(result);
        if (result != -1)
        {
            main.instance.all_blockData[result].format_Version = version_content.selectedData;
            main.instance.all_blockData[result].namespaceId = namespace_input.text;
            main.instance.all_blockData[result].collision = Convert.ToBoolean(collision_content.selectedData);
            main.instance.all_blockData[result].rotationType = rotaion_content.selectedData;
            main.instance.all_blockData[result].render_method = render_content.selectedData;
            Vector3 offset = offset_Box.GetValue();
            Vector3 size = size_Box.GetValue();
            offset.x = main.instance.SmartRound((-size.x / 2f) + offset.x);
            offset.z = main.instance.SmartRound((-size.z / 2f) + offset.z);
            main.instance.all_blockData[result].selectionBox_origin = offset;
            main.instance.all_blockData[result].selectionBox_size = size;
            GetSelectedBlockData(main.instance.all_blockData[result].blockName);
        }
        else
        {
            Debug.Log("Fail");
        }
    }

    public void SetAllBlockData(DataType dataType, string value)
    {
        int result = main.instance.all_blockData.FindIndex(b => b.blockName == SelectedBlock.blockName.Replace(".geo.json", ""));
        Debug.Log(result);
        if (result != -1)
        {
            main.instance.all_blockData[result].format_Version = version_content.selectedData;
            main.instance.all_blockData[result].namespaceId = namespace_input.text;
            main.instance.all_blockData[result].collision = Convert.ToBoolean(collision_content.selectedData);
            main.instance.all_blockData[result].rotationType = rotaion_content.selectedData;
            main.instance.all_blockData[result].render_method = render_content.selectedData;
            Vector3 offset = offset_Box.GetValue();
            Vector3 size = size_Box.GetValue();
            offset.x = main.instance.SmartRound((-size.x / 2f) + offset.x);
            offset.z = main.instance.SmartRound((-size.z / 2f) + offset.z);
            main.instance.all_blockData[result].selectionBox_origin = offset;
            main.instance.all_blockData[result].selectionBox_size = size;
            GetSelectedBlockData(main.instance.all_blockData[result].blockName);
        }
        else
        {
            Debug.Log("Fail");
        }
        //foreach (BlockData blockData of main.instance.all_blockData){

        //}
    }
}
