using PimDeWitte.UnityMainThreadDispatcher;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class edit_scene : MonoBehaviour
{
    public static edit_scene instance;

    [SerializeField]
    private TextMeshProUGUI project_path;

    [SerializeField]
    private Button select_project_btn;

    [SerializeField]
    private Button reset_project_btn;

    [SerializeField]
    private TextMeshProUGUI preview_data;

    [SerializeField]
    public string block_path;

    [SerializeField]
    public Transform block_panel;

    [SerializeField]
    public Transform group_panel;

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
        reset_project_btn.onClick.AddListener(main.instance.Reset_Project);
        RemoveGroupContent();
        RemoveBlocksContent();
    }

    void Update()
    {
        
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
        DirectoryInfo block_dir = main.instance.FindBlockDir("");
        if(block_dir == null) return;
        main.instance.ClearExtractFolder();
        //List<GameObject> blocks = main.instance.CreateBLocksCentent(block_dir, block_panel, null, "");
        List<GameObject> groups = main.instance.CreateGroupsContent(block_dir, group_panel);

        //foreach (GameObject block in blocks)
        //{
        //    button button = block.GetComponent<button>();
        //    button.setup.Invoke();
        //}

        foreach (GameObject group in groups)
        {
            group_button group_Button = group.GetComponent<group_button>();
            group_Button.setup.Invoke();
            group_Button.my_button.onClick.AddListener(RemoveBlocksContent);
            group_Button.my_button.onClick.AddListener(() =>
            {
                DirectoryInfo block_dir1 = new DirectoryInfo(block_dir.FullName + group_Button.name);
                //main.instance.CreateBLocksCentent(block_dir1, block_panel, null, group_Button.name);
            });
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

    public void ShowBlockData(string name, string sub_folder)
    {
        string filePath = sub_folder.Length <= 0 ? Application.streamingAssetsPath + "/" + main.instance.blockPath + "/" + name :Application.streamingAssetsPath  + "/" + main.instance.blockPath + "/" + sub_folder + "/" + name;
        if (File.Exists(filePath))
        {
            string jsonText = File.ReadAllText(filePath);
            var json = JSON.Parse(jsonText);
            string version = json["format_version"];
            var block = json["minecraft:block"];
            string identifier = block["description"]?["identifier"] ?? "None";
            string geometry = block["components"]?["minecraft:geometry"]?["identifier"] ?? "None";
            string collision_box = block["components"]?["minecraft:collision_box"]?.ToString() ?? "None";
            string selection_box = block["components"]?["minecraft:selection_box"]?.ToString() ?? "None";
            preview_data.text = "Format version: " + version + "\n"
                + "Identifier : " + identifier + "\n"
                + "Geometry : " + geometry + "\n"
                + "Collision_box : " + collision_box + "\n"
                + "Selection_box : " + selection_box
                ;
        }
        else
        {
            Debug.LogError("not found!");
        }
    }

    public string GetBlockData(string name)
    {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + main.instance.blockPath);
        foreach (FileInfo file in dir.GetFiles())
        {
            Debug.Log(name + " : " + file.Name);
            if (name == file.Name)
            return file.Name;
        }

        return string.Empty;
    }
}
