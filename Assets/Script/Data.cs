using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Data : MonoBehaviour
{
    public enum DataType
    {
        Version,
        Rotation,
        Collision,
        Render,
        NameSpace,
        Selection_Box_Origin,
        Selection_Box_Size,
        Destroy_Time
    }
    public static class VersionData
    {
        public static readonly string dispaly_name = "format version:";
        public static readonly List<string> versions = new List<string> { "---", "1.21.80", "1.1", "2.0" };
    }

    public static class CollisionData
    {
        public static readonly string dispaly_name = "collision box:";
        public static readonly List<string> value = new List<string> { "---", "true", "false" };
    }

    public static class RotationData
    {
        public static readonly string dispaly_name = "rotation type:";
        public static readonly List<string> types = new List<string> {  "---", "Default", "Cardinal", "Cardinal Facing", "Cardinal Block Face", "Cardinal Vertical Half","Facing","Block Face","Vertical Half" };
    }

    public static class RenderData
    {
        public static readonly string dispaly_name = "render method:";
        public static readonly List<string> types = new List<string> { "---", "alpha_test", "alpha_test_single_sided", "alpha_test_single_sided_to_opaque", "alpha_test_to_opaque", "blend", "blend_opaque", "double_sided", "opaque"  };
    }

    [System.Serializable]
    public class BlockData
    {
        public string blockName;
        public string namespaceId = "xxx";
        public string rotationType = "Cardinal";
        public string format_Version;
        public string collision = "true";
        public string render_method = "alpha_test";
        public string geomerty;
        public string texture;
        public float destroy_time = 0.5f;
        public Vector3 selectionBox_origin;
        public Vector3 selectionBox_size;
        public string file_path;

        public string Identifier => $"{namespaceId}:{blockName}";
    }

    [System.Serializable]
    public class CSVData
    {
        public string file_name;
        public string identifier;
        public string rotate_type;
        public string render_method;
        public string destroy_time;
    }
}
