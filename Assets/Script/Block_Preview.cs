using Newtonsoft.Json;
using Siccity.GLTFUtility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Data;
using static Unity.Collections.AllocatorManager;

public class Block_Preview : MonoBehaviour
{
    public static Block_Preview instance;

    [SerializeField]
    private Slider degree_slider;

    [SerializeField]
    private Slider scale_slider;

    [SerializeField]
    private Camera preview_camera;

    [SerializeField]
    private Transform model_panel;

    [SerializeField]
    private GameObject default_obj;

    [SerializeField]
    private GameObject model_obj;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        degree_slider.value = 0;
        ClearModel(true);
        degree_slider.onValueChanged.AddListener(RotateModel);
        scale_slider.onValueChanged.AddListener(ChangeScale);
    }

    void Update()
    {
        
    }
    private void RotateModel(float angle)
    {
        if (model_obj == null) return;
        float degree = angle * 360f;
        model_obj.transform.rotation = Quaternion.Euler(0f, degree, 0f);

    }
    private void ChangeScale(float scale)
    {
        if (preview_camera == null) return;
        preview_camera.orthographicSize = 1 - scale;

    }

    public void ClearModel(bool defualtSpawn)
    {
        main.instance.RemoveAllContent(model_panel);
        if (defualtSpawn)
            Instantiate(default_obj, model_panel);
    }

    public GameObject LoadGeoModel(string name)
    {
        ClearModel(false);
        BlockData result = main.instance.all_blockData.Find(block => block.blockName == name);
        if (result == null) {
            Debug.LogError("can not find BlockData");
            return null;
        }
        string geo_path = result.geomerty;
        string png_path = result.texture;
        if (!File.Exists(geo_path))
        {
            Debug.LogError("Geo JSON file not found");
            return null;
        }

        string jsonText = File.ReadAllText(geo_path);
      
        GeoJsonRoot data = JsonConvert.DeserializeObject<GeoJsonRoot>(jsonText);

        byte[] fileData = File.ReadAllBytes(png_path);
        Texture2D blockTexture = new Texture2D(2, 2); 
        bool success = blockTexture.LoadImage(fileData);

        if(!success) return null;
        if (blockTexture.width <= 1 || blockTexture.height <= 1)
        {
            Debug.LogError("!texture");
            return null;
        }
        Material blockMat = CreateMaterial(blockTexture);
        blockMat.mainTexture = blockTexture;

        if (data?.minecraft_geometry == null || data.minecraft_geometry.Count == 0)
        {
            Debug.LogError("!geometry");
            return null;
        }

        MinecraftGeometry geometry = data.minecraft_geometry[0];

        GameObject root = new GameObject(name);

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        Vector3 se_min = Vector3.positiveInfinity;
        Vector3 se_max = Vector3.negativeInfinity;

        foreach (var bone in geometry.bones)
        {
            if (bone.cubes == null) continue;

            foreach (var cube in bone.cubes)
            {
                Vector3 origin = new Vector3(cube.origin[0], cube.origin[1], cube.origin[2]);
                Vector3 size = new Vector3(cube.size[0], cube.size[1], cube.size[2]);
                Vector3 cubeMin = origin;
                Vector3 cubeMax = origin + size;

                min = Vector3.Min(min, cubeMin);
                max = Vector3.Max(max, cubeMax);
            }
        }

        Vector3 visibleBoundsOffset = new Vector3(
            geometry.description.visible_bounds_offset[0],
            geometry.description.visible_bounds_offset[1],
            geometry.description.visible_bounds_offset[2]);

        min += visibleBoundsOffset;
        max += visibleBoundsOffset;

        se_min = min / 16f;
        se_max = max / 16f;

        Vector3 selectionCenter = (se_min + se_max) / 2f;
        Vector3 selectionSize = se_max - se_min;
        selectionCenter.y -= 0.03125f;
        Vector3 modelCenterOffset = (min + max) / 2f / 16f;
        foreach (var bone in geometry.bones)
        {
            if (bone.cubes == null) continue;

            foreach (var cube in bone.cubes)
            {
                Mesh cubeMesh = CreateCubeMeshWithUV(cube, blockTexture);
                GameObject go = new GameObject("cube");
                go.transform.SetParent(root.transform);

                MeshFilter mf = go.AddComponent<MeshFilter>();
                mf.mesh = cubeMesh;

                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.material = blockMat;

                go.transform.localPosition = Vector3.zero;
            }
        }

        if (root)
        {
            root.transform.SetParent(model_panel, false);
            root.transform.localPosition = -modelCenterOffset;
            model_obj = root;
            BoxCollider box = root.AddComponent<BoxCollider>();
            box.center = selectionCenter;
            box.size = selectionSize;
        }
        return root;
    }

    Material CreateMaterial(Texture2D texture)
    {
        Material mat = new Material(Shader.Find("Custom/PixelSnapShader"));
        mat.mainTexture = texture;
        return mat;
    }

    Mesh CreateCubeMeshWithUV(Cube cubeData, Texture2D texture)
    {
        Mesh mesh = new Mesh();

        Vector3 size = new Vector3(cubeData.size[0], cubeData.size[1], cubeData.size[2]) / 16f;

        Vector3 origin = new Vector3(cubeData.origin[0], cubeData.origin[1], cubeData.origin[2]) / 16f;

        Vector3[] vertices = new Vector3[24];

        Vector2[] uvs = new Vector2[24];

        // Front (north) - Z+
        vertices[0] = origin + new Vector3(0, 0, size.z);
        vertices[1] = origin + new Vector3(0, size.y, size.z);
        vertices[2] = origin + new Vector3(size.x, size.y, size.z);
        vertices[3] = origin + new Vector3(size.x, 0, size.z);

        // Back (south) - Z-
        vertices[4] = origin + new Vector3(size.x, 0, 0);
        vertices[5] = origin + new Vector3(size.x, size.y, 0);
        vertices[6] = origin + new Vector3(0, size.y, 0);
        vertices[7] = origin + new Vector3(0, 0, 0);

        // Left (west) - X-
        vertices[8] = origin + new Vector3(0, 0, 0);
        vertices[9] = origin + new Vector3(0, size.y, 0);
        vertices[10] = origin + new Vector3(0, size.y, size.z);
        vertices[11] = origin + new Vector3(0, 0, size.z);

        // Right (east) - X+
        vertices[12] = origin + new Vector3(size.x, 0, size.z);
        vertices[13] = origin + new Vector3(size.x, size.y, size.z);
        vertices[14] = origin + new Vector3(size.x, size.y, 0);
        vertices[15] = origin + new Vector3(size.x, 0, 0);

        // Top (up) - Y+
        vertices[16] = origin + new Vector3(0, size.y, size.z);
        vertices[17] = origin + new Vector3(0, size.y, 0);
        vertices[18] = origin + new Vector3(size.x, size.y, 0);
        vertices[19] = origin + new Vector3(size.x, size.y, size.z);

        // Bottom (down) - Y-
        vertices[20] = origin + new Vector3(0, 0, 0);
        vertices[21] = origin + new Vector3(0, 0, size.z);
        vertices[22] = origin + new Vector3(size.x, 0, size.z);
        vertices[23] = origin + new Vector3(size.x, 0, 0);

        Vector2 ToUV(float x, float y)
        {
            return new Vector2(x / texture.width, 1f - y / texture.height);
        }

        void SetFaceUV(FaceUV face, int startIndex)
        {
            if (face == null || face.uv == null || face.uv_size == null) return;

            float x1 = face.uv[0];
            float y1 = face.uv[1];
            float x2 = x1 + face.uv_size[0];
            float y2 = y1 + face.uv_size[1];

            float xMin = Mathf.Min(x1, x2);
            float xMax = Mathf.Max(x1, x2);
            float yMin = Mathf.Min(y1, y2);
            float yMax = Mathf.Max(y1, y2);

            Vector2 uv00 = ToUV(xMax, yMax);
            Vector2 uv01 = ToUV(xMax, yMin);
            Vector2 uv11 = ToUV(xMin, yMin);
            Vector2 uv10 = ToUV(xMin, yMax);

            Vector2[] faceUVs;
            int rotation = face.uv_rotation.HasValue ? face.uv_rotation.Value % 360 : 0;

            switch (rotation)
            {
                case 0:
                    faceUVs = new Vector2[] { uv00, uv01, uv11, uv10 };
                    break;
                case 90:
                    faceUVs = new Vector2[] { uv10, uv00, uv01, uv11 };
                    break;
                case 180:
                    faceUVs = new Vector2[] { uv11, uv10, uv00, uv01 };
                    break;
                case 270:
                    faceUVs = new Vector2[] { uv01, uv11, uv10, uv00 };
                    break;
                default:
                    faceUVs = new Vector2[] { uv00, uv01, uv11, uv10 };
                    break;
            }

            for (int i = 0; i < 4; i++)
            {
                uvs[startIndex + i] = faceUVs[i];
            }
        }

        SetFaceUV(cubeData.uv.north, 0);   // Front
        SetFaceUV(cubeData.uv.south, 4);   // Back
        SetFaceUV(cubeData.uv.west, 8);    // Left
        SetFaceUV(cubeData.uv.east, 12);   // Right
        SetFaceUV(cubeData.uv.up, 16);     // Top
        SetFaceUV(cubeData.uv.down, 20);   // Bottom

        int[] triangles = new int[]
        {
            0, 2, 1, 0, 3, 2,       // Front (north)
            4, 6, 5, 4, 7, 6,       // Back (south)
            8, 10, 9, 8, 11, 10,    // Left (west)
            12, 14, 13, 12, 15, 14, // Right (east)
            16, 18, 17, 16, 19, 18, // Top (up)
            20, 22, 21, 20, 23, 22  // Bottom (down)
        };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        return mesh;
    }
}
