using PimDeWitte.UnityMainThreadDispatcher;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Data;

public class edit_scene : MonoBehaviour
{
    public static edit_scene instance;

    [SerializeField]
    private TextMeshProUGUI project_path;

    [SerializeField]
    private TextMeshProUGUI csv_path;

    [SerializeField]
    private Button select_project_btn;

    [SerializeField]
    private Button reset_project_btn;

    [SerializeField]
    private Button reset_csv_btn;

    [SerializeField]
    private Button apply_csv_btn;

    [SerializeField]
    private Button select_csv_btn;

    [SerializeField]
    public Transform block_panel;

    [SerializeField]
    public Transform group_panel;

    [SerializeField]
    public Transform csv_panel;

    [SerializeField]
    public Transform column_panel;

    [SerializeField]
    public List<TMP_Dropdown> column_dropdowns;

    [SerializeField]
    public List<CSVData> CSV_Datas;

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
        project_path.text = main.instance.project_path;
        select_project_btn.onClick.AddListener(Select_Project);
        select_csv_btn.onClick.AddListener(Select_CSV);
        reset_project_btn.onClick.AddListener(main.instance.Reset_Project);
        reset_csv_btn.onClick.AddListener(ResetCSV);
        apply_csv_btn.onClick.AddListener(Load_CSVData);
    }

    void Update()
    {
        if (csv_path.text == string.Empty && column_panel.gameObject.activeSelf == true)
        {
            column_panel.gameObject.SetActive(false);
            csv_panel.gameObject.SetActive(true);
        }
        else if (csv_path.text != string.Empty && column_panel.gameObject.activeSelf == false)
        {
            column_panel.gameObject.SetActive(true);
            csv_panel.gameObject.SetActive(false);
        }
    }

    private void ResetCSV()
    {
        csv_path.text = string.Empty;
        CSV_Datas.Clear();
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
        //DirectoryInfo block_dir = main.instance.FindBlockDir("");
        //if(block_dir == null) return;
        //main.instance.ClearExtractFolder();
        //List<GameObject> blocks = main.instance.CreateBLocksCentent(block_dir, block_panel, null, "");
        //List<GameObject> groups = main.instance.CreateGroupsContent(block_dir, group_panel);

        //foreach (GameObject block in blocks)
        //{
        //    button button = block.GetComponent<button>();
        //    button.setup.Invoke();
        //}

        //foreach (GameObject group in groups)
        //{
        //    group_button group_Button = group.GetComponent<group_button>();
        //    group_Button.setup.Invoke();
        //    group_Button.my_button.onClick.AddListener(RemoveBlocksContent);
        //    group_Button.my_button.onClick.AddListener(() =>
        //    {
        //        DirectoryInfo block_dir1 = new DirectoryInfo(block_dir.FullName + group_Button.name);
        //        //main.instance.CreateBLocksCentent(block_dir1, block_panel, null, group_Button.name);
        //    });
        //}
    }

    private void Select_CSV()
    {
        csv_path.text = EditorUtility.OpenFilePanel("Select CSV file", "", "csv");

        if (string.IsNullOrEmpty(csv_path.text)) return;

        List<string[]> csvData = new List<string[]>();

        string[] lines = File.ReadAllLines(csv_path.text);

        if (lines.Length == 0)
        {
            Debug.LogWarning("CSV is empty.");
            csv_path.text = string.Empty;
            return;
        }

        int columnCount = lines[0].Split(',').Length;
        int[] nonEmptyCounts = new int[columnCount];

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            for (int i = 0; i < values.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(values[i]))
                {
                    nonEmptyCounts[i]++;
                }
            }
        }
        List<string> columns = new List<string>();
        for (int i = 0; i < columnCount; i++)
        {
            if (!columns.Contains(i.ToString()))
                columns.Add(i.ToString());
            Debug.Log($"Column {i} has data in {nonEmptyCounts[i]} row(s).");
        }
        foreach (TMP_Dropdown dropdown in column_dropdowns)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(columns);
        }
    }

    private void Load_CSVData()
    {
        if (string.IsNullOrEmpty(csv_path.text)) return;

        List<CSVData> itemList = new List<CSVData>();

        string[] lines = File.ReadAllLines(csv_path.text);
        if (lines.Length < 2) return;

        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            if (values.Length < headers.Length) continue;

            CSVData item = new CSVData
            {
                file_name = values[int.Parse(column_dropdowns[0].options[column_dropdowns[0].value].text)],
                identifier = values[int.Parse(column_dropdowns[1].options[column_dropdowns[1].value].text)],
                rotate_type = values[int.Parse(column_dropdowns[2].options[column_dropdowns[2].value].text)],
                render_method = values[int.Parse(column_dropdowns[3].options[column_dropdowns[3].value].text)],
                destroy_time = values[int.Parse(column_dropdowns[4].options[column_dropdowns[4].value].text)]
            };

            itemList.Add(item);
        }

        CSV_Datas = itemList;

        string prefix = itemList[1].identifier.Split(':')[0];
        string[] parts = prefix.Split('_');
        if (parts.Length < 1) return;
        string saveFolder = main.instance.og_block_path;

        List<string> result = new List<string>();
        if (parts.Length >= 2)
        {
            if (!result.Contains(parts[0])) result.Add(parts[0]);
            if (!result.Contains(parts[1])) result.Add(parts[1]);
        }
        else result.Add(prefix);

        foreach (string part in result)
            saveFolder = Path.Combine(saveFolder, part);

        DirectoryInfo directoryInfo = new DirectoryInfo(saveFolder);
        MatchBlockFile(directoryInfo, true);

        Block_Preview.instance.ClearModel(false);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
    private void MatchBlockFile(DirectoryInfo directoryInfo, bool createObj)
    {
        foreach (FileInfo jsonFile in directoryInfo.GetFiles("*.json"))
        {
            if (jsonFile.Name.EndsWith(".json") && !jsonFile.Name.EndsWith(".meta"))
            {
                string jsonNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonFile.Name);
                foreach (CSVData csvData in CSV_Datas)
                {
                    if (csvData.file_name == jsonNameWithoutExtension)
                    {
                        //Directory.CreateDirectory(Application.streamingAssetsPath + main.instance.blockPath + "/" + directoryInfo.Name.Replace("Extract", "") + "/" + jsonNameWithoutExtension);

                        string json_destinationPath = Path.Combine(directoryInfo.FullName, jsonFile.Name);

                        //if (!File.Exists(json_destinationPath))
                        //{
                        //    jsonFile.CopyTo(json_destinationPath);
                        //}

                        BlockData blockData = new BlockData();
                        blockData.blockName = jsonNameWithoutExtension;
                        blockData.format_Version = VersionData.versions[1];
                        blockData.namespaceId = jsonNameWithoutExtension.Split(':')[0];
                        blockData.geomerty = string.Empty;
                        blockData.texture = string.Empty;
                        blockData.file_path = json_destinationPath;
                        blockData.rotationType = csvData.rotate_type;
                        blockData.render_method = csvData.render_method;
                        blockData.destroy_time = float.Parse(csvData.destroy_time);
                        main.instance.all_blockData.Add(blockData);
                        JsonData.EditJsonField(json_destinationPath, blockData);

                        if (createObj)
                        {
                            main.instance.CreateBLockCentent(block_panel, blockData, "No_folder");
                        }
                    }
                }
            }
        }
    }

    public BlockData Get_SelectedBlockData(List<BlockData> blockDatas)
    {
        if (blockDatas == null || blockDatas.Count <= 0) return null;
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
}
